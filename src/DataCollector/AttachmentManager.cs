// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.DataCollector
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.Resources;
    using Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.Utilities;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;

    internal class AttachmentManager : IDisposable
    {
        private readonly DataCollectionSink dataSink;
        private readonly CoverletEqtTrace eqtTrace;
        private readonly DataCollectionContext dataCollectionContext;
        private readonly FileHelper fileHelper;
        private readonly DirectoryHelper directoryHelper;
        private readonly string reportFileName;
        private readonly string reportDirectory;

        public AttachmentManager(DataCollectionSink dataSink, DataCollectionContext dataCollectionContext, CoverletEqtTrace eqtTrace, string reportFileName)
            : this(dataSink,
                  dataCollectionContext,
                  eqtTrace,
                  reportFileName,
                  Guid.NewGuid().ToString(),
                  new FileHelper(),
                  new DirectoryHelper())
        {
        }

        public AttachmentManager(DataCollectionSink dataSink, DataCollectionContext dataCollectionContext, CoverletEqtTrace eqtTrace, string reportFileName, string reportDirectoryName, FileHelper fileHelper, DirectoryHelper directoryHelper)
        {
            // Store input vars
            this.dataSink = dataSink;
            this.dataCollectionContext = dataCollectionContext;
            this.eqtTrace = eqtTrace;
            this.reportFileName = reportFileName;
            this.fileHelper = fileHelper;
            this.directoryHelper = directoryHelper;

            // Report directory to store the coverage reports.
            this.reportDirectory = Path.Combine(Path.GetTempPath(), reportDirectoryName);

            // Register events
            this.dataSink.SendFileCompleted += this.OnSendFileCompleted;
        }

        public void Dispose()
        {
            if (this.dataSink != null)
            {
                this.dataSink.SendFileCompleted -= this.OnSendFileCompleted;
            }
        }

        public void SendCoverageReport(string coverageReport)
        {
            // Save coverage report to file
            var coverageReportPath = this.SaveCoverageReport(coverageReport);

            // Send coverage attachment to test platform.
            this.SendAttachment(coverageReportPath);
        }

        private void OnSendFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.eqtTrace.Verbose("{0}: OnSendFileCompleted received", CoverletConstants.DataCollectorName);
            this.CleanupReportDirectory();
        }

        private string SaveCoverageReport(string report)
        {
            try
            {
                this.directoryHelper.CreateDirectory(this.reportDirectory);
                var filePath = Path.Combine(this.reportDirectory, this.reportFileName);
                this.fileHelper.WriteAllText(filePath, report);
                this.eqtTrace.Info("{0}: Saved coverage report to path: '{1}'", CoverletConstants.DataCollectorName, filePath);

                return filePath;
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format(Resources.FailedToSaveCoverageReport, CoverletConstants.DataCollectorName, this.reportFileName, this.reportDirectory);
                throw new CoverletDataCollectorException(errorMessage, ex);
            }
        }

        private void SendAttachment(string attachmentPath)
        {
            if (this.fileHelper.Exists(attachmentPath))
            {
                // Send coverage attachment to test platform.
                this.eqtTrace.Verbose("{0}: Sending attachment to test platform", CoverletConstants.DataCollectorName);
                this.dataSink.SendFileAsync(this.dataCollectionContext, attachmentPath, false);
            }
        }

        private void CleanupReportDirectory()
        {
            try
            {
                if (this.directoryHelper.Exists(this.reportDirectory))
                {
                    this.directoryHelper.Delete(this.reportDirectory, true);
                    this.eqtTrace.Verbose("{0}: Deleted report directory: '{1}'", CoverletConstants.DataCollectorName, this.reportDirectory);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format(Resources.FailedToCleanupReportDirectory, CoverletConstants.DataCollectorName, this.reportDirectory);
                throw new CoverletDataCollectorException(errorMessage, ex);
            }
        }
    }
}
