using System.Reflection;
using System.Diagnostics;
using Luc.Lwx.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;
using Microsoft.IdentityModel.Tokens;

namespace Luc.Lwx.LwxConfig;


public static class LwxConfigExtension
{   
    /// <summary>
    /// Get a required configuration value and bind it to an object of type T with an additional explanation in the error message
    /// </summary>
    [SuppressMessage("Sonar","S2955")]
    [SuppressMessage("Sonar","S1192")]
    public static T LwxGet<T>(
        this IConfiguration configuration, 
        string keyName, 
        bool isRequired = true,
        string additionalErrorInfo = "",
        Func<IConfigurationSection, T>? converter = null,
        T? defaultValue = default
    )
    {
        var section = configuration.GetSection(keyName);
        
        string? errorMsg = null;

        T? obj = GetConfigValue
        (
            section, 
            converter: converter, 
            errorMsg: out errorMsg,
            defaultValue: defaultValue
        );

        if ((obj == null || (obj is string str && string.IsNullOrEmpty(str))) && isRequired)
        {
            errorMsg ??= "";
            throw new LwxConfigException($$"""
                            
                Configuration section for key '{{keyName}}' {{errorMsg}}

                {{additionalErrorInfo}}

                The configuration should be set in:

                    appsettings.json (for non-sensitive configuration)
                    
                    LwxDevConfig/appsettings-*.json during local development

                    The key Abc:Cde:Efg should be set on those files like:
                        
                    {
                        "Abc": {
                            "Cde": {
                                "Efg": "value"
                            }
                        }
                    }

                    After is set in the appsettings.json, for production environment can be set as environment variable:

                    ABC__CDE__EFG=value

                    In dotnet, two underscores are used to separate the levels of the configuration.

                """
            );
        }

        if (!string.IsNullOrEmpty(additionalErrorInfo) && EqualityComparer<T>.Default.Equals(obj, default(T)))
        {
            throw new LwxConfigException($"Failed to bind configuration section for key '{keyName}' to type '{typeof(T)}'. {additionalErrorInfo}");
        }
        return obj!;
    }

    [SuppressMessage("Sonar","S2955",Justification="Sonar is complaining that default(T) should be used in != null, but it is not the case")]
    private static T? GetConfigValue<T>(
        IConfigurationSection section, 
        Func<IConfigurationSection, T>? converter, 
        out string? errorMsg,
        T? defaultValue = default)
    {
        errorMsg = null;
        T? obj;

        if (section == null)
        {
            obj = default(T);
        }
        else if (converter == null)
        {
            try
            {
                obj = section.Get<T>();                
            }
            catch (Exception ex)
            {
                obj = default(T);
                errorMsg = $"is not in the appropriated format (msg: {ex.Message})";
            }
        }
        else
        {
            try
            {
                obj = converter(section);                
            }
            catch (Exception ex)
            {
                obj = default(T);
                errorMsg = $"is not in the appropriated format (msg: {ex.Message})";
            }
        }

        if( errorMsg == null && obj is string objStr && !objStr.IsNullOrEmpty() && defaultValue != null )
        {
            obj = defaultValue;
        }
        else if( errorMsg == null && obj == null && defaultValue != null)
        {
            obj = defaultValue;
        }
        else 
        {
            errorMsg = $"can't be null or empty";
        }

        return obj;
    }

    public static void LwxValidKeys(this IConfiguration configuration, string sectionPath, string[] validKeys)
    {
        var keys = configuration.GetSection(sectionPath).GetChildren().Select(c => c.Key).ToList();

        foreach (var key in keys)
        {
            if (!validKeys.Contains(key))
            {
                throw new LwxConfigException($"Invalid configuration key: {key}");
            }
        }
    }

    public static bool IsTestEnvironment(this IHostEnvironment environment)
    {
        return environment.EnvironmentName == "Test";
    }
}

