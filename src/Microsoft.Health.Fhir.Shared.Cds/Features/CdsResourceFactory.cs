// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EnsureThat;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Microsoft.Health.Fhir.Cds.Features
{
    public class CdsResourceFactory : ICdsResourceFactory
    {
        private readonly ILogger<CdsResourceFactory> _logger;

        public CdsResourceFactory(ILogger<CdsResourceFactory> logger)
        {
            EnsureArg.IsNotNull(logger);
            _logger = logger;
        }

        public dynamic CreateRawResource(string resourceType, IDictionary<string, object> odata)
        {
            EnsureArg.IsNotNullOrEmpty(resourceType, nameof(resourceType));
            EnsureArg.IsNotNull(odata, nameof(odata));

            switch (resourceType)
            {
                case "Patient":
                    dynamic resource = new JObject();
                    resource.resourceType = "Patient";
                    resource.id = odata["contactid"].ToString();
                    var name = new JObject();
                    name["family"] = odata["lastname"].ToString();
                    name["given"] = new JArray(odata["firstname"].ToString());
                    resource.name = new JArray(name);
                    resource["birthDate"] = odata["birthdate"].ToString();
                    return resource;
                default:
                    _logger.LogCritical("Attempting to creating an unknown resource type");
                    throw new NotImplementedException();
            }
        }
    }
}