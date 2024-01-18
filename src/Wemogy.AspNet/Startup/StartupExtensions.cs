using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wemogy.AspNet.FluentValidation;
using Wemogy.AspNet.Formatters;
using Wemogy.AspNet.Json;
using Wemogy.AspNet.Middlewares;
using Wemogy.AspNet.Monitoring;
using Wemogy.AspNet.Refit;
using Wemogy.AspNet.Swagger;
using Wemogy.AspNet.Transformers;

namespace Wemogy.AspNet.Startup
{
    public static class StartupExtensions
    {
        /// <summary>
        /// Set the default <see cref="MvcOptions"/> settings for Controllers in wemogy applications.
        /// </summary>
        /// <param name="options">The <see cref="MvcOptions"/> to update.</param>
        /// <param name="suppressImplicitRequiredAttributeForNonNullableReferenceTypes">The <see cref="Microsoft.AspNetCore.Mvc.MvcOptions.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes"/> property for all contollers.</param>
        public static void GetWemogyDefaultControllerOptions(MvcOptions options, bool suppressImplicitRequiredAttributeForNonNullableReferenceTypes = false)
        {
            options.SuppressAsyncSuffixInActionNames = false;
            options.InputFormatters.Insert(0, new RawBodyInputFormatter());
            options.Conventions.Add(new RouteTokenTransformerConvention(new KebabCaseParameterTransformer()));
            options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = suppressImplicitRequiredAttributeForNonNullableReferenceTypes;
        }

        public static void AddDefaultControllers(this IServiceCollection serviceCollection, StartupOptions options, bool suppressImplicitRequiredAttributeForNonNullableReferenceTypes = false)
        {
            var builder = serviceCollection.AddControllers(options => GetWemogyDefaultControllerOptions(options, suppressImplicitRequiredAttributeForNonNullableReferenceTypes));
            builder.AddWemogyJsonOptions();

            if (options.DaprEnvironment != null)
            {
                builder.AddDapr();
            }
        }

        public static void AddDefaultSetup(this IServiceCollection serviceCollection, StartupOptions options)
        {
            serviceCollection.AddDefaultCors();

            if (options.OpenApiEnvironment != null)
            {
                serviceCollection.AddDefaultSwagger(options);
            }

            if (options.MonitoringEnvironment != null)
            {
                serviceCollection.AddDefaultMonitoring(options);
            }

            serviceCollection.AddDefaultControllers(options, options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes);

            serviceCollection.AddDefaultHealthChecks(options);

            serviceCollection.AddDefaultRouting();
        }

        public static void AddDefaultCors(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });
        }

        public static void AddDefaultHealthChecks(this IServiceCollection serviceCollection, StartupOptions options)
        {
            var healthChecksBuilder = serviceCollection.AddHealthChecks();
            options.ConfigureHealthCheckBuilder?.Invoke(healthChecksBuilder);
        }

        public static void AddDefaultRouting(this IServiceCollection serviceCollection)
        {
            // Enforce lowercase routes
            serviceCollection.AddRouting(options => options.LowercaseUrls = true);
        }

        public static void UseDefaultSetup(this IApplicationBuilder applicationBuilder, IHostEnvironment env, StartupOptions options)
        {
            if (env.IsDevelopment())
            {
                applicationBuilder.UseDeveloperExceptionPage();
            }

            // Must come before any "UseSwagger()" calls because the Swagger middleware, when it knows that the request
            // is for Swagger, it doesn't forward the request onto the next middleware, it just immediately returns.
            applicationBuilder.UseDefaultCors();

            if (options.OpenApiEnvironment != null)
            {
                applicationBuilder.UseDefaultSwagger(options);
            }

            if (options.MonitoringEnvironment != null)
            {
                applicationBuilder.UseDefaultMonitoring(options);
            }

            applicationBuilder.UseDefaultRouting();

            if (options.DaprEnvironment?.UseCloudEvents == true)
            {
                applicationBuilder.UseCloudEvents();
            }

            applicationBuilder.UseAuthentication();
            applicationBuilder.UseAuthorization();

            applicationBuilder.UseDefaultMiddleware(options);

            applicationBuilder.UseDefaultEndpoints(options);
        }

        public static void UseDefaultRouting(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseRouting();
        }

        public static void UseDefaultCors(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseCors();
        }

        /// <summary>
        /// Adds the default exception handling middleware from this library
        /// and additional middlewares defined in the StartupOptions to the pipeline.
        /// </summary>
        public static void UseDefaultMiddleware(this IApplicationBuilder applicationBuilder, StartupOptions options)
        {
            // Standard exception handling middleware
            applicationBuilder.UseExceptionHandlerMiddleware();

            // Additional optional middleware
            foreach (var middleware in options.Middlewares)
            {
                applicationBuilder.UseMiddleware(middleware);
            }
        }

        /// <summary>
        /// Adds the default exception handling middleware from this library to the pipeline.
        /// </summary>
        public static void UseExceptionHandlerMiddleware(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<ErrorExceptionHandlerMiddleware>();
            applicationBuilder.UseMiddleware<SystemExceptionHandlerMiddleware>();
            applicationBuilder.UseMiddleware<RefitExceptionHandlerMiddleware>();
            applicationBuilder.UseMiddleware<FluentValidationExceptionHandlerMiddleware>();
        }

        public static void UseDefaultEndpoints(this IApplicationBuilder applicationBuilder, StartupOptions options)
        {
            applicationBuilder.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/healthz");

                if (options.DaprEnvironment != null)
                {
                    endpoints.MapSubscribeHandler(); // Register endpoints for Dapr PubSub
                }
            });
        }

        /// <summary>
        /// Adds Azure Key Vault secrets to the configuration based on one of these authentication methods:
        /// Identity: The calling application has an Azure Identity, and can authenticate against Azure Key Vault
        /// Service Principal: The calling application has Client ID and Secret of an Azure Service Principal
        /// </summary>
        /// <param name="builder">An IConfigurationBuilder</param>
        /// <param name="azureKeyVaultUri">The URI to the Azure Key Vault</param>
        /// <param name="azureKeyVaultClientId">Fill, if you want to authenticate with a Service Principal, leave empty for Identity</param>
        /// <param name="azureKeyVaultClientSecret">Secret. Fill, if you want to authenticate with a Service Principal, leave empty for Identity</param>
        public static IConfigurationBuilder AddAuthenticatedAzureKeyVault(
            this IConfigurationBuilder builder,
            string azureKeyVaultUri,
            string? azureKeyVaultClientId = null,
            string? azureKeyVaultClientSecret = null)
        {
            // The clientId and clientSecrets fields should be left empty, as we always
            // prefer connecting to Azure KeyVault using a manage identity of the system we are
            // running on. Only in edge scenarios, we can provide an Client ID and Secret of an
            // Azure Service principal that has access to Key Vault.
            if (!string.IsNullOrEmpty(azureKeyVaultClientId) && !string.IsNullOrEmpty(azureKeyVaultClientSecret))
            {
                Console.Write($"Adding Azure KeyVault '{azureKeyVaultUri}' to configuration using credentials...");
                builder.AddAzureKeyVault(azureKeyVaultUri, azureKeyVaultClientId, azureKeyVaultClientSecret);
            }
            else
            {
                Console.Write($"Adding Azure KeyVault '{azureKeyVaultUri}' to configuration using identity...");
                builder.AddAzureKeyVault(azureKeyVaultUri);
            }

            Console.WriteLine("done.");

            return builder;
        }
    }
}
