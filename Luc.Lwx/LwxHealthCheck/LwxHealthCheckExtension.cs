using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;

namespace Luc.Lwx.LwxHealthCheck
{
    /// <summary>
    /// Extension methods for adding and using LwxHealthCheck.
    /// </summary>
    public static class LwxHealthCheckExtension
    {
        private static readonly TaskCompletionSource<bool> _appStartedTcs = new();

        /// <summary>
        /// Registers the application started event to signal the health check.
        /// This mechanism holds the response to the health check probe until the server started event,
        /// preventing Kubernetes from sending load to this machine before it is really ready.
        /// Example usage:
        /// <code>
        /// var builder = WebApplication.CreateBuilder(args);
        /// builder.AddLwxHealthCheck();
        /// var app = builder.Build();
        /// app.UseLwxHealthCheck();
        /// app.Run();
        /// </code>
        /// </summary>
        public static IApplicationBuilder UseLwxHealthCheck(this IApplicationBuilder app)
        {            
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            lifetime.ApplicationStarted.Register(() => _appStartedTcs.TrySetResult(true));
            return app;
        }

        /// <summary>
        /// Adds the LwxHealthCheck to the health checks.
        /// This method ensures that the health check will wait for the application to start before reporting a healthy status,
        /// preventing Kubernetes from sending load to this machine before it is really ready.
        /// Example usage:
        /// <code>
        /// var builder = WebApplication.CreateBuilder(args);
        /// builder.AddLwxHealthCheck();
        /// var app = builder.Build();
        /// app.UseLwxHealthCheck();
        /// app.Run();
        /// </code>
        /// </summary>
        public static WebApplicationBuilder AddLwxHealthCheck(this WebApplicationBuilder builder)
        {
            builder.Services.AddHealthChecks()
                .AddCheck("LwxHealthCheck", new LwxHealthCheck(_appStartedTcs));
            return builder;
        }
    }

    /// <summary>
    /// Custom health check that waits for the application to start.
    /// </summary>
    public class LwxHealthCheck : IHealthCheck
    {
        private readonly TaskCompletionSource<bool> _appStartedTcs;

        /// <summary>
        /// Initializes a new instance of the <see cref="LwxHealthCheck"/> class.
        /// </summary>
        public LwxHealthCheck(TaskCompletionSource<bool> appStartedTcs)
        {
            _appStartedTcs = appStartedTcs;
        }

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
}
