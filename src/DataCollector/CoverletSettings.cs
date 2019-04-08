// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.DataCollector
{
    using System.Linq;
    using System.Text;

    internal class CoverletSettings
    {
        public string TestModule { get; set; }

        public string[] IncludeFilters { get; set; }

        public string[] IncludeDirectories { get; set; }

        public string[] ExcludeFilters { get; set; }

        public string[] ExcludeSourceFiles { get; set; }

        public string[] ExcludeAttributes { get; set; }

        public string MergeWith { get; set; }

        public bool UseSourceLink { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendFormat("TestModule: '{0}', ", this.TestModule);
            builder.AppendFormat("IncludeFilters: '{0}', ", string.Join(",", this.IncludeFilters ?? Enumerable.Empty<string>()));
            builder.AppendFormat("IncludeDirectories: '{0}', ", string.Join(",", this.IncludeDirectories ?? Enumerable.Empty<string>()));
            builder.AppendFormat("ExcludeFilters: '{0}', ", string.Join(",", this.ExcludeFilters ?? Enumerable.Empty<string>()));
            builder.AppendFormat("ExcludeSourceFiles: '{0}', ", string.Join(",", this.ExcludeSourceFiles ?? Enumerable.Empty<string>()));
            builder.AppendFormat("ExcludeAttributes: '{0}', ", string.Join(",", this.ExcludeAttributes ?? Enumerable.Empty<string>()));
            builder.AppendFormat("MergeWith: '{0}', ", this.MergeWith);
            builder.AppendFormat("UseSourceLink: '{0}'", this.UseSourceLink);

            return builder.ToString();
        }
    }
}
