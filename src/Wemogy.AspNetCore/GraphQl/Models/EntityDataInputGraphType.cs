using System;
using GraphQL.Types;
using Wemogy.AspNetCore.GraphQl.CustomScalars;
using Wemogy.DataAccess.Interfaces;

namespace Wemogy.AspNetCore.GraphQl.Models
{
    public class EntityDataInputGraphType<T> : EntityDataInputGraphType<T, Guid> where T : IModel<Guid>
    {

    }

    public class EntityDataInputGraphType<T, TId> : InputObjectGraphType<T> where T : IModel<TId>
    {
        public EntityDataInputGraphType()
        {
            this.Field<NonNullGraphType<StringGraphType>>(nameof(IModel<TId>.Id));
            this.Field<NonNullGraphType<TimestampGraphType>>(nameof(IModel<TId>.CreatedAt));
            this.Field<NonNullGraphType<TimestampGraphType>>(nameof(IModel<TId>.UpdatedAt));
        }
    }
}
