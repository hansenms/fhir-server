// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EnsureThat;
using Newtonsoft.Json.Linq;

namespace Microsoft.Health.Fhir.CustomProvider.Features.Storage
{
    public static class ODataRawResourceFactory
    {
        public static string CreateRawResource(string resourceType, IDictionary<string, object> odata)
        {
            EnsureArg.IsNotNullOrEmpty(resourceType, nameof(resourceType));
            EnsureArg.IsNotNull(odata, nameof(odata));

            switch (resourceType)
            {
                case "Patient":
                    dynamic resource = new JObject();
                    resource.resourceType = "Patient";
                    var name = new JObject();
                    name["family"] = odata["lastname"].ToString();
                    name["given"] = new JArray(odata["firstname"]);
                    resource.name = new JArray(name);
                    return resource.ToString();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}