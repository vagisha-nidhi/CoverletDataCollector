// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.DataCollector
{
    using System;
    using Coverlet.Core;
    using Coverlet.Core.Logging;
    using Coverlet.Core.Reporters;
    using Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.Resources;
    using Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.Utilities;

    /// <summary>
    /// Manages coverlet coverage
    /// </summary>
    internal class CoverageManager
    {
        private readonly Coverage coverage;

        public IReporter Reporter { get; }

        public CoverageManager(CoverletSettings settings, TestPlatformEqtTrace eqtTrace, TestPlatformLogger logger)
            : this (settings,
                  new ReporterFactory(CoverletConstants.DefaultReportFormat).CreateReporter(),
                  new CoverletLogger(eqtTrace, logger))
        {
        }

        public CoverageManager(CoverletSettings settings, IReporter reporter, ILogger coverletLogger)
        {
            // Store input vars
            this.Reporter = reporter;

            // Coverage object
            this.coverage = new Coverage(
                settings.TestModule,
                settings.IncludeFilters,
                settings.IncludeDirectories,
                settings.ExcludeFilters,
                settings.ExcludeSourceFiles,
                settings.ExcludeAttributes,
                settings.SingleHit,
                settings.MergeWith,
                settings.UseSourceLink,
                coverletLogger);
        }

        /// <summary>
        /// Instrument modules
        /// </summary>
        public void InstrumentModules()
        {
            try
            {
                // Instrument modules
                this.coverage.PrepareModules();
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format(Resources.InstrumentationException, CoverletConstants.DataCollectorName);
                throw new CoverletDataCollectorException(errorMessage, ex);
            }
        }

        /// <summary>
        /// Gets coverlet coverage report
        /// </summary>
        /// <returns>Coverage report</returns>
        public string GetCoverageReport()
        {
            // Get coverage result
            var coverageResult = this.GetCoverageResult();

            // Get coverage report in default format
            var coverageReport = this.GetCoverageReport(coverageResult);
            return coverageReport;
        }

        /// <summary>
        /// Gets coverlet coverage result
        /// </summary>
        /// <returns>Coverage result</returns>
        private CoverageResult GetCoverageResult()
        {
            try
            {
                return this.coverage.GetCoverageResult();
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format(Resources.CoverageResultException, CoverletConstants.DataCollectorName);
                throw new CoverletDataCollectorException(errorMessage, ex);
            }
        }

        /// <summary>
        /// Gets coverage report from coverage result
        /// </summary>
        /// <param name="coverageResult">Coverage result</param>
        /// <returns>Coverage report</returns>
        private string GetCoverageReport(CoverageResult coverageResult)
        {
            try
            {
                return this.Reporter.Report(coverageResult);
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format(Resources.CoverageReportException, CoverletConstants.DataCollectorName);
                throw new CoverletDataCollectorException(errorMessage, ex);
            }
        }
    }
}
