using System;
using GraphQL;
using Wemogy.AspNetCore.GraphQl.Models;
using Wemogy.DataAccess.Query.Models;

namespace Wemogy.AspNetCore.GraphQl.Extensions
{
    public static class ResolveFieldContextExtensions
    {
        public static string GetArgument<T>(this ResolveFieldContext<T> resolveFieldContext, string argumentName)
        {
            if (!resolveFieldContext.HasArgument(argumentName))
            {
                throw new Exception($"{argumentName} Argument missing");
            }

            return resolveFieldContext.GetArgument<string>(argumentName);
        }

        public static Guid GetIdArgument(this IResolveFieldContext resolveFieldContext)
        {
            if (!resolveFieldContext.HasArgument("id"))
            {
                throw new Exception("ID Argument missing");
            }

            string id = resolveFieldContext.GetArgument<string>("id");
            return Guid.Parse(id);
        }

        public static TId GetIdArgument<TId>(this IResolveFieldContext resolveFieldContext)
        {
            if (!resolveFieldContext.HasArgument("id"))
            {
                throw new Exception("ID Argument missing");
            }

            string id = resolveFieldContext.GetArgument<string>("id");

            var tIdType = typeof(TId);
            if (tIdType == typeof(Guid))
            {
                dynamic idGuid = Guid.Parse(id);
                return idGuid;
            }
            if (tIdType == typeof(string))
            {
                dynamic idString = id;
                return idString;
            }

            throw new Exception($"GetIdArgument<TId> is currently not supported for type {tIdType.FullName}");
        }

        public static QueryParameters GetQueryParametersArgument<T>(this IResolveFieldContext<T> resolveFieldContext)
        {
            if (!resolveFieldContext.HasArgument("query"))
            {
                return null;
            }

            return resolveFieldContext.GetArgument<QueryParameters>("query");
        }

        public static T GetService<T>(this IResolveFieldContext resolveFieldContext)
        {
            return (T) resolveFieldContext.RequestServices.GetService(typeof(T));
        }

        public static bool RequestIsAuthenticated(this IResolveFieldContext resolveFieldContext)
        {
            return resolveFieldContext.GetGraphQlUserContext().UserIsAuthenticated;
        }

        public static GraphQlUserContext GetGraphQlUserContext(this IResolveFieldContext resolveFieldContext)
        {
            return (GraphQlUserContext) resolveFieldContext.UserContext;
        }
    }
}
