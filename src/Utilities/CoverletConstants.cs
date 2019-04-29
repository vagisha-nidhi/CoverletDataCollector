// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.Utilities
{
    internal static class CoverletConstants
    {
        public const string FriendlyName = "XPlat code coverage";
        public const string DefaultUri = @"datacollector://Microsoft/CoverletCodeCoverage/1.0";
        public const string DataCollectorName = "CoverletCoverageDataCollector";
        public const string DefaultReportFormat = "cobertura";
        public const string DefaultFileName = "coverage";
        public const string IncludeFiltersElementName = "Include";
        public const string IncludeDirectoriesElementName = "IncludeDirectory";
        public const string ExcludeFiltersElementName = "Exclude";
        public const string ExcludeSourceFilesElementName = "ExcludeByFile";
        public const string ExcludeAttributesElementName = "ExcludeByAttribute";
        public const string MergeWithElementName = "MergeWith";
        public const string UseSourceLinkElementName = "UseSourceLink";
        public const string SingleHitElementName = "SingleHit";
        public const string TestSourcesPropertyName = "TestSources";
    }
}
