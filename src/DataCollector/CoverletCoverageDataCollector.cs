// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.DataCollector
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            System.Diagnostics.Debugger.Launch();
            if (this.eqtTrace.IsInfoEnabled)
            {
                this.eqtTrace.Info("Initializing {0} with configuration: '{1}'", CoverletConstants.DataCollectorName, configurationElement?.OuterXml);
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

            // Remove vars
            this.events = null;
            this.coverageManager = null;
            this.attachmentManager = null;

            base.Dispose(disposing);
        }

        private void OnSessionStart(object sender, SessionStartEventArgs sessionStartEventArgs)
        {
            this.eqtTrace.Verbose("{0}: SessionStart received", CoverletConstants.DataCollectorName);

            try
            {
                // Get coverlet settings
                IEnumerable<string> testModules = this.GetTestModules(sessionStartEventArgs);
                var coverletSettingsParser = new CoverletSettingsParser(this.eqtTrace);
                var coverletSettings = coverletSettingsParser.Parse(this.configurationElement, testModules);

                // Get coverage and attachment managers
                this.coverageManager = new CoverageManager(coverletSettings);
                this.attachmentManager = new AttachmentManager(dataSink, this.dataCollectionContext, this.eqtTrace, this.GetReportFileName());

                // Start instrumentation
                this.coverageManager.StartInstrumentation();
            }
            catch(Exception ex)
            {
                // TODO: inner exception
                // TODO: anything special for CoverletException type or generic exception type. Do similar in session end as well.
                this.logger.LogWarning(ex.ToString());
                this.Dispose(true);
            }
        }

        private void OnSessionEnd(object sender, SessionEndEventArgs e)
        {
            try
            {
                this.eqtTrace.Verbose("{0}: SessionEnd received", CoverletConstants.DataCollectorName);

                // Get coverage reports
                var coverageReport = this.coverageManager?.GetCoverageReport();

                // Send result attachments to test platform.
                this.attachmentManager?.SendCoverageReport(coverageReport);
            }
            catch (Exception ex)
            {
                this.logger.LogWarning(ex.ToString());
                this.Dispose(true);
            }
        }

        private string GetReportFileName()
        {
            var fileName = CoverletConstants.DefaultFileName;
            var extension = this.coverageManager?.Reporter.Extension;

            return extension == null ? fileName : $"{fileName}.{extension}";
        }

        private IEnumerable<string> GetTestModules(SessionStartEventArgs sessionStartEventArgs)
        {
            var testModules = sessionStartEventArgs.GetPropertyValue<IEnumerable<string>>(CoverletConstants.TestSourcesPropertyName);
            if (this.eqtTrace.IsInfoEnabled)
            {
                this.eqtTrace.Info("{0}: TestModules: '{1}'",
                    CoverletConstants.DataCollectorName,
                    string.Join(",", testModules ?? Enumerable.Empty<string>()));
            }

            return testModules;
        }
    }
}
