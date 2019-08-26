// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using EnsureThat;
using Microsoft.Extensions.Configuration;
using Microsoft.Health.Extensions.DependencyInjection;
using Microsoft.Health.Fhir.Cds.Configs;
using Microsoft.Health.Fhir.Cds.Features.Health;
using Microsoft.Health.Fhir.Cds.Features.Search;
using Microsoft.Health.Fhir.Cds.Features.Storage;
using Microsoft.Health.Fhir.Core.Registration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FhirServerBuilderCdsRegistrationExtensions
    {
        public static IFhirServerBuilder AddExperimentalCdsProvider(this IFhirServerBuilder fhirServerBuilder, Action<CdsDataStoreConfiguration> configureAction = null)
        {
            EnsureArg.IsNotNull(fhirServerBuilder, nameof(fhirServerBuilder));
            IServiceCollection services = fhirServerBuilder.Services;

            services.Add(provider =>
                {
                    var config = new CdsDataStoreConfiguration();
                    provider.GetService<IConfiguration>().GetSection("Cds").Bind(config);
                    configureAction?.Invoke(config);

                    return config;
                })
                .Singleton()
                .AsSelf();

            services.Add<CdsTokenProvider>()
                .Scoped()
                .AsSelf();

            services.Add<CdsFhirDataStore>()
                .Scoped()
                .AsSelf()
                .AsImplementedInterfaces();

            services
                .AddHealthChecks()
                .AddCheck<CdsHealthCheck>(nameof(CdsHealthCheck));

            services.Add<CdsSearchService>()
                .Scoped()
                .AsSelf()
                .AsImplementedInterfaces();

            return fhirServerBuilder;
        }
    }
}
