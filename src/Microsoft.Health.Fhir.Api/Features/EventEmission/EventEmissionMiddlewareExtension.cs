// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using Microsoft.AspNetCore.Builder;

namespace Microsoft.Health.Fhir.Api.Features.EventEmission
{
    public static class EventEmissionMiddlewareExtension
    {
        public static IApplicationBuilder UseEventEmission(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<EventEmissionMiddleware>();
        }
    }
}
