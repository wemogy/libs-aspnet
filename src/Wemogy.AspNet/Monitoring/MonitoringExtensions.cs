using Azure.Monitor.OpenTelemetry.AspNetCore;
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
            // Metrics
            services.AddOpenTelemetry().WithMetrics(builder =>
            {
                foreach (var meterName in environment.MeterNames)
                {
                    builder.AddMeter(meterName);
                }

                builder.AddRuntimeInstrumentation();
                builder.AddHttpClientInstrumentation();
                builder.AddAspNetCoreInstrumentation();

                if (environment.UsePrometheus)
                {
                    builder.AddPrometheusExporter();
                }
            });

            // Traces
            services.AddOpenTelemetry().WithTracing(builder =>
            {
                builder.ConfigureResource((resource) =>
                {
                    resource.AddService(environment.ServiceName, serviceVersion: environment.ServiceVersion);
                });

                builder.AddAspNetCoreInstrumentation();
                builder.AddEntityFrameworkCoreInstrumentation();

                if (environment.UseOtlpExporter)
                {
                    builder.AddOtlpExporter(options =>
                    {
                        options.Endpoint = environment.OtlpExportEndpoint;
                    });
                }
            });

            if (environment.UseApplicationInsights)
            {
                services.AddOpenTelemetry().UseAzureMonitor(options =>
                {
                    options.ConnectionString = environment.ApplicationInsightsConnectionString;
                });
            }

            services.AddSingleton(environment);
            return services;
        }

        public static void UseDefaultMonitoring(
            this IApplicationBuilder applicationBuilder,
            MonitoringEnvironment environment)
        {
            if (environment.UsePrometheus)
            {
                applicationBuilder.UseOpenTelemetryPrometheusScrapingEndpoint();
            }
        }
    }
}
