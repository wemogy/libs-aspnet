using System;
using System.Threading;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using Wemogy.AspNetCore.GraphQl.CustomScalars;
using Wemogy.AspNetCore.GraphQl.Extensions;
using Wemogy.Core.Extensions;
using Wemogy.DataAccess.Interfaces;

namespace Wemogy.AspNetCore.GraphQl.Builders
{
    public static class GraphTypeMutationBuilder
    {
        public static void
            GraphTypeMutationCreatePatchAndDelete<TMutationType, TInputGraphType, TModelType, TDatabaseService, TPartitionKey, TContext>(
                this TMutationType queryType, bool allowAnonymous = false) where TMutationType : ObjectGraphType<object>
            where TInputGraphType : GraphType
            where TModelType : class, IModel<Guid>
            where TDatabaseService : IDatabaseService<TModelType, TPartitionKey, TContext>
            where TPartitionKey: IEquatable<TPartitionKey>
            where TContext: IContext
        {
            queryType.GraphTypeMutationCreate<TMutationType, TInputGraphType, TModelType, TDatabaseService, TPartitionKey, TContext>(allowAnonymous);
            queryType.GraphTypeMutationPatch<TMutationType, TInputGraphType, TModelType, TDatabaseService, TPartitionKey, TContext>(allowAnonymous);
            queryType.GraphTypeMutationDelete<TMutationType, TInputGraphType, TModelType, TDatabaseService, TPartitionKey, TContext>(allowAnonymous);
        }

        public static void
        GraphTypeMutationCreatePatchAndDelete<TMutationType, TInputGraphType, TModelType, TDatabaseService, TGraphType, TPartitionKey, TContext>(
        this TMutationType queryType, bool allowAnonymous = false) where TMutationType : ObjectGraphType<object>
            where TInputGraphType : GraphType
            where TModelType : class, IModel<Guid>
            where TDatabaseService : IDatabaseService<TModelType, TPartitionKey, TContext>
            where TGraphType : GraphType
            where TPartitionKey: IEquatable<TPartitionKey>
            where TContext: IContext
        {
            queryType.GraphTypeMutationCreate<TMutationType, TInputGraphType, TModelType, TDatabaseService, TGraphType, TPartitionKey, TContext>(allowAnonymous);
            queryType.GraphTypeMutationPatch<TMutationType, TInputGraphType, TModelType, TDatabaseService, TPartitionKey, TContext>(allowAnonymous);
            queryType.GraphTypeMutationDelete<TMutationType, TInputGraphType, TModelType, TDatabaseService, TPartitionKey, TContext>(allowAnonymous);
        }

        public static void
            GraphTypeMutationCreate<TMutationType, TInputGraphType, TModelType, TDatabaseService, TPartitionKey, TContext>(
                this TMutationType queryType, bool allowAnonymous = false) where TMutationType : ObjectGraphType<object>
            where TInputGraphType : GraphType
            where TModelType : class, IModel<Guid>
            where TDatabaseService : IDatabaseService<TModelType, TPartitionKey, TContext>
            where TContext: IContext
            where TPartitionKey: IEquatable<TPartitionKey>
        {
            var name = $"create{typeof(TInputGraphType).InputGraphTypeName(true)}";
            var argumentName = typeof(TInputGraphType).InputGraphTypeName();

            queryType.FieldAsync<BooleanGraphType>(
                name: name,
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<TInputGraphType>>() {Name = argumentName}
                ),
                resolve: async context =>
                {
                    if(!allowAnonymous){
                        var requestIsAuthenticated = context.RequestIsAuthenticated();

                        if (!requestIsAuthenticated)
                        {
                            throw new UnauthorizedAccessException();
                        }
                    }

                    var entityToCreate = context.GetArgument<TModelType>(argumentName);
                    var databaseService = context.GetService<TDatabaseService>();

                    await databaseService.CreateAsync(entityToCreate, CancellationToken.None);

                    return true;
                }
            );
        }

        public static void
            GraphTypeMutationCreate<TMutationType, TInputGraphType, TModelType, TDatabaseService, TGraphType, TPartitionKey, TContext>(
                this TMutationType queryType, bool allowAnonymous = false) where TMutationType : ObjectGraphType<object>
            where TInputGraphType : GraphType
            where TModelType : class, IModel<Guid>
            where TDatabaseService : IDatabaseService<TModelType, TPartitionKey, TContext>
            where TGraphType : GraphType
            where TContext: IContext
            where TPartitionKey: IEquatable<TPartitionKey>
        {
            var name = $"create{typeof(TInputGraphType).InputGraphTypeName(true)}";
            var argumentName = typeof(TInputGraphType).InputGraphTypeName();

            queryType.FieldAsync<TGraphType>(
                name: name,
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<TInputGraphType>>() {Name = argumentName}
                ),
                resolve: async context =>
                {
                    if(!allowAnonymous){
                        var requestIsAuthenticated = context.RequestIsAuthenticated();

                        if (!requestIsAuthenticated)
                        {
                            throw new UnauthorizedAccessException();
                        }
                    }

                    var entityToCreate = context.GetArgument<TModelType>(argumentName);
                    var databaseService = context.GetService<TDatabaseService>();

                    await databaseService.CreateAsync(entityToCreate, CancellationToken.None);

                    return entityToCreate;
                }
            );
        }

        public static void
            GraphTypeMutationPatch<TMutationType, TInputGraphType, TModelType, TDatabaseService, TPartitionKey, TContext>(
                this TMutationType queryType, bool allowAnonymous = false) where TMutationType : ObjectGraphType<object>
            where TInputGraphType : GraphType
            where TModelType : class, IModel<Guid>
            where TDatabaseService: IDatabaseService<TModelType, TPartitionKey, TContext>
            where TContext: IContext
            where TPartitionKey: IEquatable<TPartitionKey>
        {
            var name = $"patch{typeof(TInputGraphType).InputGraphTypeName(true)}";
            var argumentName = $"{typeof(TInputGraphType).InputGraphTypeName()}Patch";

            queryType.FieldAsync<BooleanGraphType>(
                name: name,
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>() {Name = "id"},
                    new QueryArgument<NonNullGraphType<JsonPatchDocumentGraphType>>() {Name = argumentName}
                ),
                resolve: async context =>
                {
                    if(!allowAnonymous){
                        var requestIsAuthenticated = context.RequestIsAuthenticated();

                        if (!requestIsAuthenticated)
                        {
                            throw new UnauthorizedAccessException();
                        }
                    }

                    var id = context.GetIdArgument();
                    var jsonPatchDocument =
                        JsonConvert.DeserializeObject<JsonPatchDocument<TModelType>>(
                            context.GetArgument<string>(argumentName));
                    var databaseService = context.GetService<TDatabaseService>();

                    await databaseService.UpdateAsync(id, jsonPatchDocument, CancellationToken.None);

                    return true;
                }
            );
        }

        public static void
            GraphTypeMutationDelete<TMutationType, TInputGraphType, TModelType, TDatabaseService, TPartitionKey, TContext>(
                this TMutationType queryType, bool allowAnonymous = false) where TMutationType : ObjectGraphType<object>
            where TInputGraphType : GraphType
            where TModelType : class, IModel<Guid>
            where TDatabaseService: IDatabaseService<TModelType, TPartitionKey, TContext>
            where TContext: IContext
            where TPartitionKey: IEquatable<TPartitionKey>
        {
            var name = $"delete{typeof(TInputGraphType).InputGraphTypeName(true)}";

            queryType.FieldAsync<BooleanGraphType>(
                name: name,
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>() {Name = "id"}
                ),
                resolve: async context =>
                {
                    if(!allowAnonymous){
                        var requestIsAuthenticated = context.RequestIsAuthenticated();

                        if (!requestIsAuthenticated)
                        {
                            throw new UnauthorizedAccessException();
                        }
                    }

                    var id = context.GetIdArgument();
                    var databaseService = context.GetService<TDatabaseService>();

                    await databaseService.DeleteAsync(id, CancellationToken.None);

                    return true;
                }
            );
        }
    }
}
