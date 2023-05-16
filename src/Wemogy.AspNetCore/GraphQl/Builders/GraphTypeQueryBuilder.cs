using System;
using System.Diagnostics;
using GraphQL.Types;
using Wemogy.AspNetCore.GraphQl.Extensions;
using Wemogy.AspNetCore.GraphQl.Models;
using Wemogy.DataAccess.Interfaces;

namespace Wemogy.AspNetCore.GraphQl.Builders
{
    public static class GraphTypeQueryBuilder
    {
        public static void
            GraphTypeQueryGetSingleAndList<TQueryType, TGraphType, TModelType, TDatabaseService, TContext, TPartitionKey>(
                this TQueryType queryType, bool allowAnonymous = false) where TQueryType : ObjectGraphType
            where TDatabaseService : IDatabaseService<TModelType, TPartitionKey, TContext>
            where TGraphType : IGraphType
            where TModelType : class, IModel<Guid>
            where TContext: IContext
            where TPartitionKey: IEquatable<TPartitionKey>
        {
            queryType.GraphTypeQueryGetSingle<TQueryType, TGraphType, TModelType, TDatabaseService, TContext, TPartitionKey>(allowAnonymous);
            queryType.GraphTypeQueryGetList<TQueryType, TGraphType, TModelType, TDatabaseService, TContext, TPartitionKey>(allowAnonymous);
        }

        public static void
            GraphTypeQueryGetSingle<TQueryType, TGraphType, TModelType, TDatabaseService, TContext, TPartitionKey>(this TQueryType queryType, bool allowAnonymous = false)
            where TQueryType : ObjectGraphType
            where TGraphType : IGraphType
            where TModelType : class, IModel<Guid>
            where TDatabaseService : IDatabaseService<TModelType, TPartitionKey, TContext>
            where TContext: IContext
            where TPartitionKey: IEquatable<TPartitionKey>
        {
            var name = typeof(TGraphType).GraphTypeName();
            queryType.FieldAsync<TGraphType>(
                name: name,
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>() {Name = "id"}
                ),
                resolve: async context =>
                {
                    var duration = Stopwatch.StartNew();
                    if(!allowAnonymous){
                        var requestIsAuthenticated = context.RequestIsAuthenticated();

                        if (!requestIsAuthenticated)
                        {
                            throw new UnauthorizedAccessException();
                        }
                    }

                    var id = context.GetIdArgument();
                    var databaseService = context.GetService<TDatabaseService>();

                    var result = (await databaseService.GetAsync(id, context.CancellationToken));

                    Console.WriteLine($"GET {name} duration: {duration.ElapsedMilliseconds}");

                    return result;
                }
            );
        }

        public static void
            GraphTypeQueryGetList<TQueryType, TGraphType, TModelType, TDatabaseService, TContext, TPartitionKey>(this TQueryType queryType, bool allowAnonymous = false)
            where TQueryType : ObjectGraphType
            where TGraphType : IGraphType
            where TModelType : class, IModel<Guid>
            where TDatabaseService : IDatabaseService<TModelType, TPartitionKey, TContext>
            where TContext: IContext
            where TPartitionKey: IEquatable<TPartitionKey>
        {

            var name = typeof(TGraphType).GraphTypeName() + "s";
            queryType.FieldAsync<ListGraphType<TGraphType>>(
                name: name,
                arguments: new QueryArguments(
                    new QueryArgument<QueryParameterInputGraphType>() {Name = "query"}
                ),
                resolve: async context =>
                {
                    var duration = Stopwatch.StartNew();
                    if(!allowAnonymous){
                        var requestIsAuthenticated = context.RequestIsAuthenticated();

                        if (!requestIsAuthenticated)
                        {
                            throw new UnauthorizedAccessException();
                        }
                    }

                    var queryParameters = context.GetQueryParametersArgument();
                    var databaseService = context.GetService<TDatabaseService>();

                    var result =  (await databaseService.QueryAsync(queryParameters, context.CancellationToken));

                    Console.WriteLine($"QUERY {name} duration: {duration.ElapsedMilliseconds}");
                    return result;
                }
            );
        }
    }
}
