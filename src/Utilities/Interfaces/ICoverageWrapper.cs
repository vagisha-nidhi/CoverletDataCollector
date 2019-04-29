// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.Interfaces
{
    using Coverlet.Core;
    using Coverlet.Core.Logging;
    using Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.DataCollector;

    /// <summary>
    /// Wrapper interface for Coverage class in coverlet.core
    /// Since the class is not testable, this interface is used to abstract methods for mocking in unit tests.
    /// </summary>
    internal interface ICoverageWrapper
    {
        /// <summary>
        /// Creates a coverage object from given coverlet settings
        /// </summary>
        /// <param name="settings">Coverlet settings</param>
        /// <returns>Coverage object</returns>
        Coverage CreateCoverage(CoverletSettings settings, ILogger logger);

        /// <summary>
        /// Gets the coverage result from provided coverage object
        /// </summary>
        /// <param name="coverage">Coverage</param>
        /// <returns>The coverage result</returns>
        CoverageResult GetCoverageResult(Coverage coverage);

        /// <summary>
        /// Prepares modules for getting coverage.
        /// Wrapper over coverage.PrepareModules
        /// </summary>
        /// <param name="coverage"></param>
        void PrepareModules(Coverage coverage);

    }
}
