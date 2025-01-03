using System.Text.Json.Serialization.Metadata;
using Microsoft.AspNetCore.Http.Json;

namespace Luc.Lwx.LwxJsonOptions;

public static class LwxJsonTypeResolverExtension
{
    public static IHostApplicationBuilder LwxAddJsonTypeResolvers
    (
        this IHostApplicationBuilder builder, 
        params IJsonTypeInfoResolver[] options
    )
    {
        var combined = JsonTypeInfoResolver.Combine(options);

        builder.Services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.TypeInfoResolver = combined;
        });

        return builder;
    }
}