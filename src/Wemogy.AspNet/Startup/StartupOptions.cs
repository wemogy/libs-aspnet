using System;
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

    public StartupOptions()
    {
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
    /// <param name="serviceName">Name of the (micro)service as it should appear in the logs an graphs (e.g. "core-main")</param>
    public MonitoringEnvironment AddMonitoring(string serviceName)
    {
        MonitoringEnvironment = new MonitoringEnvironment(serviceName);
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
}
