// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.TestPlatform.Extensions.CoverletRunSettingsProvider
{
    // TODO: Name of this assembly ends with DataCollector.dll so will it be picked as extension in vstest.console?
    // TODO: We are taking direct project dependency of ObjectModel. Chanage it to nuget package once RunSettingsProvider is shipped to nuget.
    using System.Xml;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.RunSettingsProvider;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.RunSettingsProvider.Attributes;

    [SettingType(CoverletConstants.InProcDataCollectorSettingsType)]
    [SettingFriendlyName(CoverletConstants.InProcDataCollectorFriendlyName)]
    [SettingUri(CoverletConstants.InProcDataCollectorUri)]
    public class CoverletInProcDataCollectorRunSettingsProvider : RunSettingsProvider
    {
        public override XmlElement Process(TestRunCriteria testRunCriteria, XmlElement configurationElement)
        {
            // TODO: Can we update configurationelement?
            // configurationElement.AppendChild()
            return null;
        }
    }
}
