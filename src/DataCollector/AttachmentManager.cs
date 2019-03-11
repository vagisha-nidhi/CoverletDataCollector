// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector
{
    using System;
    using System.ComponentModel;
    using Coverlet.Core;
    using Coverlet.Core.Reporters;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;

    internal class AttachmentManager : IDisposable
    {
        private readonly DataCollectionSink dataSink;

        public AttachmentManager(DataCollectionSink dataSink)
        {
            this.dataSink = dataSink;

            // Register events
            this.dataSink.SendFileCompleted += this.OnSendFileCompleted;
        }

        public void Dispose()
        {
            // TODO: should we do file cleanup also here? OnSendFileCompleted is already doing cleanup.
            this.dataSink.SendFileCompleted -= this.OnSendFileCompleted;
        }

        public void CreateAndSendCoverageAttachments(CoverageResult coverageResult)
        {
            // TODO: Catch exception as we have IO operations.
            string attachmentPath = this.CreateAttachment(coverageResult);
            this.SendAttachment(attachmentPath);
        }

        private void SendAttachment(string attachmentPath)
        {
            throw new NotImplementedException();
        }

        private string CreateAttachment(CoverageResult coverageResult)
        {
            var reporter = new ReporterFactory("json").CreateReporter();
            var serializedResult = reporter.Report(coverageResult);
            throw new NotImplementedException();
        }

        private void OnSendFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // TODO: file cleanup
        }
    }
}
