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

    // Middleware
    _options
      .AddMiddleware<ApiExceptionFilter>();

    // Add Swagger
    _options
        .AddOpenApi("v1")
        .WithApiGroup("public", "Fancy API", "This is my fancy API.")
        .WithSecurityScheme(SecuritySchemeDefaults.JwtBearer);

    // Add Monitoring
    _options
        .AddMonitoring()
        .WithMeter(Observability.Meter.Name)
        .WithApplicationInsights(Configuration["AzureApplicationInsightsConnectionString"])
        .WithPrometheus();
}
```

Add the `StartupOptions` and register the default setup using the `AddDefaultSetup` extension method. Alternatively, you can also register our defaults manually in case you need to tweak in some of your own adjustments.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Register all options automatically
    services.AddDefaultSetup(_options);

    // or

    // Register the options manually
    services.AddDefaultCors();
    services.AddDefaultSwagger(_options);
    services.AddDefaultMonitoring(_options);
    services.AddDefaultControllers(_options, _options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes);
    services.AddDefaultHealthChecks(_options);
    services.AddDefaultRouting();
}
```

Make sure, the default setup is getting used. Again, you can either use the `UseDefaultSetup` extension method or use the options manually, in case you need to tweak in some of your own adjustments.

```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseDefaultSetup(env, _options);

    // or

    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseDefaultCors();
    app.UseDefaultSwagger(_options);
    app.UseDefaultMonitoring(_options);
    app.UseDefaultRouting();
    app.UseCloudEvents(); // when using Dapr
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseDefaultMiddleware(_options);
    app.UseDefaultEndpoints(_options);
}
```

For modern or Minimal API services (introduced in .NET 6), this would look similar to this. In the `Program.cs`, add the `StartupOptions` and register the default setup.

```csharp
var options = new StartupOptions();
options
    .AddOpenApi("v1")

options
    .AddMonitoring()
    .WithApplicationInsights(Configuration["AzureApplicationInsightsConnectionString"])

// ...

app.AddDefaultSetup(options);
app.UseDefaultSetup(app.Environment, options);
```

## Monitoring

We use Open Telemetry to provide monitoring for your application. The library provides a default setup for you, which you can use as is or extend to your needs. Open Telemetry interacts with the native Metrics and Tracing capabilities of ASP.NET.

### Create Monitoring class

Every project should have a `Observability.cs` class, which is responsible for defining the Meters.

```csharp
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Reflection;
using OpenTelemetry.Metrics;

public class Observability
{
    // Define a default ActivitySource
    public static readonly ActivitySource DefaultActivities = new ActivitySource("ServiceName");

    // Define a default Meter with name and version
    public static readonly Meter Meter = new("ServiceName", Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0");

    // Create Counters, Histograms, etc. from that default Meter
    public  static readonly Counter<long> Pings = Meter.CreateCounter<long>("service_countername", description: "Total number of pings");
    public static readonly Histogram<int> PingDelay = Meter.CreateHistogram<int>("service_histgramname", "ms", "Think time in ms for a ping");
}
```

Make sure to register the Meter at Startup.

```csharp
var options = new StartupOptions();
options
    .AddMonitoring()
    .WithActivitySource(Observability.DefaultActivities.Name)
    .WithMeter(Observability.Meter.Name)
    // ...
```

Use the Meter in your code.

```csharp
Observability.Pings.Add(1);

Observability.PingDelay.Record(new Random().Next(50, 100));
```

Use Activities in your Code to create sections of code that can be traced.

```csharp
using (var fistActivity = Observability.DefaultActivities.StartActivity("First section"))
{
    fistActivity?.AddTag("date", DateTime.UtcNow.ToString()); // Add optional tags
    Thread.Sleep(100); // Do work
    fistActivity?.Stop(); // Stop activity
};

using (var secondActivity = Observability.DefaultActivities.StartActivity("Second section"))
{
    secondActivity?.AddTag("foo", "far"); // Add optional tags
    Thread.Sleep(200); // Do work
    secondActivity?.Stop();  // Stop activity
};
```

## Health Checks

The library automatically includes a health check endpoint at `/healthz`, which checks the basic health of the service.

You can add additional health checks to the default setup.

- Inline Health Checks
- Custom Health Checks, that implement the IHealthCheck interface
- [Database Health Checks](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-6.0#database-probe)
- [Entity Framework Core DbContext probes](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-6.0#entity-framework-core-dbcontext-probe)

```csharp
_options.ConfigureHealthChecks(builder => {
    builder
        .AddCheck("MyHealthCheck", () => HealthCheckResult.Healthy("Everything is fine.")
        .AddCheck("MyOtherHealthCheck", MyHealthChecker) // Implement IHealthCheck
        .AddSqlServer("<MY_CONNECTION_STRING>")
        .AddDbContextCheck<SampleDbContext>();
});
```
