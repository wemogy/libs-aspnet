using System;
using GraphQL.MicrosoftDI;
using Wemogy.AspNetCore.GraphQl.Models;
using GraphQL.Server;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Wemogy.AspNetCore.GraphQl.Extensions
{
    public static class GraphQlSetupExtensions
    {
        public static IGraphQLBuilder AddGraphQlSchema<TSchema>(this IServiceCollection serviceCollection, Func<IServiceProvider, TSchema> factory) where TSchema: Schema
        {
            serviceCollection.AddSingleton<TSchema>(services => factory(new SelfActivatingServiceProvider(services)));

            return serviceCollection.AddGraphQL(options =>
                {
                    options.EnableMetrics = true;
                    options.UnhandledExceptionDelegate = context =>
                    {
                        // ToDo: maybe log to insights
                        Console.WriteLine(context.Exception);
                    };
                })
                // add required service for de/serialization
                .AddSystemTextJson()
                // add required services for DataLoader support
                .AddDataLoader()
                .AddUserContextBuilder(httpContext => new GraphQlUserContext(httpContext))
                .AddGraphTypes(typeof(TSchema));
        }

        /// <summary>
        /// Make sure that TSchema type has the required constructor!
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <typeparam name="TSchema">TSchema MUST have a constructor with IServiceProvider parameter</typeparam>
        /// <returns></returns>
        public static IGraphQLBuilder AddGraphQlSchema<TSchema>(this IServiceCollection serviceCollection)
            where TSchema: Schema
        {
            return serviceCollection.AddGraphQlSchema(serviceProvider => Activator.CreateInstance(typeof(TSchema), serviceProvider) as TSchema);
        }

        public static void UseGraphQlSchema<TSchema>(this IApplicationBuilder applicationBuilder, string path = "/graphql") where TSchema: Schema
        {
            applicationBuilder
                .UseGraphQL<TSchema>(path)
                .UseGraphQLAltair();
        }
    }
}
