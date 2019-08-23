// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.Core.Features.Persistence;
using Microsoft.Health.Fhir.Core.Features.Search;
using Microsoft.Health.Fhir.Core.Models;
using Microsoft.Health.Fhir.CustomProvider.Configs;
using Microsoft.Health.Fhir.CustomProvider.Features.Search.Expressions.Visitors.QueryGenerators;
using Microsoft.Health.Fhir.CustomProvider.Features.Storage;

namespace Microsoft.Health.Fhir.CustomProvider.Features.Search
{
    public class CustomProviderSearchService : SearchService
    {
        private readonly ILogger<CustomProviderSearchService> _logger;

        private readonly CustomProviderDataStoreConfiguration _config;

        private readonly CustomProviderTokenProvider _tokenProvider;

        public CustomProviderSearchService(
            ISearchOptionsFactory searchOptionsFactory,
            IFhirDataStore fhirDataStore,
            IModelInfoProvider modelInfoProvider,
            CustomProviderDataStoreConfiguration config,
            CustomProviderTokenProvider tokenProvider,
            ILogger<CustomProviderSearchService> logger)
            : base(searchOptionsFactory, fhirDataStore, modelInfoProvider)
        {
            EnsureArg.IsNotNull(logger, nameof(logger));
            EnsureArg.IsNotNull(fhirDataStore, nameof(fhirDataStore));

            _tokenProvider = tokenProvider;
            _config = config;
            _logger = logger;
        }

        protected override async Task<SearchResult> SearchInternalAsync(SearchOptions searchOptions, CancellationToken cancellationToken)
        {
            var results = new List<ResourceWrapper>();

            var sb = new StringBuilder();
            var queryGenerator = new ODataQueryGenerator(sb);
            searchOptions.Expression.AcceptVisitor(queryGenerator, searchOptions);

            _logger.LogInformation($"searchParameter: {sb.ToString()}");
            _logger.LogInformation($"Token: {await _tokenProvider.GetAccessToken()}");
            return new SearchResult(results, searchOptions.UnsupportedSearchParams, searchOptions.ContinuationToken);
        }

        protected override Task<SearchResult> SearchHistoryInternalAsync(SearchOptions searchOptions, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
