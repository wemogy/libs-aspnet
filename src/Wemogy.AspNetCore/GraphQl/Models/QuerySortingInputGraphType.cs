using GraphQL.Types;
using Wemogy.AspNetCore.GraphQl.Enums;
using Wemogy.DataAccess.Query.Enums;
using Wemogy.DataAccess.Query.Models;

namespace Wemogy.AspNetCore.GraphQl.Models
{
    public class QuerySortingInputGraphType: InputObjectGraphType<QuerySorting>
    {
        public QuerySortingInputGraphType()
        {
            this.Field(o => o.OrderBy);
            this.Field(o => o.SearchAfter, nullable: true);
            this.Field<SortOrderGraphType>(nameof(QuerySorting.SortOrder)).DefaultValue = SortOrder.Ascending;;
        }
    }
}
