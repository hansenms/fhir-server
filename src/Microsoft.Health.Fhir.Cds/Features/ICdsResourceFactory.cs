// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.Health.Fhir.Cds.Features
{
    public interface ICdsResourceFactory
    {
        dynamic CreateRawResource(string resourceType, IDictionary<string, object> odata);
    }
}