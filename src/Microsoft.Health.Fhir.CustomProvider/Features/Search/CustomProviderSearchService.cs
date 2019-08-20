// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.Core.Features.Persistence;
using Microsoft.Health.Fhir.Core.Features.Search;
using Microsoft.Health.Fhir.Core.Models;
using Microsoft.Health.Fhir.CustomProvider.Configs;

namespace Microsoft.Health.Fhir.CustomProvider.Features.Search
{
    public class CustomProviderSearchService : SearchService
    {
        private readonly ILogger<CustomProviderSearchService> _logger;

        private readonly CustomProviderDataStoreConfiguration _config;

        public CustomProviderSearchService(
            ISearchOptionsFactory searchOptionsFactory,
            IFhirDataStore fhirDataStore,
            IModelInfoProvider modelInfoProvider,
            CustomProviderDataStoreConfiguration config,
            ILogger<CustomProviderSearchService> logger)
            : base(searchOptionsFactory, fhirDataStore, modelInfoProvider)
        {
            EnsureArg.IsNotNull(logger, nameof(logger));

            _config = config;
            _logger = logger;
        }

        protected override Task<SearchResult> SearchInternalAsync(SearchOptions searchOptions, CancellationToken cancellationToken)
        {
            var results = new List<ResourceWrapper>();
            _logger.LogInformation(searchOptions.Expression.ToString());
            _logger.LogInformation($"ClientId: {_config.ClientId}");
            return Task.FromResult(new SearchResult(results, searchOptions.UnsupportedSearchParams, searchOptions.ContinuationToken));
        }

        protected override Task<SearchResult> SearchHistoryInternalAsync(SearchOptions searchOptions, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
