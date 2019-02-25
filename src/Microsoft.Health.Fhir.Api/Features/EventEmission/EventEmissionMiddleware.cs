// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Health.Fhir.Core.Configs;

namespace Microsoft.Health.Fhir.Api.Features.EventEmission
{
    public class EventEmissionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<EventEmissionMiddleware> _logger;
        private readonly EventEmissionConfiguration _eventEmissionConfiguration;
        private readonly EventHubClient _eventHubClient;

        public EventEmissionMiddleware(RequestDelegate next, ILogger<EventEmissionMiddleware> logger, IOptions<EventEmissionConfiguration> config)
        {
            EnsureArg.IsNotNull(next, nameof(next));
            EnsureArg.IsNotNull(logger, nameof(logger));

            _next = next;
            _logger = logger;
            _eventEmissionConfiguration = config.Value;

            if (_eventEmissionConfiguration.Enabled)
            {
                var connectionStringBuilder = new EventHubsConnectionStringBuilder(_eventEmissionConfiguration.ConnectionString)
                {
                    EntityPath = _eventEmissionConfiguration.EventHubName,
                };

                _logger.LogInformation($"Registering even emission middleware: {_eventEmissionConfiguration.EventHubName}");

                _eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
            }
            else
            {
                _eventHubClient = null;
            }
        }

        public async Task Invoke(HttpContext context)
        {
            // In this simple example, we will only emit an event when something changes
            if (!_eventEmissionConfiguration.Enabled || (context.Request.Method != "PUT" && context.Request.Method != "POST"))
            {
                await _next(context);
                return;
            }

            // Copy a pointer to the original response body stream
            var originalBodyStream = context.Response.Body;

            // Create a new memory stream...
            using (var responseBody = new MemoryStream())
            {
                // ...and use that for the temporary response body
                context.Response.Body = responseBody;

                // Continue down the Middleware pipeline, eventually returning to this class
                await _next(context);

                // Format the response from the server
                var response = await FormatResponse(context.Response);

                // If we were successful in updating, then emit an event
                if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
                {
                    try
                    {
                        await _eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(response)));
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError($"{DateTime.Now} > Exception: {exception.Message}");
                    }
                }

                // Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
                await responseBody.CopyToAsync(originalBodyStream);
            }

            return;
        }

        private static async Task<string> FormatResponse(HttpResponse response)
        {
            // We need to read the response stream from the beginning...
            response.Body.Seek(0, SeekOrigin.Begin);

            // ...and copy it into a string
            string text = await new StreamReader(response.Body).ReadToEndAsync();

            // We need to reset the reader for the response so that the client can read it.
            response.Body.Seek(0, SeekOrigin.Begin);

            // Return the string for the response, including the status code (e.g. 200, 404, 401, etc.)
            return $"{text}";
        }
    }
}