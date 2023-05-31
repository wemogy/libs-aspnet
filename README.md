# ![wemogy logo](https://wemogyimages.blob.core.windows.net/logos/wemogy-github-tiny.png) ASP.NET Library

## Getting started

Install the [NuGet package](https://www.nuget.org/packages/Wemogy.AspNet) into your project.

```bash
dotnet add package Wemogy.AspNet
```

When spinning up the ASP.NET application, you configure the library, which is usually done in the `Startup.cs` file.

First, create a `StartupOptions` instance and configure it for your project.

```csharp
private readonly StartupOptions _options;

public Startup(IConfiguration configuration)
{
    Configuration = configuration;

    _options = new StartupOptions();

    // Add Swagger
    _options
        .AddOpenApi("v1")
        .WithApiGroup("public", "Fancy API", "This is my fancy API.")
        .WithSecurityScheme(SecuritySchemeDefaults.JwtBearer);

    // Add Monitoring
    _options
        .AddMonitoring(Configuration["ServiceName"])
        .WithApplicationInsights(Configuration["AzureApplicationInsightsConnectionString"])
        .WithPrometheus();
}
```

Add the `StartupOptions` and register the default setup.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddDefaultSetup(_options);

    // ...
}
```

Make sure, the default setup gets used.

```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseDefaultSetup(env, _options);

    // ...
}
```

Checkout the [Documentation](https://libs-aspnet.docs.wemogy.com/) to get information about the available classes and extensions.
