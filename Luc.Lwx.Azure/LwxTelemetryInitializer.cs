using Luc.Lwx.LwxConfig;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Luc.Lwx.Azure;

internal sealed class LwxTelemetryInitializer
(
    IConfiguration configuration
) : ITelemetryInitializer    
{
    private readonly string _roleName = configuration.LwxGet<string>
    (
        "ApplicationInsights:RoleName",
        isRequired: true
    );

    public void Initialize(ITelemetry telemetry)
    {
        telemetry.Context.GlobalProperties["ApplicationToken"] = _roleName;        
    }
}