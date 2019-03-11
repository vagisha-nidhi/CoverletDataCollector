// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.TestPlatform.Extensions.CoverletCoverageDataCollector
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Coverlet.Core;

    internal class CoverageManager : IDisposable
    {
        private readonly Coverage coverage;
        private readonly ManualResetEvent instrumentationCompletionEvent = new ManualResetEvent(true);

        public CoverageManager(CoverletSettings settings)
        {
            this.coverage = new Coverage(settings.TestModules[0], settings.IncludeFilters, settings.IncludeDirectories, settings.ExcludeFilters, settings.ExcludeSourceFiles, settings.ExcludeAttributes, settings.MergeWith, settings.UseSourceLink);
        }

        public void Dispose()
        {
            // Dispose wait handles
            this.instrumentationCompletionEvent.Dispose();
        }

        public async void StartInstrumentationAsync()
        {
            await Task.Run(() => this.StartInstrumentation());
        }

        public void WaitForInstrumentationComplete()
        {
            this.instrumentationCompletionEvent.WaitOne();
        }

        public CoverageResult GetCoverageResult()
        {
            return this.coverage.GetCoverageResult();
        }

        private void StartInstrumentation()
        {
            try
            {
                // Instrument modules
                this.coverage.PrepareModules(); // TODO: Log error in case of exception and restore the original modules. Skip coverage collection or error out after throwing error.
            }
            finally
            {
                // Set instrument compeltion wait handle.
                this.instrumentationCompletionEvent.Set();
            }
        }
    }
}
