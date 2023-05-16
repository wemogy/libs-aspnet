using GraphQL.Types;
using Wemogy.DataAccess.Query.Models;

namespace Wemogy.AspNetCore.GraphQl.Models
{
    public class QueryParameterInputGraphType : InputObjectGraphType<QueryParameters>
    {
        public QueryParameterInputGraphType()
        {
            this.Field(o => o.Take, true);
            this.Field<ListGraphType<QueryFilterInputGraphType>>(nameof(QueryParameters.Filters));
            this.Field<ListGraphType<QuerySortingInputGraphType>>(nameof(QueryParameters.Sortings));
        }
    }
}
