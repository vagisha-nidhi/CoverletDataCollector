// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.DataCollector
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.Resources;
    using Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.Utilities;

    internal class CoverletSettingsParser
    {
        private readonly CoverletLogger logger;
        private readonly CoverletEqtTrace eqtTrace;

        public CoverletSettingsParser(CoverletLogger logger, CoverletEqtTrace eqtTrace)
        {
            this.logger = logger;
            this.eqtTrace = eqtTrace;
        }

        public CoverletSettings Parse(XmlElement configurationElement, IEnumerable<string> testModules)
        {
            var coverletSettings = new CoverletSettings
            {
                TestModule = this.ParseTestModule(testModules)
            };

            if (configurationElement != null)
            {
                coverletSettings.IncludeFilters = this.ParseIncludeFilters(configurationElement);
                coverletSettings.IncludeDirectories = this.ParseIncludeDirectories(configurationElement);
                coverletSettings.ExcludeFilters = this.ParseExcludeFilters(configurationElement);
                coverletSettings.ExcludeSourceFiles = this.ParseExcludeSourceFiles(configurationElement);
                coverletSettings.ExcludeAttributes = this.ParseExcludeAttributes(configurationElement);
                coverletSettings.MergeWith = this.ParseMergeWith(configurationElement);
                coverletSettings.UseSourceLink = this.ParseUseSourceLink(configurationElement);
            }

            if (this.eqtTrace.IsVerboseEnabled)
            {
                this.eqtTrace.Verbose("{0}: Initializing coverlet process with settings: {1}", CoverletConstants.DataCollectorName, coverletSettings.ToString());
            }

            return coverletSettings;
        }

        private string ParseTestModule(IEnumerable<string> testModules)
        {
            // Validate if atleast one source present.
            if (testModules == null || !testModules.Any())
            {
                this.eqtTrace.Error("{0}: No test modules found", CoverletConstants.DataCollectorName);
                this.logger.LogError(new CoverletDataCollectorException(Resources.NoTestModulesFound));
            }

            // Note:
            // 1) .NET core test run supports one testModule per run. Coverlet also supports one testModule per run. So, we are using first testSource only and ignoring others.
            // 2) If and when .NET full is supported with coverlet OR .NET core starts supporting multiple testModules, revisit this code to use other testModules as well.
            return testModules?.FirstOrDefault();
        }

        private string[] ParseIncludeFilters(XmlElement configurationElement)
        {
            var includeFiltersElement = configurationElement[CoverletConstants.IncludeFiltersElementName];
            return includeFiltersElement?.InnerText?.Split(',');
        }

        private string[] ParseIncludeDirectories(XmlElement configurationElement)
        {
            var includeDirectoriesElement = configurationElement[CoverletConstants.IncludeDirectoriesElementName];
            return includeDirectoriesElement?.InnerText?.Split(',');
        }

        private string[] ParseExcludeFilters(XmlElement configurationElement)
        {
            var excludeFiltersElement = configurationElement[CoverletConstants.ExcludeFiltersElementName];
            return excludeFiltersElement?.InnerText?.Split(',');
        }

        private string[] ParseExcludeSourceFiles(XmlElement configurationElement)
        {
            var excludeSourceFilesElement = configurationElement[CoverletConstants.ExcludeSourceFilesElementName];
            return excludeSourceFilesElement?.InnerText?.Split(',');
        }

        private string[] ParseExcludeAttributes(XmlElement configurationElement)
        {
            var excludeAttributesElement = configurationElement[CoverletConstants.ExcludeAttributesElementName];
            return excludeAttributesElement?.InnerText?.Split(',');
        }

        private string ParseMergeWith(XmlElement configurationElement)
        {
            var mergeWithElement = configurationElement[CoverletConstants.MergeWithElementName];
            return mergeWithElement?.InnerText;
        }

        private bool ParseUseSourceLink(XmlElement configurationElement)
        {
            var useSourceLinkElement = configurationElement[CoverletConstants.UseSourceLinkElementName];
            bool.TryParse(useSourceLinkElement?.InnerText, out var useSourceLink);
            return useSourceLink;
        }
    }
}