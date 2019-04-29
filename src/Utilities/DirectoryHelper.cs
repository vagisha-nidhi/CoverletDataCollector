// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.Utilities
{
    using System.IO;
    using Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.Utilities.Interfaces;

    /// <inheritdoc />
    internal class DirectoryHelper : IDirectoryHelper
    {
        /// <inheritdoc />
        public bool Exists(string path)
        {
            return Directory.Exists(path);
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
    }
}
