// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.DataCollector
{
    using System;
    using Coverlet.Core.Logging;
    using Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.Utilities;

    internal class CoverletLogger : ILogger
    {
        private readonly TestPlatformEqtTrace eqtTrace;
        private readonly TestPlatformLogger logger;

        public CoverletLogger(TestPlatformEqtTrace eqtTrace, TestPlatformLogger logger)
        {
            this.eqtTrace = eqtTrace;
            this.logger = logger;
        }

        public void LogError(string message)
        {
            this.logger.LogWarning(message);
        }

        public void LogError(Exception exception)
        {
            this.logger.LogWarning(exception.ToString());
        }

        public void LogInformation(string message)
        {
            this.eqtTrace.Info(message);
        }

        public void LogVerbose(string message)
        {
            this.eqtTrace.Verbose(message);
        }

        public void LogWarning(string message)
        {
            this.eqtTrace.Warning(message);
        }
    }
}
