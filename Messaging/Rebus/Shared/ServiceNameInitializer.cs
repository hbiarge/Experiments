using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Shared
{
    public class ServiceNameInitializer : ITelemetryInitializer
    {
        private readonly string _roleName;

        public ServiceNameInitializer(string roleName)
        {
            _roleName = roleName;
        }

        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Cloud.RoleName = _roleName;
        }
    }
}
