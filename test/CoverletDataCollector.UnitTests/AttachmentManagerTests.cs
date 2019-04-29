// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace CoverletCoverageDataCollector.UnitTests
{
    using System;
    using System.ComponentModel;
    using System.IO;

    using Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.DataCollector;
    using Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.Utilities;
    using Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.Utilities.Interfaces;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class AttachmentManagerTests
    {
        private AttachmentManager attachmentManager;
        private Mock<DataCollectionSink> mockDataCollectionSink;
        private DataCollectionContext dataCollectionContext;
        private TestPlatformLogger testPlatformLogger;
        private TestPlatformEqtTrace eqtTrace;
        private Mock<IFileHelper> mockFileHelper;
        private Mock<IDirectoryHelper> mockDirectoryHelper;
        private Mock<DataCollectionLogger> mockDataCollectionLogger;
        public AttachmentManagerTests()
        {
            this.mockDataCollectionSink = new Mock<DataCollectionSink>();
            this.mockDataCollectionLogger = new Mock<DataCollectionLogger>();
            TestCase testcase = new TestCase { Id = Guid.NewGuid() };
            this.dataCollectionContext = new DataCollectionContext(testcase);
            this.testPlatformLogger = new TestPlatformLogger(this.mockDataCollectionLogger.Object, this.dataCollectionContext);
            this.eqtTrace = new TestPlatformEqtTrace();
            this.mockFileHelper = new Mock<IFileHelper>();
            this.mockDirectoryHelper = new Mock<IDirectoryHelper>();

            this.attachmentManager = new AttachmentManager(this.mockDataCollectionSink.Object, this.dataCollectionContext, this.testPlatformLogger,
                this.eqtTrace, "report.cobertura.xml", @"E:\temp", this.mockFileHelper.Object, this.mockDirectoryHelper.Object);
        }

        [TestMethod]
        public void SendCoverageReportShouldSaveReportToFile()
        {
            string coverageReport = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
                                    + "<coverage line-rate=\"1\" branch-rate=\"1\" version=\"1.9\" timestamp=\"1556263787\" lines-covered=\"0\" lines-valid=\"0\" branches-covered=\"0\" branches-valid=\"0\">"
                                    + "<sources/>"
                                    + "<packages/>"
                                    + "</coverage>";
            this.attachmentManager.SendCoverageReport(coverageReport);
            this.mockFileHelper.Verify(x => x.WriteAllText(@"E:\temp\report.cobertura.xml", coverageReport), Times.Once);
        }

        [TestMethod]
        public void SendCoverageReportShouldThrowExceptionWhenFailedToSaveReportToFile()
        {
            this.attachmentManager = new AttachmentManager(this.mockDataCollectionSink.Object, this.dataCollectionContext, this.testPlatformLogger,
               this.eqtTrace, null, @"E:\temp", this.mockFileHelper.Object, this.mockDirectoryHelper.Object);

            string coverageReport = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
                                    + "<coverage line-rate=\"1\" branch-rate=\"1\" version=\"1.9\" timestamp=\"1556263787\" lines-covered=\"0\" lines-valid=\"0\" branches-covered=\"0\" branches-valid=\"0\">"
                                    + "<sources/>"
                                    + "<packages/>"
                                    + "</coverage>";

            var message = Assert.ThrowsException<CoverletDataCollectorException>(() => this.attachmentManager.SendCoverageReport(coverageReport)).Message;
            Assert.AreEqual(message, "CoverletCoverageDataCollector: Failed to save coverage report '' in directory 'E:\\temp'");
        }

        [TestMethod]
        public void SendCoverageReportShouldSendAttachmentToTestPlatform()
        {
            var directory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
            this.attachmentManager = new AttachmentManager(this.mockDataCollectionSink.Object, this.dataCollectionContext, this.testPlatformLogger,
               this.eqtTrace, "report.cobertura.xml", directory.ToString(), new FileHelper(), this.mockDirectoryHelper.Object);

            string coverageReport = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
                                    + "<coverage line-rate=\"1\" branch-rate=\"1\" version=\"1.9\" timestamp=\"1556263787\" lines-covered=\"0\" lines-valid=\"0\" branches-covered=\"0\" branches-valid=\"0\">"
                                    + "<sources/>"
                                    + "<packages/>"
                                    + "</coverage>";

            this.attachmentManager.SendCoverageReport(coverageReport);

            this.mockDataCollectionSink.Verify(x => x.SendFileAsync(It.IsAny<FileTransferInformation>()));

            directory.Delete(true);
        }

        [TestMethod]
        public void OnSendFileCompletedShouldCleanUpReportDirectory()
        {
            this.mockDirectoryHelper.Setup(x => x.Exists(@"E:\temp")).Returns(true);

            this.mockDataCollectionSink.Raise(x => x.SendFileCompleted += null, new AsyncCompletedEventArgs(null, false, null));
            
            this.mockDirectoryHelper.Verify(x => x.Delete(@"E:\temp", true), Times.Once);
        }

        [TestMethod]
        public void OnSendFileCompletedShouldThrowCoverletDataCollectorExceptionIfUnableToCleanUpReportDirectory()
        {
            this.mockDirectoryHelper.Setup(x => x.Exists(@"E:\temp")).Returns(true);
            this.mockDirectoryHelper.Setup(x => x.Delete(@"E:\temp", true)).Throws(new FileNotFoundException());

            this.mockDataCollectionSink.Raise(x => x.SendFileCompleted += null, new AsyncCompletedEventArgs(null, false, null));
            this.mockDataCollectionLogger.Verify(x => x.LogWarning(this.dataCollectionContext, 
                "Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.Utilities.CoverletDataCollectorException: CoverletCoverageDataCollector: Failed to cleanup report directory: 'E:\\temp' ---> System.IO.FileNotFoundException: Unable to find the specified file.\r\n   at Moq.MethodCall.ThrowExceptionResponse.RespondTo(Invocation invocation) in C:\\projects\\moq4\\src\\Moq\\MethodCall.cs:line 465\r\n   at Moq.MethodCall.Execute(Invocation invocation) in C:\\projects\\moq4\\src\\Moq\\MethodCall.cs:line 109\r\n   at Moq.FindAndExecuteMatchingSetup.Handle(Invocation invocation, Mock mock) in C:\\projects\\moq4\\src\\Moq\\Interception\\InterceptionAspects.cs:line 137\r\n   at Moq.Mock.Moq.IInterceptor.Intercept(Invocation invocation) in C:\\projects\\moq4\\src\\Moq\\Interception\\Mock.cs:line 20\r\n   at Castle.DynamicProxy.AbstractInvocation.Proceed()\r\n   at Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.DataCollector.AttachmentManager.CleanupReportDirectory() in E:\\CodeBase\\CoverletDataCollector\\src\\DataCollector\\AttachmentManager.cs:line 148\r\n   --- End of inner exception stack trace ---\r\n   at Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.DataCollector.AttachmentManager.CleanupReportDirectory() in E:\\CodeBase\\CoverletDataCollector\\src\\DataCollector\\AttachmentManager.cs:line 155\r\n   at Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.DataCollector.AttachmentManager.OnSendFileCompleted(Object sender, AsyncCompletedEventArgs e) in E:\\CodeBase\\CoverletDataCollector\\src\\DataCollector\\AttachmentManager.cs:line 116"), Times.Once);
        }

    }
}
