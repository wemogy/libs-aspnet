using System;
using GraphQL.Language.AST;
using GraphQL.Types;
using Wemogy.Core.Extensions;

namespace Wemogy.AspNetCore.GraphQl.CustomScalars
{
    public class TimestampGraphType: ScalarGraphType
    {
        public override object Serialize(object value)
        {
            if (!(value is DateTime valueAsDateTime))
            {
                return null;
            }

            return valueAsDateTime.ToUnixEpochDate();
        }

        public override object ParseValue(object value)
        {
            if (!(value is long valueAsLong))
            {
                return default(DateTime);
            }

            return valueAsLong.FromUnixEpochDate();
        }

        public override object ParseLiteral(IValue value)
        {
            return this.ParseValue(value);
        }
    }
}
