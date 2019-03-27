// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.Utilities
{
    using System.IO;
    using Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.Utilities.Interfaces;

    /// <inheritdoc />
    internal class FileHelper : IFileHelper
    {
        /// <inheritdoc />
        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        /// <inheritdoc />
        public void WriteAllText(string path, string contents)
        {
            File.WriteAllText(path, contents);
        }

        /// <inheritdoc />
        public string GetTempPath()
        {
            return Path.GetTempPath();
        }

        /// <inheritdoc />
        public string Combine(string path1, string path2)
        {
            return Path.Combine(path1, path2);
        }

        /// <inheritdoc />
        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        /// <inheritdoc />
        public void Delete(string path, bool recursive)
        {
            Directory.Delete(path, recursive);
        }

        /// <inheritdoc />
        public string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }
    }
}
