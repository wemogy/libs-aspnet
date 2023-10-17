using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Wemogy.AspNet.Swagger;

namespace Wemogy.AspNet.Swagger
{
    public class OpenApiEnvironment
    {
        public string Version { get; }
        public string PathToXmlDocumentationFile { get; private set; }
        public Dictionary<string, OpenApiConfig> OpenApiGroups { get; }
        public bool RemoveDtoSuffix { get; }
        public List<Action<SwaggerGenOptions>> SwaggerGenOptions { get; }

        public OpenApiEnvironment(string version, string pathToXmlDocumentationFile)
        {
            Version = version;
            PathToXmlDocumentationFile = pathToXmlDocumentationFile;
            OpenApiGroups = new Dictionary<string, OpenApiConfig>();
            RemoveDtoSuffix = true;
            SwaggerGenOptions = new List<Action<SwaggerGenOptions>> { x => x.SupportNonNullableReferenceTypes() };
        }

        /// <summary>
        /// Adds a new API group to the OpenAPI specification.
        /// </summary>
        /// <param name="name">Name of the OpenAPI spec file (no special chars)</param>
        /// <param name="title">Title of the OpenAPI spec.</param>
        /// <param name="description">Description of the OpenAPI spec.</param>
        /// <param name="publish">Publish OpenAPI specification to the UI.</param>
        public OpenApiEnvironment WithApiGroup(string name, string title, string description, bool publish = true)
        {
            var info = new OpenApiInfo
            {
                Title = title,
                Version = Version,
                Description = description
            };
            OpenApiGroups.Add(name, new OpenApiConfig(info, publish));
            SwaggerGenOptions.Add(c => c.AddDefaults(name, info, PathToXmlDocumentationFile, RemoveDtoSuffix));
            return this;
        }

        public OpenApiEnvironment WithSecurityScheme(SecuritySchemeDefaults scheme)
        {
            SwaggerGenOptions.Add(c => c.AddSecurityScheme(scheme));
            return this;
        }

        public OpenApiEnvironment WithFilter<TFilter>()
            where TFilter : IOperationFilter
        {
            SwaggerGenOptions.Add(c => c.OperationFilter<TFilter>());
            return this;
        }
    }
}
