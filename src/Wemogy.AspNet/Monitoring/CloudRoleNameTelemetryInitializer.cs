using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Wemogy.AspNet.Monitoring
{
    public class CloudRoleNameTelemetryInitializer : ITelemetryInitializer
    {
        readonly string roleName;

        public CloudRoleNameTelemetryInitializer(string roleName)
        {
            this.roleName = roleName;
        }

        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Cloud.RoleName = roleName;
        }
    }
}
