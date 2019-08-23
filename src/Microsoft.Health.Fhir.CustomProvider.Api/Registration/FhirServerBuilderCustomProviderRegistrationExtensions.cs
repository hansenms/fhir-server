// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using EnsureThat;
using Microsoft.Extensions.Configuration;
using Microsoft.Health.Extensions.DependencyInjection;
using Microsoft.Health.Fhir.Core.Registration;
using Microsoft.Health.Fhir.CustomProvider.Configs;
using Microsoft.Health.Fhir.CustomProvider.Features.Health;
using Microsoft.Health.Fhir.CustomProvider.Features.Search;
using Microsoft.Health.Fhir.CustomProvider.Features.Storage;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FhirServerBuilderCustomProviderRegistrationExtensions
    {
        public static IFhirServerBuilder AddExperimentalCustomProvider(this IFhirServerBuilder fhirServerBuilder, Action<CustomProviderDataStoreConfiguration> configureAction = null)
        {
            EnsureArg.IsNotNull(fhirServerBuilder, nameof(fhirServerBuilder));
            IServiceCollection services = fhirServerBuilder.Services;

            services.Add(provider =>
                {
                    var config = new CustomProviderDataStoreConfiguration();
                    provider.GetService<IConfiguration>().GetSection("CustomProvider").Bind(config);
                    configureAction?.Invoke(config);

                    return config;
                })
                .Singleton()
                .AsSelf();

            services.Add<CustomProviderTokenProvider>()
                .Scoped()
                .AsSelf();

            services.Add<CustomProviderFhirDataStore>()
                .Scoped()
                .AsSelf()
                .AsImplementedInterfaces();

            services
                .AddHealthChecks()
                .AddCheck<CustomProviderHealthCheck>(nameof(CustomProviderHealthCheck));

            services.Add<CustomProviderSearchService>()
                .Scoped()
                .AsSelf()
                .AsImplementedInterfaces();

            return fhirServerBuilder;
        }
    }
}
