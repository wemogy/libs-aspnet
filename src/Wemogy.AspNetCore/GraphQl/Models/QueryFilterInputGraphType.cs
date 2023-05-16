using GraphQL.Types;
using Wemogy.AspNetCore.GraphQl.Enums;
using Wemogy.DataAccess.Query.Models;

namespace Wemogy.AspNetCore.GraphQl.Models
{
    public class QueryFilterInputGraphType : InputObjectGraphType<QueryFilter>
    {
        public QueryFilterInputGraphType()
        {
            this.Field(o => o.Property);
            this.Field(o => o.Value);
            this.Field<ComparatorGraphType>(nameof(QueryFilter.Comparator));
            this.Field(o => o.ExpressionTreeNodeId);
        }
    }
}
