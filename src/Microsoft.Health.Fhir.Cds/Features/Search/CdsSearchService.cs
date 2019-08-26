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
using Microsoft.Health.Fhir.Cds.Configs;
using Microsoft.Health.Fhir.Cds.Features.Search.Expressions.Visitors.QueryGenerators;
using Microsoft.Health.Fhir.Cds.Features.Storage;
using Microsoft.Health.Fhir.Core.Features.Persistence;
using Microsoft.Health.Fhir.Core.Features.Search;
using Microsoft.Health.Fhir.Core.Models;
using Simple.OData.Client;

namespace Microsoft.Health.Fhir.Cds.Features.Search
{
    public class CdsSearchService : SearchService
    {
        private readonly ILogger<CdsSearchService> _logger;

        private readonly CdsDataStoreConfiguration _config;

        private readonly CdsTokenProvider _tokenProvider;

        private readonly ODataClient _client;

        private readonly ICdsResourceFactory _cdsResourceFactory;

        public CdsSearchService(
            ISearchOptionsFactory searchOptionsFactory,
            IFhirDataStore fhirDataStore,
            IModelInfoProvider modelInfoProvider,
            ICdsResourceFactory cdsResourceFactory,
            CdsDataStoreConfiguration config,
            CdsTokenProvider tokenProvider,
            ILogger<CdsSearchService> logger)
            : base(searchOptionsFactory, fhirDataStore, modelInfoProvider)
        {
            EnsureArg.IsNotNull(cdsResourceFactory, nameof(cdsResourceFactory));
            EnsureArg.IsNotNull(logger, nameof(logger));
            EnsureArg.IsNotNull(fhirDataStore, nameof(fhirDataStore));

            _cdsResourceFactory = cdsResourceFactory;
            _tokenProvider = tokenProvider;
            _config = config;
            _logger = logger;

            var clientConfig = new ODataClientSettings
            {
                BaseUri = _config.Url,
                BeforeRequestAsync = async (httpRequestMessage) =>
                {
                    httpRequestMessage.Headers.Add("Authorization", "Bearer " + await _tokenProvider.GetAccessToken());
                },
            };

            _client = new ODataClient(clientConfig);
        }

        protected override async Task<SearchResult> SearchInternalAsync(SearchOptions searchOptions, CancellationToken cancellationToken)
        {
            var results = new List<ResourceWrapper>();

            var sb = new StringBuilder();
            var queryGenerator = new ODataQueryGenerator(sb);
            searchOptions.Expression.AcceptVisitor(queryGenerator, searchOptions);

            var packages = await _client.FindEntriesAsync(sb.ToString());
            foreach (var package in packages)
            {
                var resource = _cdsResourceFactory.CreateRawResource(queryGenerator.FhirResourceName, package);
                var versionString = package["versionnumber"].ToString();
                var modifiedOnString = package["modifiedon"].ToString();

                results.Add(new ResourceWrapper(
                    _cdsResourceFactory.GetResourceId(queryGenerator.FhirResourceName, package),
                    versionString,
                    queryGenerator.FhirResourceName,
                    resource,
                    new ResourceRequest("GET"),
                    new DateTimeOffset(DateTime.Parse(modifiedOnString)),
                    false, // isDeleted
                    null,
                    null,
                    null));
            }

            return new SearchResult(results, searchOptions.UnsupportedSearchParams, searchOptions.ContinuationToken);
        }

        protected override Task<SearchResult> SearchHistoryInternalAsync(SearchOptions searchOptions, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
