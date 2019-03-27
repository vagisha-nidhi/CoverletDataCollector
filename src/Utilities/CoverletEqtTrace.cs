// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.Utilities
{
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;

    internal class CoverletEqtTrace
    {
        public bool IsInfoEnabled => EqtTrace.IsInfoEnabled;
        public bool IsVerboseEnabled => EqtTrace.IsVerboseEnabled;

        public void Verbose(string format, params object[] args)
        {
            EqtTrace.Verbose(format, args);
        }

        public void Warning(string format, params object[] args)
        {
            EqtTrace.Warning(format, args);
        }

        public void Info(string format, params object[] args)
        {
            EqtTrace.Info(format, args);
        }

        public void Error(string format, params object[] args)
        {
            EqtTrace.Error(format, args);
        }
    }
}