// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Text;
using EnsureThat;
using Microsoft.Health.Fhir.Core.Features.Search;
using Microsoft.Health.Fhir.Core.Features.Search.Expressions;

namespace Microsoft.Health.Fhir.Cds.Features.Search.Expressions.Visitors.QueryGenerators
{
    internal class ODataQueryGenerator : IExpressionVisitor<SearchOptions, object>
    {
        public ODataQueryGenerator(StringBuilder sb)
        {
            EnsureArg.IsNotNull(sb, nameof(sb));
            StringBuilder = sb;
        }

        public StringBuilder StringBuilder { get; }

        public string FhirResourceName { get; private set; }

        public string ODataEntityName { get; private set; }

        public object VisitBinary(BinaryExpression expression, SearchOptions context)
        {
            throw new NotImplementedException();
        }

        public object VisitChained(ChainedExpression expression, SearchOptions context)
        {
            throw new NotImplementedException();
        }

        public object VisitCompartment(CompartmentSearchExpression expression, SearchOptions context)
        {
            throw new NotImplementedException();
        }

        public object VisitMissingField(MissingFieldExpression expression, SearchOptions context)
        {
            throw new NotImplementedException();
        }

        public object VisitMissingSearchParameter(MissingSearchParameterExpression expression, SearchOptions context)
        {
            throw new NotImplementedException();
        }

        public object VisitMultiary(MultiaryExpression expression, SearchOptions context)
        {
            if (expression.MultiaryOperation == MultiaryOperator.Or)
            {
                throw new NotImplementedException();
            }

            foreach (var e in expression.Expressions)
            {
                e.AcceptVisitor(this, context);
            }

            return expression;
        }

        public object VisitSearchParameter(SearchParameterExpression expression, SearchOptions context)
        {
            if (expression.Parameter.Name == "_type")
            {
                if (StringBuilder.Length != 0)
                {
                    throw new Exception("String builder should be empty when encountering _type");
                }

                FhirResourceName = expression.Expression.AcceptVisitor(this, context).ToString();
                switch (FhirResourceName)
                {
                    case "Patient":
                        ODataEntityName = "contacts";
                        StringBuilder.Append("contacts?$filter=(msemr_contacttype eq 935000000)");
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                switch (expression.Parameter.Name)
                {
                    case "family":
                        StringBuilder.Append($" and startswith(lastname, '{expression.Expression.AcceptVisitor(this, context)}')");
                        break;
                    case "given":
                        StringBuilder.Append($" and startswith(firstname, '{expression.Expression.AcceptVisitor(this, context)}')");
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            return expression;
        }

        public object VisitString(StringExpression expression, SearchOptions context)
        {
            return expression.Value;
        }
    }
}
