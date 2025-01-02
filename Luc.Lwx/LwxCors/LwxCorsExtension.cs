using Luc.Lwx.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Luc.Lwx.LwxCors;

/// <summary>
/// Provides extension methods for configuring CORS in a web application.
/// </summary>
public static class LwxCorsExtension
{
    private static readonly string[] SupportedMethods = ["GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS", "HEAD"];

    /// <summary>
    /// Configures CORS policies from the configuration section "LwxCors".
    /// <example>
    /// Example appsettings.json configuration:
    /// <code>
    /// {
    ///   "LwxCors": {
    ///     "AllowedOrigins": "https://example.com,https://another.example.com",
    ///     "AllowedMethods": "GET, POST, PUT, DELETE",
    ///     "AllowedHeaders": "Content-Type, Authorization"
    ///   }
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public static WebApplicationBuilder LwxConfigureCors(this WebApplicationBuilder builder)
    {
        var corsSection = builder.Configuration.GetSection("LwxCors");
        if (!corsSection.Exists())
        {
            throw new LwxConfigException(GenerateExceptionText("LwxCors section is missing in appsettings.json."));
        }

        var allowedOrigins = corsSection.GetValue<string>("AllowedOrigins")?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray() ?? throw new LwxConfigException(GenerateExceptionText("AllowedOrigins is required in LwxCors section."));
        var allowedMethods = corsSection.GetValue<string>("AllowedMethods")?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray() ?? [];
        var allowedHeaders = corsSection.GetValue<string>("AllowedHeaders")?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray() ?? [];

        ValidateOrigins(allowedOrigins);
        ValidateMethods(allowedMethods);

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("LwxCorsPolicy", policy =>
            {
                policy.WithOrigins(allowedOrigins)
                      .WithMethods(allowedMethods)
                      .WithHeaders(allowedHeaders);
            });
        });

        return builder;
    }

    /// <summary>
    /// Validates that the provided origins are well-formed URIs.
    /// </summary>
    private static void ValidateOrigins(string[] origins)
    {
        foreach (var origin in origins)
        {
            if (!Uri.IsWellFormedUriString(origin, UriKind.Absolute))
            {
                throw new LwxConfigException(GenerateExceptionText($"Invalid URL in AllowedOrigins: {origin}"));
            }
        }
    }

    /// <summary>
    /// Validates that the provided methods are supported HTTP methods.
    /// </summary>
    private static void ValidateMethods(string[] methods)
    {
        foreach (var method in methods)
        {
            if (Array.IndexOf(SupportedMethods, method.ToUpper()) < 0)
            {
                throw new LwxConfigException(GenerateExceptionText($"Unsupported HTTP method in AllowedMethods: {method}"));
            }
        }
    }

    /// <summary>
    /// Generates the exception text with an example configuration.
    /// </summary>
    private static string GenerateExceptionText(string message)
    {
        return $$"""
        {{message}}

        Example configuration:
        {
            "LwxCors": 
            {
                "AllowedOrigins": "https://example.com, https://another.example.com",
                "AllowedMethods": "GET, POST, PUT, DELETE",
                "AllowedHeaders": "Content-Type, Authorization"
            }
        }

        In kubernetes you may want to override the values using the configmap:

            apiVersion: v1
            kind: ConfigMap
            metadata:
            name: my-config
              data:
                /publish/appsettings.json: |
                    {
                        "LwxCors": 
                        {
                            "AllowedOrigins": "https://example.com, https://another.example.com",
                            "AllowedMethods": "GET, POST, PUT, DELETE",
                            "AllowedHeaders": "Content-Type, Authorization"
                        }
                    }

        In kubernetes you may want to override the values using the configmap:

            apiVersion: v1
            kind: ConfigMap
            metadata:
              name: my-config
              data:        
                LwxCors__AllowedOrigins: https://example.com, https://another.example.com
                LwxCors__AllowedMethods: GET, POST, PUT, DELETE
                LwxCors__AllowedHeaders: Content-Type, Authorization

        """;
    }
}