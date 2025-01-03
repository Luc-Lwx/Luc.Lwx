using System.Text.Json;
using Luc.Lwx.Interface;

namespace Luc.Lwx.LwxActivityLog;

public static class LwxActivityLogSetupExtensions
{
    internal static IApplicationBuilder LwxConfigureActivityLog(this IApplicationBuilder app)
    {
        return app.UseMiddleware<LwxActivityLogMiddleware>();
    }

    internal static IHostApplicationBuilder LwxConfigureActivityLog(this IHostApplicationBuilder builder)
    {        
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
     
        var configSection = builder.Configuration.GetSection("LwxActivityLog") ?? throw new LwxConfigException("LwxActivityLog section is missing in the configuration file.");
        try
        {
            var config = configSection.Get<LwxActivityLogConfig>() ?? throw new LwxConfigException(GenerateConfigErrorMessage("Invalid format for LwxActivityLog section in the configuration file."));
            builder.Services.AddSingleton(config);            
        }
        catch (Exception ex)
        {
            throw new LwxConfigException(GenerateConfigErrorMessage("Error reading LwxActivityLog configuration section"), ex);
        }
        
        return builder;
    }

    public static IHostApplicationBuilder LwxConfigureActivityLogOutput(this IHostApplicationBuilder builder, ILwxActivityLogOutput output)
    {        
        builder.Services.AddTransient<LwxActivityLogMiddleware>();   
        builder.Services.AddSingleton<ILwxActivityLogOutput>(output);           
        return builder;
    }

    private static string GenerateConfigErrorMessage(string message)
    {
        return $$"""


            {{message}}

            The LwxActivityLog should be configured in appsettings.json file like:

            "LwxActivityLog": 
            {
                "FixIpAddr": true,
                "IgnoreEndpointsWithoutAttribute": true,
                "ErrorHandler": true
            }

            If you are using kubernetes, and needs to override the config values, you can use the following environment variables:

                LwxActivityLog__FixIpAddr=true
                LwxActivityLog__IgnoreEndpointsWithoutAttribute=true
                LwxActivityLog__ErrorHandler=true

            This is usually in a configmap insice an yaml.

            """;
    }
}

