// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.Core.Features.Persistence;
using Microsoft.Health.Fhir.Core.Features.Search;
using Microsoft.Health.Fhir.Core.Models;

namespace Microsoft.Health.Fhir.CustomProvider.Features.Search
{
    public class CustomProviderSearchService : SearchService
    {
        private readonly ILogger<CustomProviderSearchService> _logger;

        public CustomProviderSearchService(
            ISearchOptionsFactory searchOptionsFactory,
            IFhirDataStore fhirDataStore,
            IModelInfoProvider modelInfoProvider,
            ILogger<CustomProviderSearchService> logger)
            : base(searchOptionsFactory, fhirDataStore, modelInfoProvider)
        {
            EnsureArg.IsNotNull(logger, nameof(logger));

            _logger = logger;
        }

        protected override Task<SearchResult> SearchInternalAsync(SearchOptions searchOptions, CancellationToken cancellationToken)
        {
            return SearchImpl(searchOptions, false, cancellationToken);
        }

        protected override Task<SearchResult> SearchHistoryInternalAsync(SearchOptions searchOptions, CancellationToken cancellationToken)
        {
            return SearchImpl(searchOptions, true, cancellationToken);
        }

        private Task<SearchResult> SearchImpl(SearchOptions searchOptions, bool historySearch, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
