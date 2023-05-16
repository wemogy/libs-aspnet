using Microsoft.ApplicationInsights;
using Prometheus;

namespace Wemogy.AspNetCore.Monitoring
{
    public class MonitoringService : IMonitoringService
    {
        readonly TelemetryClient _appInsightsTelemetryClient;
        readonly MonitoringEnvironment _env;

        public MonitoringService(TelemetryClient appInsightsTelemetryClient, MonitoringEnvironment env)
        {
            _appInsightsTelemetryClient = appInsightsTelemetryClient;
            _env = env;
        }

        public void TrackEvent(string eventName, string eventDescription = "")
        {
            // Application Insights
            if (_env.UseApplicationInsights)
            {
                _appInsightsTelemetryClient.TrackEvent(eventName);
            }

            // Prometheus
            if (_env.UsePrometheus)
            {
                var counter = Metrics.CreateCounter(eventName, eventDescription);
                counter.Inc();
            }
        }
    }
}
