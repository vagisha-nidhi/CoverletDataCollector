// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.DataCollector
{
    using System.Collections.Generic;
    using System.Xml;
    using Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.Utilities;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;

    [DataCollectorTypeUri(CoverletConstants.DefaultUri)]
    [DataCollectorFriendlyName(CoverletConstants.FriendlyName)]
    public class CoverletCoverageDataCollector : DataCollector
    {
        private readonly CoverletEqtTrace eqtTrace;
        private DataCollectionEvents events;
        private CoverletLogger logger;
        private XmlElement configurationElement;
        private DataCollectionSink dataSink;
        private DataCollectionContext dataCollectionContext;
        private CoverageManager coverageManager;
        private AttachmentManager attachmentManager;

        public CoverletCoverageDataCollector() : this(new CoverletEqtTrace())
        {
        }

        private CoverletCoverageDataCollector(CoverletEqtTrace eqtTrace) : base()
        {
            this.eqtTrace = eqtTrace;
        }

        public override void Initialize(
            XmlElement configurationElement,
            DataCollectionEvents events,
            DataCollectionSink dataSink,
            DataCollectionLogger logger,
            DataCollectionEnvironmentContext environmentContext)
        {
            if (this.eqtTrace.IsInfoEnabled)
            {
                this.eqtTrace.Info("Initializing {0} with configuration: {1}", CoverletConstants.DataCollectorName, configurationElement?.OuterXml);
            }

            // Store input variables
            this.events = events;
            this.configurationElement = configurationElement;
            this.dataSink = dataSink;
            this.dataCollectionContext = environmentContext.SessionDataCollectionContext;
            this.logger = new CoverletLogger(logger, this.dataCollectionContext);

            // Register events
            this.events.SessionStart += this.OnSessionStart;
            this.events.SessionEnd += this.OnSessionEnd;
        }

        protected override void Dispose(bool disposing)
        {
            this.eqtTrace.Verbose("{0}: Disposing", CoverletConstants.DataCollectorName);

            // Unregister events
            if (this.events != null)
            {
                this.events.SessionStart -= this.OnSessionStart;
                this.events.SessionEnd -= this.OnSessionEnd;
            }

            // Dispose
            this.attachmentManager?.Dispose();
            base.Dispose(disposing);
        }

        private void OnSessionStart(object sender, SessionStartEventArgs sessionStartEventArgs)
        {
            this.eqtTrace.Verbose("{0}: SessionStart received", CoverletConstants.DataCollectorName);

            // Get coverlet settings
            IEnumerable<string> testModules = this.GetTestModules(sessionStartEventArgs);
            var coverletSettingsParser = new CoverletSettingsParser(this.logger, this.eqtTrace);
            var coverletSettings = coverletSettingsParser.Parse(this.configurationElement, testModules);

            // Get coverage and attachment managers
            this.coverageManager = new CoverageManager(coverletSettings, this.logger, this.eqtTrace);
            this.attachmentManager = new AttachmentManager(dataSink, this.dataCollectionContext, this.logger, this.eqtTrace, this.GetReportFileName());
            
            // Start instrumentation
            this.coverageManager.StartInstrumentation();
        }

        private string GetReportFileName()
        {
            var fileName = CoverletConstants.DefaultFileName;
            var extension = this.coverageManager.Reporter.Extension;

            return $"{fileName}.{extension}";
        }

        private void OnSessionEnd(object sender, SessionEndEventArgs e)
        {
            this.eqtTrace.Verbose("{0}: SessionEnd received", CoverletConstants.DataCollectorName);

            // Get coverage reports
            var coverageReport = this.coverageManager.GetCoverageReport();

            // Send result attachments to test platform.
            this.attachmentManager.SendCoverageReport(coverageReport);
        }

        private IEnumerable<string> GetTestModules(SessionStartEventArgs sessionStartEventArgs)
        {
            var testModules = sessionStartEventArgs.GetPropertyValue<IEnumerable<string>>("TestSources");
            if (this.eqtTrace.IsInfoEnabled)
            {
                this.eqtTrace.Info("{0}: TestModules: {1}", CoverletConstants.DataCollectorName, string.Join(",", testModules));
            }

            return testModules;
        }
    }
}
