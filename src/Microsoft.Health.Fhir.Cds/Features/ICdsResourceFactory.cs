// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.Health.Fhir.Core.Features.Persistence;

namespace Microsoft.Health.Fhir.Cds.Features
{
    public interface ICdsResourceFactory
    {
        RawResource CreateRawResource(string resourceType, IDictionary<string, object> odata);

        string GetResourceId(string resourceType, IDictionary<string, object> odata);
    }
}