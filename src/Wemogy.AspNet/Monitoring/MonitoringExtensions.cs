using System;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Wemogy.AspNet.Startup;
using Wemogy.Core.Monitoring;

namespace Wemogy.AspNet.Monitoring
{
    public static class MonitoringExtensions
    {
        public static IServiceCollection AddDefaultMonitoring(
            this IServiceCollection services,
            StartupOptions options)
        {
            if (options.MonitoringEnvironment == null)
            {
                throw new ArgumentException("MonitoringEnvironment is not configured.");
            }

            // Metrics
            services.AddOpenTelemetry().WithMetrics(builder =>
            {
                foreach (var meterName in options.MonitoringEnvironment.MeterNames)
                {
                    builder.AddMeter(meterName);
                }

                builder.AddRuntimeInstrumentation();
                builder.AddHttpClientInstrumentation();
                builder.AddAspNetCoreInstrumentation();

                if (options.MonitoringEnvironment.UsePrometheus)
                {
                    builder.AddPrometheusExporter();
                }
            });

            // Traces
            services.AddOpenTelemetry().WithTracing(builder =>
            {
                builder.ConfigureResource((resource) =>
                {
                    resource.AddService(
                        serviceName: options.MonitoringEnvironment.ServiceName,
                        serviceNamespace: options.MonitoringEnvironment.ServiceNamespace,
                        serviceInstanceId: options.MonitoringEnvironment.ServiceInstanceId,
                        serviceVersion: options.MonitoringEnvironment.ServiceVersion);
                });

                builder.AddAspNetCoreInstrumentation();
                builder.AddEntityFrameworkCoreInstrumentation();

                foreach (var activitySourceName in options.MonitoringEnvironment.ActivitySourceNames)
                {
                    builder.AddSource(activitySourceName);
                }

                if (options.MonitoringEnvironment.UseOtlpExporter)
                {
                    builder.AddOtlpExporter(oltpOptions =>
                    {
                        oltpOptions.Endpoint = options.MonitoringEnvironment.OtlpExportEndpoint;
                    });
                }
            });

            // Azure
            if (options.MonitoringEnvironment.UseApplicationInsights)
            {
                services.AddOpenTelemetry().UseAzureMonitor(azureMonitorOptions =>
                {
                    azureMonitorOptions.ConnectionString = options.MonitoringEnvironment.ApplicationInsightsConnectionString;
                    azureMonitorOptions.SamplingRatio = options.MonitoringEnvironment.ApplicationInsightsSamplingRatio;
                });
            }

            services.AddSingleton(options.MonitoringEnvironment);
            return services;
        }

        public static void UseDefaultMonitoring(
            this IApplicationBuilder applicationBuilder,
            StartupOptions options)
        {
            if (options.MonitoringEnvironment == null)
            {
                throw new ArgumentException("OpenApiEnvironment is not configured.");
            }

            if (options.MonitoringEnvironment.UsePrometheus)
            {
                applicationBuilder.UseOpenTelemetryPrometheusScrapingEndpoint();
            }
        }
    }
}
