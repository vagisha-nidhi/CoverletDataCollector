﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.DataCollector
{
    using Coverlet.Core;
    using Coverlet.Core.Logging;
    using Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.Interfaces;

    /// <summary>
    /// Implementation for wrapping over Coverage class in coverlet.core
    /// </summary>
    internal class CoverageWrapper : ICoverageWrapper
    {
        /// <summary>
        /// Creates a coverage object from given coverlet settings
        /// </summary>
        /// <param name="settings">Coverlet settings</param>
        /// <returns>Coverage object</returns>
        public Coverage CreateCoverage(CoverletSettings settings, ILogger coverletLogger)
        {
            return new Coverage(
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
        /// Gets the coverage result from provided coverage object
        /// </summary>
        /// <param name="coverage">Coverage</param>
        /// <returns>The coverage result</returns>
        public CoverageResult GetCoverageResult(Coverage coverage)
        {
            return coverage.GetCoverageResult();           
        }

        /// <summary>
        /// Prepares modules for getting coverage.
        /// Wrapper over coverage.PrepareModules
        /// </summary>
        /// <param name="coverage"></param>
        public void PrepareModules(Coverage coverage)
        {
            coverage.PrepareModules();
        }
    }
}
