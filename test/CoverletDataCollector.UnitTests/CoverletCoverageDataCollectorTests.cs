// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace CoverletCoverageDataCollector.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.DataCollector;
    using Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.Utilities;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
    using Moq;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.Interfaces;
    using Coverlet.Core;
    using Coverlet.Core.Logging;

    [TestClass]
    public class CoverletCoverageDataCollectorTests
    {
        private DataCollectionEnvironmentContext context;
        private CoverletCoverageCollector coverletCoverageDataCollector;
        private DataCollectionContext dataCollectionContext;
        private Mock<DataCollectionEvents> mockDataColectionEvents;
        private Mock<DataCollectionSink> mockDataCollectionSink;
        private Mock<ICoverageWrapper> mockCoverageWrapper;
        private XmlElement configurationElement;
        private Mock<DataCollectionLogger> mockLogger;

        public CoverletCoverageDataCollectorTests()
        {
            this.mockDataColectionEvents = new Mock<DataCollectionEvents>();
            this.mockDataCollectionSink = new Mock<DataCollectionSink>();
            this.mockLogger = new Mock<DataCollectionLogger>();
            this.configurationElement = null;

            TestCase testcase = new TestCase { Id = Guid.NewGuid() };
            this.dataCollectionContext = new DataCollectionContext(testcase);
            this.context = new DataCollectionEnvironmentContext(this.dataCollectionContext);
            this.mockCoverageWrapper = new Mock<ICoverageWrapper>();
        }

        [TestMethod]
        public void OnSessionStartShouldInitializeCoverageWithCorrectCoverletSettings()
        {
            coverletCoverageDataCollector = new CoverletCoverageCollector(new TestPlatformEqtTrace(), this.mockCoverageWrapper.Object);
            coverletCoverageDataCollector.Initialize(
                    this.configurationElement,
                    this.mockDataColectionEvents.Object,
                    this.mockDataCollectionSink.Object,
                    null,
                    this.context);
            IDictionary<string, object> sessionStartProperties = new Dictionary<string, object>();

            sessionStartProperties.Add("TestSources", new List<string> { "abc.dll" });

            this.mockDataColectionEvents.Raise(x => x.SessionStart += null, new SessionStartEventArgs(sessionStartProperties));

            this.mockCoverageWrapper.Verify(x => x.CreateCoverage(It.Is<CoverletSettings>(y => string.Equals(y.TestModule, "abc.dll")), It.IsAny<ILogger>()), Times.Once);
        }

        [TestMethod]
        public void OnSessionStartShouldPrepareModulesForCoverage()
        {
            coverletCoverageDataCollector = new CoverletCoverageCollector(new TestPlatformEqtTrace(), this.mockCoverageWrapper.Object);
            coverletCoverageDataCollector.Initialize(
                    this.configurationElement,
                    this.mockDataColectionEvents.Object,
                    this.mockDataCollectionSink.Object,
                    null,
                    this.context);
            IDictionary<string, object> sessionStartProperties = new Dictionary<string, object>();
            Coverage coverage = new Coverage("abc.dll", null, null, null, null, null, true, "abc.json", true, It.IsAny<ILogger>());

            sessionStartProperties.Add("TestSources", new List<string> { "abc.dll" });
            this.mockCoverageWrapper.Setup(x => x.CreateCoverage(It.IsAny<CoverletSettings>(), It.IsAny<ILogger>())).Returns(coverage);

            this.mockDataColectionEvents.Raise(x => x.SessionStart += null, new SessionStartEventArgs(sessionStartProperties));

            this.mockCoverageWrapper.Verify(x => x.CreateCoverage(It.Is<CoverletSettings>(y => y.TestModule.Contains("abc.dll")), It.IsAny<ILogger>()), Times.Once);
            this.mockCoverageWrapper.Verify(x => x.PrepareModules(It.IsAny<Coverage>()), Times.Once);
        }

        [TestMethod]
        public void OnSessionEndShouldSendGetCoverageReportToTestPlatform()
        {
            coverletCoverageDataCollector = new CoverletCoverageCollector(new TestPlatformEqtTrace(), new CoverageWrapper());
            coverletCoverageDataCollector.Initialize(
                    this.configurationElement,
                    this.mockDataColectionEvents.Object,
                    this.mockDataCollectionSink.Object,
                    null,
                    this.context);

            string module = GetType().Assembly.Location;
            string pdb = Path.Combine(Path.GetDirectoryName(module), Path.GetFileNameWithoutExtension(module) + ".pdb");

            var directory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));

            File.Copy(module, Path.Combine(directory.FullName, Path.GetFileName(module)), true);
            File.Copy(pdb, Path.Combine(directory.FullName, Path.GetFileName(pdb)), true);

            IDictionary<string, object> sessionStartProperties = new Dictionary<string, object>();
            sessionStartProperties.Add("TestSources", new List<string> { Path.Combine(directory.FullName, Path.GetFileName(module)) });

            this.mockDataColectionEvents.Raise(x => x.SessionStart += null, new SessionStartEventArgs(sessionStartProperties));
            this.mockDataColectionEvents.Raise(x => x.SessionEnd += null, new SessionEndEventArgs());

            this.mockDataCollectionSink.Verify(x => x.SendFileAsync(It.IsAny<FileTransferInformation>()), Times.Once);

            directory.Delete(true);
        }

        [TestMethod]
        public void OnSessionStartShouldLogWarningIfInstrumentationFailed()
        {
            coverletCoverageDataCollector = new CoverletCoverageCollector(new TestPlatformEqtTrace(), this.mockCoverageWrapper.Object);
            coverletCoverageDataCollector.Initialize(
                    this.configurationElement,
                    this.mockDataColectionEvents.Object,
                    this.mockDataCollectionSink.Object,
                    this.mockLogger.Object,
                    this.context);
            IDictionary<string, object> sessionStartProperties = new Dictionary<string, object>();

            sessionStartProperties.Add("TestSources", new List<string> { "abc.dll" });

            this.mockCoverageWrapper.Setup(x => x.PrepareModules(It.IsAny<Coverage>())).Throws(new FileNotFoundException());
            
            this.mockDataColectionEvents.Raise(x => x.SessionStart += null, new SessionStartEventArgs(sessionStartProperties));

            this.mockLogger.Verify(x => x.LogWarning(this.dataCollectionContext,
                "Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.Utilities.CoverletDataCollectorException: CoverletCoverageDataCollector: Failed to instrument modules ---> System.IO.FileNotFoundException: Unable to find the specified file.\r\n   at Moq.MethodCall.ThrowExceptionResponse.RespondTo(Invocation invocation) in C:\\projects\\moq4\\src\\Moq\\MethodCall.cs:line 465\r\n   at Moq.MethodCall.Execute(Invocation invocation) in C:\\projects\\moq4\\src\\Moq\\MethodCall.cs:line 109\r\n   at Moq.FindAndExecuteMatchingSetup.Handle(Invocation invocation, Mock mock) in C:\\projects\\moq4\\src\\Moq\\Interception\\InterceptionAspects.cs:line 137\r\n   at Moq.Mock.Moq.IInterceptor.Intercept(Invocation invocation) in C:\\projects\\moq4\\src\\Moq\\Interception\\Mock.cs:line 20\r\n   at Castle.DynamicProxy.AbstractInvocation.Proceed()\r\n   at Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.DataCollector.CoverageManager.InstrumentModules() in E:\\CodeBase\\CoverletDataCollector\\src\\DataCollector\\CoverageManager.cs:line 51\r\n   --- End of inner exception stack trace ---\r\n   at Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.DataCollector.CoverageManager.InstrumentModules() in E:\\CodeBase\\CoverletDataCollector\\src\\DataCollector\\CoverageManager.cs:line 56\r\n   at Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.DataCollector.CoverletCoverageCollector.OnSessionStart(Object sender, SessionStartEventArgs sessionStartEventArgs) in E:\\CodeBase\\CoverletDataCollector\\src\\DataCollector\\CoverletCoverageDataCollector.cs:line 115"));
        }
    }
}
