using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Wemogy.AspNet.Startup;
using Wemogy.Core.Extensions;

namespace Wemogy.AspNet.Swagger
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddDefaultSwagger(
            this IServiceCollection services,
            StartupOptions options)
        {
            if (options.OpenApiEnvironment == null)
            {
                throw new ArgumentException("OpenApiEnvironment is not configured.");
            }

            services.AddSwaggerGen(c =>
            {
                foreach (var configure in options.OpenApiEnvironment.SwaggerGenOptions)
                {
                    configure(c);
                }
            });

            return services;
        }

        public static void UseDefaultSwagger(this IApplicationBuilder applicationBuilder, StartupOptions options)
        {
            if (options.OpenApiEnvironment == null)
            {
                throw new ArgumentException("OpenApiEnvironment is not configured.");
            }

            applicationBuilder.UseSwagger();
            applicationBuilder.UseSwaggerUI(c =>
            {
                foreach (var group in options.OpenApiEnvironment.OpenApiGroups)
                {
                    // Only publish groups that are marked as publishable
                    if (group.Value.Publish)
                    {
                        c.SwaggerEndpoint($"/swagger/{group.Key}/swagger.json", $"{group.Value.OpenApiInfo.Title} - {group.Value.OpenApiInfo.Version}");
                    }
                }
            });
        }

        internal static void AddDefaults(
                this SwaggerGenOptions c,
                string name,
                OpenApiInfo openApiInfo,
                string? pathToXmlDocumentationFile = null,
                bool removeDtoSuffix = true,
                bool removeAsyncSuffix = true)
        {
            c.SwaggerDoc(name, openApiInfo);

            if (removeDtoSuffix)
            {
                c.CustomSchemaIds(x => x.Name.RemoveTrailingString("Dto"));
            }

            // Add Operation ID based on controller method name
            if (removeAsyncSuffix)
            {
                c.CustomOperationIds(e => $"{e.ActionDescriptor.RouteValues["action"]}".RemoveTrailingString("Async"));
            }
            else
            {
                c.CustomOperationIds(e => $"{e.ActionDescriptor.RouteValues["action"]}");
            }

            c.IncludeXmlComments(pathToXmlDocumentationFile);
        }

        internal static void AddSecurityScheme(this SwaggerGenOptions c, SecuritySchemeDefaults securityScheme)
        {
            OpenApiSecurityScheme scheme;
            string name;

            switch (securityScheme)
            {
                case SecuritySchemeDefaults.JwtBearer:
                    name = "Bearer";
                    scheme = new OpenApiSecurityScheme
                    {
                        Description = "Add your JWT token here",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT"
                    };
                    break;
                default:
                case SecuritySchemeDefaults.None:
                    return;
            }

            c.AddSecurityDefinition(name, scheme);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = name,
                            Type = ReferenceType.SecurityScheme
                        }
                    }, new List<string>()
                }
            });
        }
    }
}
