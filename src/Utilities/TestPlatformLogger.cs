// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.Utilities
{
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;

    internal class TestPlatformLogger
    {
        private readonly DataCollectionLogger logger;
        private readonly DataCollectionContext dataCollectionContext;

        public TestPlatformLogger(DataCollectionLogger logger, DataCollectionContext dataCollectionContext)
        {
            this.logger = logger;
            this.dataCollectionContext = dataCollectionContext;
        }

        public void LogWarning(string warning)
        {
            this.logger.LogWarning(this.dataCollectionContext, warning);
        }
    }
}