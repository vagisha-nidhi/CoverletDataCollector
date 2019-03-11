// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.TestPlatform.Extensions.CoverletRunSettingsProvider
{
    internal static class CoverletConstants
    {
        public const string InProcDataCollectorSettingsType = "InProcDataCollectionRunSettings";
        public const string InProcDataCollectorFriendlyName = "XPlat inproc code coverage";
        public const string InProcDataCollectorUri = "datacollector://Coverlet/InprocCodeCoverage/1.0"; // TODO: make changes in inproc data collector.
    }
}
