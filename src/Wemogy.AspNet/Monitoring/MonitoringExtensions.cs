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

            return services.AddDefaultMonitoring(options.MonitoringEnvironment);
        }

        public static IServiceCollection AddDefaultMonitoring(
            this IServiceCollection services,
            MonitoringEnvironment monitoringEnvironment)
        {
            // Metrics
            services.AddOpenTelemetry().WithMetrics(builder =>
            {
                foreach (var meterName in monitoringEnvironment.MeterNames)
                {
                    builder.AddMeter(meterName);
                }

                builder.AddRuntimeInstrumentation();
                builder.AddHttpClientInstrumentation();
                builder.AddAspNetCoreInstrumentation();

                if (monitoringEnvironment.UsePrometheus)
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
                        serviceName: monitoringEnvironment.ServiceName,
                        serviceNamespace: monitoringEnvironment.ServiceNamespace,
                        serviceInstanceId: monitoringEnvironment.ServiceInstanceId,
                        serviceVersion: monitoringEnvironment.ServiceVersion);
                });

                builder.AddAspNetCoreInstrumentation();
                builder.AddEntityFrameworkCoreInstrumentation();

                foreach (var activitySourceName in monitoringEnvironment.ActivitySourceNames)
                {
                    builder.AddSource(activitySourceName);
                }

                if (monitoringEnvironment.UseOtlpExporter)
                {
                    builder.AddOtlpExporter(oltpOptions =>
                    {
                        oltpOptions.Endpoint = monitoringEnvironment.OtlpExportEndpoint;
                    });
                }
            });

            // Azure
            if (monitoringEnvironment.UseApplicationInsights)
            {
                services.AddOpenTelemetry().UseAzureMonitor(azureMonitorOptions =>
                {
                    azureMonitorOptions.ConnectionString = monitoringEnvironment.ApplicationInsightsConnectionString;
                    azureMonitorOptions.SamplingRatio = monitoringEnvironment.ApplicationInsightsSamplingRatio;
                });
            }

            services.AddSingleton(monitoringEnvironment);
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

            applicationBuilder.UseDefaultMonitoring(options.MonitoringEnvironment);
        }

        public static void UseDefaultMonitoring(
            this IApplicationBuilder applicationBuilder,
            MonitoringEnvironment monitoringEnvironment)
        {
            if (monitoringEnvironment.UsePrometheus)
            {
                applicationBuilder.UseOpenTelemetryPrometheusScrapingEndpoint();
            }
        }
    }
}
