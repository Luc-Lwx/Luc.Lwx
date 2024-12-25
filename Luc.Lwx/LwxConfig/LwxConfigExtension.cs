using System.Reflection;
using System.Diagnostics;
using Luc.Lwx.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace Luc.Lwx.LwxConfig;


public static class LwxConfigExtension
{
    public static WebApplicationBuilder RequireLwxDevConfig(this WebApplicationBuilder builder)
    {
        return builder.RequireLwxDevConfig(Assembly.GetCallingAssembly());
    }
    public static WebApplicationBuilder RequireLwxDevConfig(this WebApplicationBuilder builder, Assembly assembly)
    {
        switch (builder.Environment.EnvironmentName)
        {
            case "Development":
                ConfigureLwxDevConfigInternal(builder.Configuration, assembly, "appsettings-dev-");
                break;

            case "Test":
                ConfigureLwxDevConfigInternal(builder.Configuration, assembly, "test-");
                break;
        }
        return builder;
    }

    private static void ConfigureLwxDevConfigInternal(IConfigurationBuilder builder, Assembly assembly, string prefix)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var configDirectory = LocateConfigDirectory(currentDirectory);

        if (configDirectory != null)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly), "Assembly cannot be null.");
            }

            var assemblyName = assembly?.GetName()?.Name;
            if (string.IsNullOrEmpty(assemblyName))
            {
                throw new ArgumentException("Assembly name cannot be null or empty.", nameof(assembly));
            }

            LoadConfigFiles(builder, configDirectory, assemblyName, prefix);
        }
        else
        {
            var errorMessage = GetConfigNotFoundMessage(prefix);
            Debug.WriteLine(errorMessage);
            throw new LwxConfigException(errorMessage);
        }
    }

    private static void LoadConfigFiles(IConfigurationBuilder builder, string configDirectory, string assemblyName, string prefix)
    {
        string[] namespaceParts = assemblyName.Split('.');
        bool configFound = false;

        for (int i = namespaceParts.Length; i > 0; i--)
        {
            string partialNamespace = string.Join('.', namespaceParts, 0, i);
            string configFilePath = Path.Combine(configDirectory, $"{prefix}{partialNamespace}.json");
            try
            {
                if (File.Exists(configFilePath))
                {
                    Debug.WriteLine($"LWX: Looking for config file: {configFilePath} - FOUND");
                    builder.AddJsonFile(configFilePath, optional: true, reloadOnChange: true);
                    configFound = true;
                }
                else
                {
                    Debug.WriteLine($"LWX: Looking for config file: {configFilePath} - not found");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LWX: Error loading config file: {configFilePath} - {ex.Message}");
                throw new LwxConfigException($"Error loading config file: {configFilePath}", ex);
            }
        }

        if (!configFound)
        {
            throw new LwxConfigException(GetConfigNotFoundMessage(prefix));
        }
    }

    private static string? LocateConfigDirectory(string currentDirectory)
    {
        DirectoryInfo? directoryInfo = new DirectoryInfo(currentDirectory);

        while (directoryInfo != null)
        {
            string configDirectoryPath = Path.Combine(directoryInfo.FullName, "LwxDevConfig");
            Debug.WriteLine($"LWX: Looking for config directory: {configDirectoryPath}");
            if (Directory.Exists(configDirectoryPath))
            {
                Debug.WriteLine($"LWX: Found config dir: {configDirectoryPath}");
                return configDirectoryPath;
            }
            directoryInfo = directoryInfo.Parent;
        }

        return null;
    }

    private static string GetConfigNotFoundMessage(string prefix)
    {
        return $"""
            For the development enviroment, the configuration should be kept in the LwxDevConfig directory and marked ignored on .gitignore.

            On a larger deployment with many solutions you can also put the configuration in your development dir, like:

            /development/myTeam/LwxDevConfig
            /development/myTeam/MyCompany.MyProduct.MyModule1
            /development/myTeam/MyCompany.MyProduct.MyModule2
            /development/myTeam/MyCompany.MyProduct.MyModule3
            /development/myTeam/MyCompany.MyProduct.MyModule4

            The main purpose of that is to have a shared configuration for all the modules in the same unit.

            Configuration files are searched in the following order:
            1. From the current directory up, locate the directory 'LwxDevConfig'.
            2. Within the 'LwxDevConfig' directory, look for JSON files named '{prefix}<namespace>.json', 
            where <namespace> is the assembly namespace with one element removed each time.

            Example:
            For assembly 'MyCompany.MyProduct.MyModule', the following files are searched:
            - {prefix}MyCompany.MyProduct.MyModule.json
            - {prefix}MyCompany.MyProduct.json
            - {prefix}MyCompany.json

            Directory search example:
            Starting from the current directory, the search moves up the directory tree:
            - /current/directory/LwxDevConfig
            - /current/LwxDevConfig
            - /LwxDevConfig
            """;
    }


    /// <summary>
    /// Get a required string configuration with an additional explanation in the error message
    /// </summary>    
    [SuppressMessage("Sonar","S4136")]
    public static string? LwxGetConfig(this IConfiguration configuration, string keyName, string? isRequired = null, string? defaultValue = null)
    {
        var value = configuration[keyName] ?? defaultValue;
        if (!string.IsNullOrEmpty(isRequired) && string.IsNullOrEmpty(value))
        {
            
            
        }
        return value;
    }


    /// <summary>
    /// Get a required configuration value and bind it to an object of type T with an additional explanation in the error message
    /// </summary>
    public static T LwxGetConfig<T>(this IConfiguration configuration, string keyName, string? isRequired = null)
    {
        var section = configuration.GetSection(keyName);
        if (!string.IsNullOrEmpty(isRequired) && !section.Exists())
        {
            throw new LwxConfigException($$"""
            
                
                Configuration section for key '{{keyName}}' is missing or empty.

                {{isRequired}}

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
        var obj = section.Get<T>();
        if (!string.IsNullOrEmpty(isRequired) && EqualityComparer<T>.Default.Equals(obj, default(T)))
        {
            throw new LwxConfigException($"Failed to bind configuration section for key '{keyName}' to type '{typeof(T)}'. {isRequired}");
        }
        return obj!;
    }

    /// <summary>
    /// Get a required configuration value and deserialize it into an object of type T
    /// </summary>
    public static T LwxGetConfigObject<T>(this IConfiguration configuration, string keyName, string? isRequired = null)
    {
        var section = configuration.GetSection(keyName);
        if (!string.IsNullOrEmpty(isRequired) && !section.Exists())
        {
            throw new LwxConfigException($$"""
                
                Configuration section for key '{keyName}' is missing or empty.

                {{isRequired}}

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
        var obj = section.Get<T>();
        if (!string.IsNullOrEmpty(isRequired) && EqualityComparer<T>.Default.Equals(obj, default(T)))
        {
            throw new LwxConfigException($"Failed to bind configuration section for key '{keyName}' to type '{typeof(T)}'. {isRequired}");
        }
        return obj!;
    }

    /// <summary>
    /// Get a required string configuration with an additional explanation in the error message
    /// </summary>    
    public static string? LwxGetConfig(this IServiceProvider serviceProvider, string keyName, string? isRequired = null, string? defaultValue = null)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        return configuration.LwxGetConfig(keyName, isRequired, defaultValue);
    }

    /// <summary>
    /// Get a required configuration value and bind it to an object of type T with an additional explanation in the error message
    /// </summary>
    public static T LwxGetConfig<T>(this IServiceProvider serviceProvider, string keyName, string? isRequired = null)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        return configuration.LwxGetConfig<T>(keyName, isRequired);
    }


    /// <summary>
    /// Get a required string configuration with an additional explanation in the error message
    /// </summary>    
    public static string? LwxGetConfig(this IApplicationBuilder app, string keyName, string? isRequired = null, string? defaultValue = null)
    {
        var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
        return configuration.LwxGetConfig(keyName, isRequired, defaultValue);
    }

    /// <summary>
    /// Get a required configuration value and bind it to an object of type T with an additional explanation in the error message
    /// </summary>
    public static T LwxGetConfig<T>(this IApplicationBuilder app, string keyName, string? isRequired = null)
    {
        var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
        return configuration.LwxGetConfig<T>(keyName, isRequired);
    }

    /// <summary>
    /// Get a required string configuration with an additional explanation in the error message
    /// </summary>    
    public static string? GetConfig(this WebApplicationBuilder builder, string keyName, string? isRequired = null, string? defaultValue = null)
    {
        return builder.Configuration.LwxGetConfig(keyName, isRequired, defaultValue);
    }

    /// <summary>
    /// Get a required configuration value and bind it to an object of type T with an additional explanation in the error message
    /// </summary>
    public static T GetConfig<T>(this WebApplicationBuilder builder, string keyName, string? isRequired = null)
    {
        return builder.Configuration.LwxGetConfig<T>(keyName, isRequired);
    }

    public static bool IsTestEnvironment(this IHostEnvironment environment)
    {
        return environment.EnvironmentName == "Test";
    }

    public static void LwxValidateKeys(this IConfiguration configuration, string sectionPath, string[] validKeys)
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
}

