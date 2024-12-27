using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;

namespace Luc.Lwx.LwxWaitForServerStartFix;

/// <summary>
/// Extension methods for adding and using LwxHealthCheck.
/// </summary>
public static class LwxWaitForServerStartFixExtension
{
    private static readonly TaskCompletionSource<bool> _appStartedTcs = new();

    /// <summary>
    /// 
    /// Prevents the server to respond to to the first health check probe before the application is started.
    /// 
    /// We had problem with Kubernetes scaling because the health check probe was returning healthy before the application was ready.
    /// 
    /// Example usage:
    /// 
    /// <code>
    /// var builder = WebApplication.CreateBuilder(args);
    /// builder.AddLwxHealthCheck();
    /// 
    /// var app = builder.Build();
    /// app.UseLwxHealthCheck();
    /// app.MapHealthChecks("/health");
    /// app.Run();
    /// 
    /// </code>
    /// </summary>
    public static IApplicationBuilder LwxConfigureHealhCheckFix(this IApplicationBuilder app)
    {            
        var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStarted.Register(() => _appStartedTcs.TrySetResult(true));
        return app;
    }

    /// <summary>
    /// 
    /// Prevents the server to respond to to the first health check probe before the application is started.
    /// 
    /// We had problem with Kubernetes scaling because the health check probe was returning healthy before the application was ready.
    /// 
    /// Example usage:
    /// 
    /// <code>
    /// var builder = WebApplication.CreateBuilder(args);
    /// builder.AddLwxHealthCheck();
    /// 
    /// var app = builder.Build();
    /// app.UseLwxHealthCheck();
    /// app.MapHealthChecks("/health");
    /// app.Run();
    /// 
    /// </code>        
    /// </summary>
    public static WebApplicationBuilder LwxConfigureHealthCheckFix(this WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            .AddCheck("LwxHealthCheck", new LwxWaitForServerStartHealthCheck(_appStartedTcs));
        return builder;
    }
}