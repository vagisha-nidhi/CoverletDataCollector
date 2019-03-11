// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector
{
    using System.Xml;
    using Coverlet.Core;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;

    [DataCollectorTypeUri(CoverletConstants.DataCollectorDefaultUri)]
    [DataCollectorFriendlyName(CoverletConstants.DataCollectorFriendlyName)]
    public class CoverletCoverageDataCollector : DataCollector
    {
        // Hack 1:
        // Vstest console supports multiple test modules in one pass but coverlet supports only one test module at a time for instrumentation. So we are doing following hack until coverlet supports multiple test modules in one pass:
        // 1. Pass first test module as test module to coverlet.
        // 2. In include directories, add directories of all test modules so that modules in those directories can be included.
        // 3. In exclude filters, exclude all test modules so that test modules will not be instrumented.
        // These above steps are hack until Coverlet supports multiple test module in one pass.
        // Hack 2:
        // Coverlet.core doesn't create result attachments for output formats. It gives the CoverageResult object. Consumer of coverlet needs to convert the CoverageResult object to result attachments.
        // Ideally, this should be done by coverlet.core. Until that happens, we are doing following hack:
        // 1. CoverletDataCollector is responsible for creating result attachments from CoverageResult object.
        // 2. Code is duplicate of what is currently present in Coverlet.msbuild and Coverlet.console
        private DataCollectionEvents events;
        private DataCollectionLogger logger;
        private DataCollectionContext dataCollectorContext;
        private CoverletSettings settings;
        private CoverageManager coverageManager;
        private AttachmentManager attachmentManager;

        public override void Initialize(
            XmlElement configurationElement,
            DataCollectionEvents events,
            DataCollectionSink dataSink,
            DataCollectionLogger logger,
            DataCollectionEnvironmentContext environmentContext)
        {
            // TODO: Think about logs, telemtry, console output etc.
            // TODO: thinks about design mode, command line for both Run all and Run selected scenarios.
            EqtTrace.Info("Enabling coverlet datacollector with configuration: {0}", configurationElement?.OuterXml); // TODO: If testplatform is already outputing info of datacollector initialization, don't put it here.

            // Store input variables
            this.events = events;
            this.logger = logger;
            this.dataCollectorContext = environmentContext.SessionDataCollectionContext;
            this.settings = new CoverletSettings(configurationElement);
            this.coverageManager = new CoverageManager(this.settings);
            this.attachmentManager = new AttachmentManager(dataSink);

            // Register events
            this.events.SessionStart += this.OnSessionStart;
            this.events.SessionEnd += this.OnSessionEnd;

            // Start instrumentation async
            this.coverageManager.StartInstrumentationAsync(); // TODO: as we are not awaiting, errors in this async will be eaten up. Ensure the error cases works as expected.
        }

        protected override void Dispose(bool disposing)
        {
            // TODO: Ensure that all files are cleaned up. Ideally this will be done by sessionend. Check if we can have a case where dispose is called before sessionend call. If yes, we need to cleanup here as well.
            // TODO: check if this method is ever invoked.
            // Unregister events
            this.events.SessionStart -= this.OnSessionStart;
            this.events.SessionEnd -= this.OnSessionEnd;

            this.coverageManager.Dispose();
            this.attachmentManager.Dispose();

            // Call base dispose
            base.Dispose(disposing);
        }

        private void OnSessionStart(object sender, SessionStartEventArgs e)
        {
            this.coverageManager.WaitForInstrumentationComplete();
        }

        private void OnSessionEnd(object sender, SessionEndEventArgs e)
        {
            CoverageResult coverageResult = this.coverageManager.GetCoverageResult();
            this.attachmentManager.CreateAndSendCoverageAttachments(coverageResult);
        }
    }
}
