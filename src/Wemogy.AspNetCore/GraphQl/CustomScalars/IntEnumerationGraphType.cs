using System;
using GraphQL.Language.AST;
using GraphQL.Types;

namespace Wemogy.AspNetCore.GraphQl.CustomScalars
{
    public class IntEnumerationGraphType<T> : ScalarGraphType where T : struct
    {
        public override object Serialize(object value)
        {
            Type genericType = typeof(T);
            if (!genericType.IsEnum)
            {
                return 0;
            }

            var valueAsString = value.ToString();
            Enum actValue = Enum.Parse(typeof(T), valueAsString) as Enum;
            return Convert.ToInt32(actValue);
        }

        public override object ParseValue(object value)
        {
            Enum.TryParse<T>(value?.ToString(), out T result);
            return result;
        }

        public override object ParseLiteral(IValue value)
        {
            Enum.TryParse<T>(value.Value?.ToString(), out T result);
            return result;
        }
    }
}
