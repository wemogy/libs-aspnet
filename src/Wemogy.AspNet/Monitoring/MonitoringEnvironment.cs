namespace Wemogy.AspNet.Monitoring
{
    public class MonitoringEnvironment
    {
        public string ServiceName { get; }
        public string ApplicationInsightsConnectionString { get; private set; }
        public bool UsePrometheus { get; private set; }
        public bool UseApplicationInsights => !string.IsNullOrEmpty(ApplicationInsightsConnectionString);

        public MonitoringEnvironment(string serviceName)
        {
            ServiceName = serviceName;
        }

        public MonitoringEnvironment WithApplicationInsights(string connectionString)
        {
            ApplicationInsightsConnectionString = connectionString;
            return this;
        }

        public MonitoringEnvironment WithPrometheus()
        {
            UsePrometheus = true;
            return this;
        }
    }
}
