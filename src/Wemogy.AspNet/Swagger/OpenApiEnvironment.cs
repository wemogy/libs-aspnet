using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNet.SwaggerGen;
using Wemogy.AspNet.Swagger;

namespace Wemogy.AspNet.Swagger
{
    public class OpenApiEnvironment
    {
        public string Version { get; }
        public string PathToXmlDocumentationFile { get; private set; }
        public Dictionary<string, OpenApiInfo> OpenApiGroups { get; }
        public bool RemoveDtoSuffix { get; }
        public List<Action<SwaggerGenOptions>> SwaggerGenOptions { get; }

        public OpenApiEnvironment(string version, string pathToXmlDocumentationFile)
        {
            Version = version;
            PathToXmlDocumentationFile = pathToXmlDocumentationFile;
            OpenApiGroups = new Dictionary<string, OpenApiInfo>();
            RemoveDtoSuffix = true;
            SwaggerGenOptions = new List<Action<SwaggerGenOptions>> { x => x.SupportNonNullableReferenceTypes() };
        }

        public OpenApiEnvironment WithApiGroup(string name, string title, string description)
        {
            var info = new OpenApiInfo
            {
                Title = title,
                Version = Version,
                Description = description
            };
            OpenApiGroups.Add(name, info);
            SwaggerGenOptions.Add(c => c.AddDefaults(name, info, PathToXmlDocumentationFile, RemoveDtoSuffix));
            return this;
        }

        public OpenApiEnvironment WithSecurityScheme(SecuritySchemeDefaults scheme)
        {
            SwaggerGenOptions.Add(c => c.AddSecurityScheme(scheme));
            return this;
        }

        public OpenApiEnvironment WithFilter<TFilter>() where TFilter : IOperationFilter
        {
            SwaggerGenOptions.Add(c => c.OperationFilter<TFilter>());
            return this;
        }
    }
}
