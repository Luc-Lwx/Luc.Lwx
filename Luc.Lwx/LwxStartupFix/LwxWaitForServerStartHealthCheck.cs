using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Luc.Lwx.LwxStartupFix;

/// <summary>
/// Custom health check that waits for the application to start.
/// </summary>
internal class LwxStartupHealthCheck(TaskCompletionSource<bool> appStartedTcs) : IHealthCheck
{
    private readonly TaskCompletionSource<bool> _appStartedTcs = appStartedTcs;

    /// <summary>
    /// Checks the health of the application.
    /// This mechanism holds the response to the health check probe until the server started event,
    /// preventing Kubernetes from sending load to this machine before it is really ready.
    /// </summary>
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        await _appStartedTcs.Task;
        return HealthCheckResult.Healthy("App is started");
    }
}
