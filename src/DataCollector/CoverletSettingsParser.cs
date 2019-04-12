// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.DataCollector
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.Resources;
    using Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.Utilities;

    /// <summary>
    /// Coverlet settings parser
    /// </summary>
    internal class CoverletSettingsParser
    {
        private readonly TestPlatformEqtTrace eqtTrace;

        public CoverletSettingsParser(TestPlatformEqtTrace eqtTrace)
        {
            this.eqtTrace = eqtTrace;
        }

        /// <summary>
        /// Parser coverlet settings
        /// </summary>
        /// <param name="configurationElement">Configuration element</param>
        /// <param name="testModules">Test modules</param>
        /// <returns>Coverlet settings</returns>
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
                coverletSettings.SingleHit = this.ParseSingleHit(configurationElement);
            }

            if (this.eqtTrace.IsVerboseEnabled)
            {
                this.eqtTrace.Verbose("{0}: Initializing coverlet process with settings: \"{1}\"", CoverletConstants.DataCollectorName, coverletSettings.ToString());
            }

            return coverletSettings;
        }

        /// <summary>
        /// Parses test module
        /// </summary>
        /// <param name="testModules">Test modules</param>
        /// <returns>Test module</returns>
        private string ParseTestModule(IEnumerable<string> testModules)
        {
            // Validate if atleast one source present.
            if (testModules == null || !testModules.Any())
            {
                var errorMessage = string.Format(Resources.NoTestModulesFound, CoverletConstants.DataCollectorName);
                throw new CoverletDataCollectorException(errorMessage);
            }

            // Note:
            // 1) .NET core test run supports one testModule per run. Coverlet also supports one testModule per run. So, we are using first testSource only and ignoring others.
            // 2) If and when .NET full is supported with coverlet OR .NET core starts supporting multiple testModules, revisit this code to use other testModules as well.
            return testModules.FirstOrDefault();
        }

        /// <summary>
        /// Parse filters to include
        /// </summary>
        /// <param name="configurationElement">Configuration element</param>
        /// <returns>Filters to include</returns>
        private string[] ParseIncludeFilters(XmlElement configurationElement)
        {
            var includeFiltersElement = configurationElement[CoverletConstants.IncludeFiltersElementName];
            return includeFiltersElement?.InnerText?.Split(',');
        }

        /// <summary>
        /// Parse directories to include
        /// </summary>
        /// <param name="configurationElement">Configuration element</param>
        /// <returns>Directories to include</returns>
        private string[] ParseIncludeDirectories(XmlElement configurationElement)
        {
            var includeDirectoriesElement = configurationElement[CoverletConstants.IncludeDirectoriesElementName];
            return includeDirectoriesElement?.InnerText?.Split(',');
        }

        /// <summary>
        /// Parse filters to exclude
        /// </summary>
        /// <param name="configurationElement">Configuration element</param>
        /// <returns>Filters to exclude</returns>
        private string[] ParseExcludeFilters(XmlElement configurationElement)
        {
            var excludeFiltersElement = configurationElement[CoverletConstants.ExcludeFiltersElementName];
            return excludeFiltersElement?.InnerText?.Split(',');
        }

        /// <summary>
        /// Parse source files to exclude
        /// </summary>
        /// <param name="configurationElement">Configuration element</param>
        /// <returns>Source files to exclude</returns>
        private string[] ParseExcludeSourceFiles(XmlElement configurationElement)
        {
            var excludeSourceFilesElement = configurationElement[CoverletConstants.ExcludeSourceFilesElementName];
            return excludeSourceFilesElement?.InnerText?.Split(',');
        }

        /// <summary>
        /// Parse attributes to exclude
        /// </summary>
        /// <param name="configurationElement">Configuration element</param>
        /// <returns>Attributes to exclude</returns>
        private string[] ParseExcludeAttributes(XmlElement configurationElement)
        {
            var excludeAttributesElement = configurationElement[CoverletConstants.ExcludeAttributesElementName];
            return excludeAttributesElement?.InnerText?.Split(',');
        }

        /// <summary>
        /// Parse merge with attribute
        /// </summary>
        /// <param name="configurationElement">Configuration element</param>
        /// <returns>Merge with attribute</returns>
        private string ParseMergeWith(XmlElement configurationElement)
        {
            var mergeWithElement = configurationElement[CoverletConstants.MergeWithElementName];
            return mergeWithElement?.InnerText;
        }

        /// <summary>
        /// Parse use source link flag
        /// </summary>
        /// <param name="configurationElement">Configuration element</param>
        /// <returns>Use source link flag</returns>
        private bool ParseUseSourceLink(XmlElement configurationElement)
        {
            var useSourceLinkElement = configurationElement[CoverletConstants.UseSourceLinkElementName];
            bool.TryParse(useSourceLinkElement?.InnerText, out var useSourceLink);
            return useSourceLink;
        }

        /// <summary>
        /// Parse single hit flag
        /// </summary>
        /// <param name="configurationElement">Configuration element</param>
        /// <returns>Single hit flag</returns>
        private bool ParseSingleHit(XmlElement configurationElement)
        {
            var singleHitElement = configurationElement[CoverletConstants.SingleHitElementName];
            bool.TryParse(singleHitElement?.InnerText, out var singleHit);
            return singleHit;
        }
    }
}