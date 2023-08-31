using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using Wemogy.Core.Monitoring;

namespace Wemogy.AspNet.Monitoring
{
    public static class MonitoringExtensions
    {
        public static IServiceCollection AddDefaultMonitoring(
            this IServiceCollection services,
            MonitoringEnvironment environment)
        {
            if (environment.UseApplicationInsights)
            {
                services.AddApplicationInsightsTelemetry(x => x.ConnectionString = environment.ApplicationInsightsConnectionString);
                services.AddSingleton<ITelemetryInitializer>(new CloudRoleNameTelemetryInitializer(environment.ServiceName));
            }

            // Metrics
            services.AddOpenTelemetry().WithMetrics(builder =>
            {
                builder.AddRuntimeInstrumentation();
                builder.AddAspNetCoreInstrumentation();
                builder.AddPrometheusExporter();
            });

            // Traces
            services.AddOpenTelemetry().WithTracing(builder =>
            {
                builder.ConfigureResource((resource) =>
                {
                    resource.AddService(environment.ServiceName);
                });
                builder.AddAspNetCoreInstrumentation();
                builder.AddEntityFrameworkCoreInstrumentation();
                builder.AddOtlpExporter();
            });

            services.AddSingleton(environment);
            services.AddSingleton<IMonitoringService, MonitoringService>();
            return services;
        }

        public static void UseDefaultMonitoring(
            this IApplicationBuilder applicationBuilder,
            MonitoringEnvironment environment)
        {


            if (environment.UsePrometheus)
            {
                applicationBuilder.UseMetricServer();
                applicationBuilder.UseHttpMetrics();
            }
        }
    }
}
