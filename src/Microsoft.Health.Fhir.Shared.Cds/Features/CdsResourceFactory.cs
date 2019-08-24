// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EnsureThat;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.Core.Features.Persistence;
using Microsoft.Health.Fhir.Core.Models;

namespace Microsoft.Health.Fhir.Cds.Features
{
    public class CdsResourceFactory : ICdsResourceFactory
    {
        private readonly IRawResourceFactory _rawResourceFactory;

        private readonly ILogger<CdsResourceFactory> _logger;

        public CdsResourceFactory(
            IRawResourceFactory resourceFactory,
            ILogger<CdsResourceFactory> logger)
        {
            EnsureArg.IsNotNull(resourceFactory);
            EnsureArg.IsNotNull(logger);
            _rawResourceFactory = resourceFactory;
            _logger = logger;
        }

        public RawResource CreateRawResource(string resourceType, IDictionary<string, object> odata)
        {
            EnsureArg.IsNotNullOrEmpty(resourceType, nameof(resourceType));
            EnsureArg.IsNotNull(odata, nameof(odata));

            var resourceId = GetResourceId(resourceType, odata);

            switch (resourceType)
            {
                case "Patient":
                    var patient = new Patient();
                    patient.Id = resourceId;
                    var humanName = new HumanName();
                    humanName.Family = odata["lastname"].ToString();
                    humanName.Given = new List<string>
                    {
                        odata["firstname"].ToString(),
                    };
                    patient.Name = new List<HumanName>
                    {
                        humanName,
                    };
                    patient.BirthDate = odata["birthdate"].ToString();
                    return _rawResourceFactory.Create(new ResourceElement(patient.ToTypedElement()));
                default:
                    _logger.LogCritical("Attempting to creating an unknown resource type");
                    throw new NotImplementedException();
            }
        }

        public string GetResourceId(string resourceType, IDictionary<string, object> odata)
        {
            switch (resourceType)
            {
                case "Patient":
                    return odata["contactid"].ToString();
                default:
                    _logger.LogCritical("Attempting to get id of an unknown resource type");
                    throw new NotImplementedException();
            }
        }
    }
}