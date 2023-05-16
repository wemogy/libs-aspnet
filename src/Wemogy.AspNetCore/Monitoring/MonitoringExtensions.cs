using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;

namespace Wemogy.AspNetCore.Monitoring
{
    public static class MonitoringExtensions
    {
        public static IServiceCollection AddDefaultMonitoring(
            this IServiceCollection services,
            MonitoringEnvironment environment
        )
        {
            if (environment.UseApplicationInsights)
            {
                services.AddApplicationInsightsTelemetry(x => x.ConnectionString = environment.ApplicationInsightsConnectionString);
                services.AddSingleton<ITelemetryInitializer>(new CloudRoleNameTelemetryInitializer(environment.ServiceName));
            }

            services.AddSingleton(environment);
            services.AddSingleton<IMonitoringService, MonitoringService>();
            return services;
        }

        public static void UseDefaultMonitoring(
            this IApplicationBuilder applicationBuilder,
            MonitoringEnvironment environment
        )
        {
            if (environment.UsePrometheus)
            {
                applicationBuilder.UseMetricServer();
                applicationBuilder.UseHttpMetrics();
            }
        }
    }
}

