using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Wemogy.AspNet.Dapr;
using Wemogy.AspNet.Monitoring;
using Wemogy.AspNet.Swagger;
using Wemogy.Core.Monitoring;

namespace Wemogy.AspNet.Startup;

public class StartupOptions
{
    internal OpenApiEnvironment? OpenApiEnvironment { get; private set; }
    internal MonitoringEnvironment? MonitoringEnvironment { get; private set; }
    internal DaprEnvironment? DaprEnvironment { get; private set; }
    internal HashSet<Type> Middlewares { get; private set; }
    internal HashSet<Type> HealthChecks { get; private set; }

    /// <summary>
    /// Sets the <see cref="Microsoft.AspNetCore.Mvc.MvcOptions.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes"/> property for all contollers.
    /// </summary>
    public bool SuppressImplicitRequiredAttributeForNonNullableReferenceTypes { get; set; }

    public StartupOptions()
    {
        Middlewares = new HashSet<Type>();
    }

    public OpenApiEnvironment AddOpenApi(string version, string? pathToXmlDocumentationFile = null)
    {
        pathToXmlDocumentationFile ??= Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetCallingAssembly().GetName().Name}.xml");
        OpenApiEnvironment = new OpenApiEnvironment(version, pathToXmlDocumentationFile);
        return OpenApiEnvironment;
    }

    /// <summary>
    /// Adds and configures logging and monitoring for the application.
    /// </summary>
    /// <param name="serviceName">Name of the service as it should appear in the logs an graphs (e.g. "core-main")</param>
    /// <param name="serviceVersion">Version of the service as it should appear in the logs an graphs (e.g. "1.2.3")</param>
    public MonitoringEnvironment AddMonitoring(string serviceName, string serviceVersion)
    {
        MonitoringEnvironment = new MonitoringEnvironment(serviceName, serviceVersion);
        return MonitoringEnvironment;
    }

    /// <summary>
    /// Adds and configures logging and monitoring for the application.
    /// </summary>
    public MonitoringEnvironment AddMonitoring()
    {
        MonitoringEnvironment = new MonitoringEnvironment();
        return MonitoringEnvironment;
    }

    /// <summary>
    /// Ensures, that Dapr is added to the application and registers the Dapr middleware.
    /// </summary>
    public DaprEnvironment AddDapr()
    {
        DaprEnvironment = new DaprEnvironment();
        return DaprEnvironment;
    }

    public StartupOptions AddMiddleware<TMiddleware>()
        where TMiddleware : class
    {
        Middlewares.Add(typeof(TMiddleware));
        return this;
    }
}
