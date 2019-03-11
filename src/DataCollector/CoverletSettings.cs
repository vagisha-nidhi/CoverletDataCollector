// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector
{
    using System;
    using System.Xml;

    internal class CoverletSettings
    {
        public CoverletSettings(XmlElement configurationElement)
        {
            this.ParseCoverletSettings(configurationElement);
        }

        public string[] IncludeFilters { get; internal set; }

        public string[] IncludeDirectories { get; internal set; }

        public string[] ExcludeFilters { get; internal set; }

        public string[] ExcludeSourceFiles { get; internal set; }

        public string[] ExcludeAttributes { get; internal set; }

        public string MergeWith { get; internal set; }

        public bool UseSourceLink { get; internal set; }

        public string[] TestModules { get; internal set; }

        private void ParseCoverletSettings(XmlElement configurationElement)
        {
            this.ParseIncludeFilters();
            this.ParseIncludeDirectories();
            this.ParseExcludeFilters();
            this.ParseExcludeFilters();
            this.ParseExcludeSourceFiles();
            this.ParseMergeWith();
            this.ParseUseSourceLink();
            this.ParseTestModules();

            // TODO: If code is common for parsing, merge the logic.
        }

        private void ParseTestModules()
        {
            // TODO: In all these methods, do validations as user can put anything. In case of invalidations, decide whether to throw error OR ignore those attribute with a warning.
            throw new NotImplementedException();
        }

        private void ParseUseSourceLink()
        {
            throw new NotImplementedException();
        }

        private void ParseMergeWith()
        {
            throw new NotImplementedException();
        }

        private void ParseExcludeSourceFiles()
        {
            throw new NotImplementedException();
        }

        private void ParseIncludeFilters()
        {
            throw new NotImplementedException();
        }

        private void ParseExcludeFilters()
        {
            throw new NotImplementedException();
        }

        private void ParseIncludeDirectories()
        {
            // TODO: Directories of test modules should also be included.
            throw new NotImplementedException();
        }
    }
}
