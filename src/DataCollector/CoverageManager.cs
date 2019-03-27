// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector
{
    using System;
    using Coverlet.Core;
    using Coverlet.Core.Reporters;

    internal class CoverageManager
    {
        private readonly Coverage coverage;
        private readonly CoverletLogger logger;
        private readonly CoverletEqtTrace eqtTrace;

        public IReporter Reporter { get; }

        public CoverageManager(CoverletSettings settings, CoverletLogger logger, CoverletEqtTrace eqtTrace)
            : this (settings,
                  logger,
                  eqtTrace,
                  new ReporterFactory(CoverletConstants.DefaultReportFormat).CreateReporter())
        {
        }

        public CoverageManager(CoverletSettings settings, CoverletLogger logger, CoverletEqtTrace eqtTrace, IReporter reporter)
        {
            // Store input vars
            this.logger = logger;
            this.eqtTrace = eqtTrace;
            this.Reporter = reporter;

            // Coverage object
            this.coverage = new Coverage(
                settings.TestModule,
                settings.IncludeFilters,
                settings.IncludeDirectories,
                settings.ExcludeFilters,
                settings.ExcludeSourceFiles,
                settings.ExcludeAttributes,
                settings.MergeWith,
                settings.UseSourceLink);
        }

        public void StartInstrumentation()
        {
            try
            {
                // Instrument modules
                this.coverage.PrepareModules();
            }
            catch (Exception ex)
            {
                this.eqtTrace.Error("{0}: Failed to instrument modules with exception {1}", CoverletConstants.DataCollectorName, ex);
                this.logger.LogError(new CoverletDataCollectorException(Resources.InstrumentationException, ex));
            }
        }

        public string GetCoverageReport()
        {
            // Get coverage result
            var coverageResult = this.GetCoverageResult();
            if (coverageResult == null) return null;

            // Get coverage report in default format
            var coverageReport = this.GetCoverageReport(coverageResult);
            return coverageReport;
        }

        private CoverageResult GetCoverageResult()
        {
            try
            {
                return this.coverage.GetCoverageResult();
            }
            catch (Exception ex)
            {
                this.eqtTrace.Error("{0}: Failed to get coverage result with exception {1}", CoverletConstants.DataCollectorName, ex);
                this.logger.LogError(new CoverletDataCollectorException(Resources.CoverageResultException, ex));
            }

            return default(CoverageResult);
        }

        private string GetCoverageReport(CoverageResult coverageResult)
        {
            try
            {
                return this.Reporter.Report(coverageResult);
            }
            catch (Exception ex)

            {
                this.eqtTrace.Error("{0}: Failed to get {1} report with exception {2}", CoverletConstants.DataCollectorName, this.Reporter.Format, ex);
                this.logger.LogError(new CoverletDataCollectorException(Resources.CoverageResultException, ex));
            }

            return default(string);
        }
    }
}
