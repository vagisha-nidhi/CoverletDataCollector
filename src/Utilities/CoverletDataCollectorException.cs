// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.Utilities
{
    using System;

    [Serializable]
    internal class CoverletDataCollectorException : Exception
    {
        public CoverletDataCollectorException(string message) : base(message)
        {
        }

        public CoverletDataCollectorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}