// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.Cds.Configs;
using Microsoft.Health.Fhir.Core.Features.Conformance;
using Microsoft.Health.Fhir.Core.Features.Persistence;
using Microsoft.Health.Fhir.Core.Models;
using Microsoft.Health.Fhir.ValueSets;

namespace Microsoft.Health.Fhir.Cds.Features.Storage
{
    public class CdsFhirDataStore : IFhirDataStore, IProvideCapability
    {
        private readonly ILogger<CdsFhirDataStore> _logger;
        private readonly CdsDataStoreConfiguration _config;

        private readonly CdsTokenProvider _tokenProvider;

        public CdsFhirDataStore(
            ILogger<CdsFhirDataStore> logger,
            CdsDataStoreConfiguration config,
            CdsTokenProvider tokenProvider)
        {
            _logger = logger;
            _config = config;
            _tokenProvider = tokenProvider;
        }

        public Task<UpsertOutcome> UpsertAsync(ResourceWrapper resource, WeakETag weakETag, bool allowCreate, bool keepHistory, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ResourceWrapper> GetAsync(ResourceKey key, CancellationToken cancellationToken)
        {
            _logger.LogError("Read not implemented yet");
            throw new NotImplementedException();
        }

        public Task HardDeleteAsync(ResourceKey key, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Build(IListedCapabilityStatement statement)
        {
            EnsureArg.IsNotNull(statement, nameof(statement));

            foreach (var resource in ModelInfoProvider.GetResourceTypeNames())
            {
                statement.BuildRestResourceComponent(resource, builder =>
                {
                    builder.AddResourceVersionPolicy(ResourceVersionPolicy.NoVersion);
                    builder.AddResourceVersionPolicy(ResourceVersionPolicy.Versioned);
                    builder.AddResourceVersionPolicy(ResourceVersionPolicy.VersionedUpdate);
                    builder.ReadHistory = true;
                    builder.UpdateCreate = true;
                });
            }
        }
    }
}
