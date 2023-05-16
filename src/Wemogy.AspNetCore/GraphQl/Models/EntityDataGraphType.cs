using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GraphQL.Types;
using GraphQL.DataLoader;
using Wemogy.AspNetCore.GraphQl.CustomScalars;
using Wemogy.AspNetCore.GraphQl.Extensions;
using Wemogy.DataAccess.Interfaces;
using IDataLoaderContextAccessor = GraphQL.DataLoader.IDataLoaderContextAccessor;

namespace Wemogy.AspNetCore.GraphQl.Models
{
    public abstract class EntityDataGraphType<T> : ObjectGraphType<T> where T : IModel<Guid>
    {
        protected readonly IDataLoaderContextAccessor Accessor;
        protected readonly IServiceProvider ServiceProvider;


        protected EntityDataGraphType(IDataLoaderContextAccessor accessor, IServiceProvider serviceProvider)
        {
            this.Accessor = accessor;
            ServiceProvider = serviceProvider;

            this.Field(nameof(IModel<Guid>.Id), o => o.Id.ToString());
            this.Field<TimestampGraphType>(nameof(IModel<Guid>.CreatedAt));
            this.Field<TimestampGraphType>(nameof(IModel<Guid>.UpdatedAt));
        }

        /// <summary>
        /// Use this to load your :1 relation foreign objects
        /// </summary>
        /// <typeparam name="TGraphType"></typeparam>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="idResolver"></param>
        private void DatabaseServiceReferenceFieldAsync<TGraphType, TModel, TService, TContext>(
            Expression<Func<T, Guid?>> idResolver) where TService : IDatabaseService<TModel, Guid, TContext>
            where TGraphType : IGraphType
            where TModel : class, IModel<Guid>
            where TContext: IContext
        {
            var foreignKeyPropertyName = (idResolver.Body as MemberExpression)?.Member.Name;
            if (foreignKeyPropertyName == null)
            {
                foreignKeyPropertyName =
                    ((idResolver.Body as UnaryExpression)?.Operand as MemberExpression)?.Member.Name;
            }

            if (foreignKeyPropertyName == null || foreignKeyPropertyName.Length < 3)
            {
                throw new Exception(
                    $"EntityDataGraphType.DatabaseServiceReferenceFieldAsync could not resolve the foreignKeyPropertyName or the foreignKeyPropertyName has an invalid length! foreignKeyPropertyName: {foreignKeyPropertyName}");
            }

            var isNullable = idResolver.ReturnType.Name.Contains("Nullable");
            var compiledIdResolver = idResolver.Compile();
            this.Field(
                name: foreignKeyPropertyName,
                expression: o => compiledIdResolver(o).HasValue ? compiledIdResolver(o).Value.ToString() : null,
                nullable: isNullable);

            /*
            // Convert xxxId to xxx (remove the Id at the end)
            var foreignPropertyName = foreignKeyPropertyName.Substring(0, foreignKeyPropertyName.Length - 2);
            this.FieldAsync<TGraphType>(
                name: foreignPropertyName,
                resolve: async context =>
                {
                    #region If the collection is already loaded => return it. (ToDo: Maybe optimize)

                    var x = foreignPropertyName;
                    var property = context.Source.GetType().GetProperty(x);
                    if (property != null && (property.GetValue(context.Source) as dynamic) != null)
                    {
                        return property.GetValue(context.Source) as dynamic;
                    }

                    #endregion

                    var id = idResolver.Compile()(context.Source);
                    if (!id.HasValue)
                    {
                        return null;
                    }

                    var databaseService = context.GetService<T, TService>();

                    return await this.Accessor.LoadFromDatabaseService(databaseService, id.Value);
                }
            );
             */
        }

        protected void DatabaseServiceCollectionReferenceFieldAsync<TGraphType, TModel, TDatabaseService, TContext>(string name,
            string propertyName) where TDatabaseService : IDatabaseService<TModel, Guid, TContext>
            where TGraphType : IGraphType
            where TModel : class, IModel<Guid>
            where TContext: IContext
        {
            this.Field<ListGraphType<TGraphType>, IEnumerable<TModel>>()
                .Name(name)
                .Description($"Get all {name}")
                .ResolveAsync(context =>
                {
                    #region If the collection is already loaded => return it. (ToDo: Maybe optimize)

                    var x = name;
                    var property = context.Source.GetType().GetProperty(x);
                    /*var activeUserManager = context.GetService<T, ActiveUserManager>();

                    var isSharedLinkDataRequest = activeUserManager.SharedLinkId.HasValue &&
                                                (context.Operation.SelectionSet.Selections.FirstOrDefault() as Field)
                                                ?.Name == "sharedLinkData";
                    if (isSharedLinkDataRequest ||
                        property != null && (property.GetValue(context.Source) as dynamic).Count > 0)
                    {
                        return property.GetValue(context.Source) as dynamic;
                    }*/

                    #endregion

                    var databaseService = context.GetService<TDatabaseService>();

                    // Get or add a collection batch loader with the key "GetOrdersByUserId"
                    // The loader will call GetOrdersByUserIdAsync with a batch of keys
                    var referencesLoader = this.Accessor.Context.GetOrAddCollectionBatchLoader<Guid, TModel>(
                        $"Get{typeof(TModel)}ByReferenceProperty{propertyName}",
                        (IEnumerable<Guid> ids) =>
                            databaseService.GetByReferenceIdsAsync(ids.ToList(), propertyName,
                                context.CancellationToken));

                    // Add this UserId to the pending keys to fetch data for
                    // The task will complete with an IEnumberable<Order> once the fetch delegate has returned
                    return referencesLoader.LoadAsync(context.Source.Id);
                });
        }
        /*
        protected void DatabaseServiceReferenceFieldAsync<TGraphType, TModel, TDatabaseService>(string name,
            string propertyName) where TDatabaseService : IDatabaseService<TModel>
            where TGraphType : IGraphType
            where TModel : DocumentModel
        {
            this.FieldAsync<TGraphType>(
                name: name,
                resolve: async context =>
                {
                    var databaseService = context.GetService<T, TDatabaseService>();

                    // Get or add a collection batch loader with the key "GetOrdersByUserId"
                    // The loader will call GetOrdersByUserIdAsync with a batch of keys
                    var referencesLoader = this.Accessor.Context.GetOrAddCollectionBatchLoader<Guid, TModel>(
                        $"Get{typeof(TModel)}ByReferenceProperty{propertyName}",
                        (IEnumerable<Guid> ids) =>
                            databaseService.GetByReferenceIdsAsync(ids.ToList(), propertyName, CancellationToken.None));

                    // Add this UserId to the pending keys to fetch data for
                    // The task will complete with an IEnumberable<Order> once the fetch delegate has returned
                    return await referencesLoader.LoadAsync(context.Source.Id);
                });
        }*/
/*
        /// <summary>
        /// Use this to load your :n relation foreign objects
        /// </summary>
        /// <typeparam name="TGraphType"></typeparam>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TDatabaseService"></typeparam>
        /// <param name="name"></param>
        /// <param name="propertyName"></param>
        protected void DatabaseServiceCollectionReferenceFieldAsync<TGraphType, TModel, TDatabaseService>(string name,
            string propertyName) where TDatabaseService : IDatabaseService<TModel>
            where TGraphType : IGraphType
            where TModel : DocumentModel
        {
            this.FieldAsync<ListGraphType<TGraphType>>(
                name: name,
                resolve: async context =>
                {
                    #region If the collection is already loaded => return it. (ToDo: Maybe optimize)

                    var x = name;
                    var property = context.Source.GetType().GetProperty(x);
                    var activeUserManager = context.GetService<T, ActiveUserManager>();

                    var isSharedLinkDataRequest = activeUserManager.SharedLinkId.HasValue &&
                                                (context.Operation.SelectionSet.Selections.FirstOrDefault() as Field)
                                                ?.Name == "sharedLinkData";
                    if (isSharedLinkDataRequest ||
                        property != null && (property.GetValue(context.Source) as dynamic).Count > 0)
                    {
                        return property.GetValue(context.Source) as dynamic;
                    }

                    #endregion

                    var databaseService = context.GetService<T, TDatabaseService>();

                    // Get or add a collection batch loader with the key "GetOrdersByUserId"
                    // The loader will call GetOrdersByUserIdAsync with a batch of keys
                    var referencesLoader = this.Accessor.Context.GetOrAddCollectionBatchLoader<Guid, TModel>(
                        $"Get{typeof(TModel)}ByReferenceProperty{propertyName}",
                        (IEnumerable<Guid> ids) =>
                            databaseService.GetByReferenceIdsAsync(ids, propertyName, CancellationToken.None));

                    // Add this UserId to the pending keys to fetch data for
                    // The task will complete with an IEnumberable<Order> once the fetch delegate has returned
                    return await referencesLoader.LoadAsync(context.Source.Id);
                });
        }

        /// <summary>
        /// Use this to load your :n relation foreign objects
        /// </summary>
        /// <typeparam name="TGraphType"></typeparam>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TInclude"></typeparam>
        /// <param name="name"></param>
        /// <param name="include"></param>
        /// <param name="query"></param>
        /// <param name="databaseRelationService"></param>
        protected void DatabaseServiceCollectionRelationFieldAsync<TGraphType, TModel, TInclude, TDatabaseService>(
            string name, Expression<Func<TModel, TInclude>> include, Func<Guid, Expression<Func<TModel, bool>>> query)
            where TDatabaseService : IDatabaseRelationService<TModel>
            where TGraphType : IGraphType
            where TModel : class
        {
            this.FieldAsync<ListGraphType<TGraphType>>(
                name: name,
                resolve: async context =>
                {
                    var databaseRelationService = context.GetService<T, TDatabaseService>();
                    var references = await databaseRelationService.QueryAsync(include, query(context.Source.Id),
                        CancellationToken.None);

                    return references;
                });
        }
 */
    }
}
