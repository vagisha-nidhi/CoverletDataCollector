// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector
{
    using System;
    using System.ComponentModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;

    internal class AttachmentManager : IDisposable
    {
        private readonly DataCollectionSink dataSink;
        private readonly CoverletLogger logger;
        private readonly CoverletEqtTrace eqtTrace;
        private readonly DataCollectionContext dataCollectionContext;
        private readonly FileHelper fileHelper;
        private readonly string reportFileName;
        private readonly string reportDirectory;

        public AttachmentManager(DataCollectionSink dataSink, DataCollectionContext dataCollectionContext, CoverletLogger logger, CoverletEqtTrace eqtTrace, string reportFileName)
            : this(dataSink,
                  dataCollectionContext,
                  logger,
                  eqtTrace,
                  reportFileName,
                  Guid.NewGuid().ToString(),
                  new FileHelper())
        {
        }

        public AttachmentManager(DataCollectionSink dataSink, DataCollectionContext dataCollectionContext, CoverletLogger logger, CoverletEqtTrace eqtTrace, string reportFileName, string reportDirectoryName, FileHelper fileHelper)
        {
            // Store input vars
            this.dataSink = dataSink;
            this.dataCollectionContext = dataCollectionContext;
            this.logger = logger;
            this.eqtTrace = eqtTrace;
            this.reportFileName = reportFileName;
            this.fileHelper = fileHelper;

            // Report directory to store the coverage reports.
            this.reportDirectory = this.fileHelper.Combine(this.fileHelper.GetTempPath(), reportDirectoryName);

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
            if (string.IsNullOrWhiteSpace(coverageReportPath)) return;

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
                this.fileHelper.CreateDirectory(this.reportDirectory);
                var filePath = this.fileHelper.Combine(this.reportDirectory, this.reportFileName);
                this.fileHelper.WriteAllText(filePath, report);
                this.eqtTrace.Info("{0}: Saved coverage report to path: {1}", CoverletConstants.DataCollectorName, filePath);

                return filePath;
            }
            catch (Exception ex)
            {
                this.eqtTrace.Error("{0}: Failed to save coverage report to file: {1} in directory: {2} with exception {3}", CoverletConstants.DataCollectorName, this.reportFileName, this.reportDirectory, ex);
                this.logger.LogError(new CoverletDataCollectorException(Resources.FailedToSaveCoverageReport, ex));
            }

            return default(string);
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
                if (this.fileHelper.Exists(this.reportDirectory))
                {
                    this.fileHelper.Delete(this.reportDirectory, true);
                    this.eqtTrace.Verbose("{0}: Deleted report directory: {1}", CoverletConstants.DataCollectorName, this.reportDirectory);
                }
            }
            catch (Exception ex)
            {
                this.eqtTrace.Warning("{0}: Failed to delete report directory: {1} with exception {2}", CoverletConstants.DataCollectorName, this.reportDirectory, ex);
            }
        }
    }
}
