// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.CustomProvider.Configs;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Microsoft.Health.Fhir.CustomProvider.Features.Storage
{
    public class CustomProviderTokenProvider
    {
        private readonly ILogger<CustomProviderTokenProvider> _logger;
        private readonly CustomProviderDataStoreConfiguration _config;
        private readonly AuthenticationContext _authContext;
        private readonly ClientCredential _clientCreds;

        public CustomProviderTokenProvider(
            CustomProviderDataStoreConfiguration config,
            ILogger<CustomProviderTokenProvider> logger)
        {
            EnsureArg.IsNotNull(config, nameof(config));
            EnsureArg.IsNotNull(logger, nameof(logger));

            _config = config;
            _logger = logger;
            _authContext = new AuthenticationContext(_config.Authority.ToString());
            _clientCreds = new ClientCredential(_config.ClientId, _config.ClientSecret);
        }

        public async Task<string> GetAccessToken()
        {
            try
            {
                return (await _authContext.AcquireTokenAsync(_config.Audience, _clientCreds)).AccessToken;
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Unable to acquire access token: " + ex.ToString());
                throw;
            }
        }
    }
}