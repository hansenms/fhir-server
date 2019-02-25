// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

namespace Microsoft.Health.Fhir.Api.Features.EventEmission
{
    public class EventEmissionConfiguration
    {
        public bool Enabled { get; set; }

        public string ConnectionString { get; set; }

        public string EventHubName { get; set; }
    }
}