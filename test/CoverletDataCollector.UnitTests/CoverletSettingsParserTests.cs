// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace CoverletCoverageDataCollector.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.DataCollector;
    using Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.Utilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CoverletSettingsParserTests
    {
        private CoverletSettingsParser coverletSettingsParser;

        public CoverletSettingsParserTests()
        {
            this.coverletSettingsParser = new CoverletSettingsParser(new TestPlatformEqtTrace());
        }

        [TestMethod]
        public void ParseShouldThrowCoverletDataCollectorExceptionIfTestModulesIsNull()
        {
            var message = Assert.ThrowsException<CoverletDataCollectorException>(() => this.coverletSettingsParser.Parse(null, null)).Message;

            Assert.AreEqual(message, "CoverletCoverageDataCollector: No test modules found");
        }

        [TestMethod]
        public void ParseShouldThrowCoverletDataCollectorExceptionIfTestModulesIsEmpty()
        {
            var message = Assert.ThrowsException<CoverletDataCollectorException>(() => this.coverletSettingsParser.Parse(null, Enumerable.Empty<string>())).Message;

            Assert.AreEqual(message, "CoverletCoverageDataCollector: No test modules found");
        }

        [TestMethod]
        public void ParseShouldSelectFirstTestModuleFromTestModulesList()
        {
            var testModules = new List<string> { "module1.dll", "module2.dll", "module3.dll" };

            var coverletSettings = this.coverletSettingsParser.Parse(null, testModules);

            Assert.AreEqual(coverletSettings.TestModule, "module1.dll");
        }

        [TestMethod]
        public void ParseShouldCorrectlyParseConfigurationElement()
        {
            var testModules = new List<string> { "abc.dll" };
            var doc = new XmlDocument();
            var configElement = doc.CreateElement("Configuration");
            this.CreateCoverletNodes(doc, configElement, CoverletConstants.IncludeFiltersElementName, "[*]*");
            this.CreateCoverletNodes(doc, configElement, CoverletConstants.ExcludeFiltersElementName, "[coverlet.*.tests?]*");
            this.CreateCoverletNodes(doc, configElement, CoverletConstants.IncludeDirectoriesElementName, @"E:\temp");
            this.CreateCoverletNodes(doc, configElement, CoverletConstants.ExcludeSourceFilesElementName, "module1.cs,module2.cs");
            this.CreateCoverletNodes(doc, configElement, CoverletConstants.ExcludeAttributesElementName, "Obsolete,GeneratedCodeAttribute,CompilerGeneratedAttribute");
            this.CreateCoverletNodes(doc, configElement, CoverletConstants.MergeWithElementName, "/path/to/result.json");
            this.CreateCoverletNodes(doc, configElement, CoverletConstants.UseSourceLinkElementName, "false");
            this.CreateCoverletNodes(doc, configElement, CoverletConstants.SingleHitElementName, "true");

            var coverletSettings = this.coverletSettingsParser.Parse(configElement, testModules);

            Assert.AreEqual(coverletSettings.TestModule, "abc.dll");
            Assert.AreEqual(coverletSettings.IncludeFilters[0], "[*]*");
            Assert.AreEqual(coverletSettings.IncludeDirectories[0], @"E:\temp");
            Assert.AreEqual(coverletSettings.ExcludeSourceFiles[0], "module1.cs");
            Assert.AreEqual(coverletSettings.ExcludeSourceFiles[1], "module2.cs");
            Assert.AreEqual(coverletSettings.ExcludeAttributes[0], "Obsolete");
            Assert.AreEqual(coverletSettings.ExcludeAttributes[1], "GeneratedCodeAttribute");
            Assert.AreEqual(coverletSettings.MergeWith, "/path/to/result.json");
            Assert.IsFalse(coverletSettings.UseSourceLink);
            Assert.IsTrue(coverletSettings.SingleHit);
        }

        private void CreateCoverletNodes(XmlDocument doc,XmlElement configElement, string nodeSetting, string nodeValue)
        {
            var node = doc.CreateNode("element", nodeSetting, string.Empty);
            node.InnerText = nodeValue;
            configElement.AppendChild(node);
        }
    }
}
