// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector.Utilities
{
    using System;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;

    internal class CoverletLogger
    {
        private readonly DataCollectionLogger logger;
        private readonly DataCollectionContext dataCollectionContext;

        public CoverletLogger(DataCollectionLogger logger, DataCollectionContext dataCollectionContext)
        {
            this.logger = logger;
            this.dataCollectionContext = dataCollectionContext;
        }

        public void LogError(Exception exception)
        {
            this.logger.LogError(this.dataCollectionContext, exception);
        }
    }
}