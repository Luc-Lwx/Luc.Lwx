using System.Reflection;
using System.Diagnostics;
using Luc.Lwx.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Luc.Lwx.LwxConfig;

public static class LwxConfigExtension
{
    public static WebApplicationBuilder RequireLwxDevConfig(this WebApplicationBuilder builder)
    {
        return builder.RequireLwxDevConfig(Assembly.GetCallingAssembly());
    }
    public static WebApplicationBuilder RequireLwxDevConfig(this WebApplicationBuilder builder, Assembly assembly)
    {            
        switch(builder.Environment.EnvironmentName) 
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


    public static bool IsTestEnvironment(this IHostEnvironment environment)
    {
        return environment.EnvironmentName == "Test";
    }
}

