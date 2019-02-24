// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.Core.Configs;

namespace Microsoft.Health.Fhir.Api.Features.EventEmission
{
    public class EventEmissionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<EventEmissionMiddleware> _logger;

        public EventEmissionMiddleware(RequestDelegate next, ILogger<EventEmissionMiddleware> logger)
        {
            EnsureArg.IsNotNull(next, nameof(next));
            EnsureArg.IsNotNull(logger, nameof(logger));

            _next = next;
            _logger = logger;

            _logger.LogInformation("Registering even emission middleware");
        }

        public async Task Invoke(HttpContext context)
        {
            _logger.LogInformation("In event emission middleware");

            // Call the next delegate/middleware in the pipeline
            await _next(context);

            var verb = context.Request.Method.ToString();

            _logger.LogInformation($"Done with event emission {verb}.");

            return;
        }
    }
}