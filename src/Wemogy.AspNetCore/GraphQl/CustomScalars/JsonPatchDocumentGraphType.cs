using GraphQL.Language.AST;
using GraphQL.Types;
using Wemogy.Core.Extensions;

namespace Wemogy.AspNetCore.GraphQl.CustomScalars
{
    public class JsonPatchDocumentGraphType : ScalarGraphType
    {
        public override object Serialize(object value)
        {
            throw new System.NotImplementedException();
        }

        public override object ParseValue(object value)
        {
            throw new System.NotImplementedException();
        }

        public override object ParseLiteral(IValue value)
        {
            return value.Value.ToJson();
        }
    }
}
