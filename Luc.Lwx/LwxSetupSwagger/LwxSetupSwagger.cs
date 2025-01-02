using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Luc.Lwx.LwxConfig;
using System.Linq;
using System.Configuration.Assemblies;
using System.Security.Cryptography;
using System.Text;
using Luc.Lwx.Util;

namespace Luc.Lwx.LwxSetupSwagger;

public static partial class SwaggerSetup
{
    public static void LwxConfigureSwagger
    (
        this WebApplicationBuilder builder, 
        string? title = null, 
        string? description = null, 
        string? contactEmail = null, 
        string? author = null, 
        string? version = "v1", 
        string[]? additionalUrls = null
    )
    {
        if (title != null) builder.Configuration["Lwx:SwaggerTitle"] = title;
        if (description != null) builder.Configuration["Lwx:SwaggerDescription"] = description;
        if (contactEmail != null) builder.Configuration["Lwx:SwaggerContactEmail"] = contactEmail;
        if (author != null) builder.Configuration["Lwx:SwaggerAuthor"] = author;
        if (version != null) builder.Configuration["Lwx:SwaggerVersion"] = version;
        if (additionalUrls != null) builder.Configuration["Lwx:SwaggerAdditionalUrls"] = string.Join(";", additionalUrls);

        var swaggerDescription = builder.Configuration.LwxGetConfig(
            "Lwx:SwaggerDescription",            
            defaultValue: ""
        );
        var swaggerTitle = builder.Configuration.LwxGetConfig(
            "Lwx:SwaggerTitle",
            defaultValue: ""
        );
        var swaggerContactEmail = builder.Configuration.LwxGetConfig(
            "Lwx:SwaggerContactEmail",
            defaultValue: ""
        );
        var swaggerAuthor = builder.Configuration.LwxGetConfig(
            "Lwx:SwaggerAuthor",
            defaultValue: ""
        );
        var swaggerVersion = builder.Configuration.LwxGetConfig(
            "Lwx:SwaggerVersion",
            defaultValue: "v1"
        );
        var additionalUrlsConfig = builder.Configuration.LwxGetConfig(
            "Lwx:SwaggerAdditionalUrls",
            defaultValue: null
        );

        builder.Configuration.LwxValidateKeys("AppSettings:Lwx", new[] { 
            "SwaggerDescription", "SwaggerTitle", "SwaggerContactEmail", "SwaggerAuthor", "SwaggerVersion", "SwaggerAdditionalUrls" });

        string[] serverUrls = [
            .. (builder.WebHost?.GetSetting("urls")?.Split(";") ?? []), 
            .. (additionalUrls ?? additionalUrlsConfig?.Split(";") ?? [])
        ];

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(swaggerVersion, new OpenApiInfo
            {
                Title = swaggerTitle,
                Version = swaggerVersion,                    
                Description = swaggerDescription,
                Contact = new OpenApiContact
                {
                    Email = swaggerContactEmail,
                    Name = swaggerAuthor
                }
            });
            
            foreach (var url in serverUrls ) 
            {
                c.AddServer(new OpenApiServer
                {
                    Url = url,
                    Description = "API Server URL"
                });
            }

            c.InferSecuritySchemes();                

            c.CustomSchemaIds(GetSchemaId);
        });
    }

    
   

    public static void LwxConfigureSwagger(this IApplicationBuilder app)
    {
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            c.DefaultModelsExpandDepth(-1); 
            c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); 
            c.EnableTryItOutByDefault(); 
        });
    }  

    private static string GetSchemaId(Type type)
    {   
        var result = type.FullName;
        if (result == null)
        {
            return "t" + Guid.NewGuid().ToString();
        }
        var assemblyName = type.Assembly?.GetName()?.Name;
        if (assemblyName != null)
        {            
            result = result[(assemblyName.Length + 1)..];
            var hash = assemblyName.LwxHashSha1Hex()[..8];
            result = $"n{hash}.{result}";
        }
        return result.Replace('.', '_').Replace('+', '_');
    }
}
