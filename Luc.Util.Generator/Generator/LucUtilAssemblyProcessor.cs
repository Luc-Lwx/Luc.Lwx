using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Luc.Util.Generator;

internal partial class LucUtilAssemblyProcessor
{
    
    public SourceProductionContext Context { get; private set; }
    public ImmutableArray<GeneratorSyntaxContext> TypeSymbols { get; private set; }
    public string ProjectDir { get; private set; }
    public AppSettingsLayout? AppSettings { get; private set; }
    private readonly string? _assemblyName;

    public Dictionary<string,List<string>> GeneratedSrcEndpointMappings { get; internal set; } = [];
    public Dictionary<string,List<string>> GeneratedSrcAuthPolicyMappings { get; internal set; } = [];
    public Dictionary<string,List<string>> GeneratedSrcAuthSchemeMappings { get; internal set; } = [];

    public void AddGeneratedSrc_EndpointMappingMethod(string group, string methodSrc)
    {
        var groupMap = GeneratedSrcEndpointMappings.GetValueOrDefault(group);
        if( groupMap == null )
        {
            groupMap = [];
            GeneratedSrcEndpointMappings.Add(group, groupMap);
        }
        groupMap.Add(methodSrc);        
    }

    public void AddGeneratedSrc_AuthPolicyMappingMethod(string group, string methodSrc)
    {
        var groupMap = GeneratedSrcAuthPolicyMappings.GetValueOrDefault(group);
        if( groupMap == null )
        {
            groupMap = [];
            GeneratedSrcAuthPolicyMappings.Add(group, groupMap);
        }
        groupMap.Add(methodSrc);        
    }

    public void AddGeneratedSrc_AuthSchemeMappingMethod(string group, string methodSrc)
    {
        var groupMap = GeneratedSrcAuthSchemeMappings.GetValueOrDefault(group);
        if( groupMap == null )
        {
            groupMap = [];
            GeneratedSrcAuthSchemeMappings.Add(group, groupMap);
        }
        groupMap.Add(methodSrc);        
    }


    public LucUtilAssemblyProcessor
    (
        SourceProductionContext sourceProductionContext, 
        ImmutableArray<string?> appSettingsFiles, 
        ImmutableArray<GeneratorSyntaxContext> typeSymbols 
    ) 
    { 
        Context = sourceProductionContext;
        TypeSymbols = typeSymbols;

        AppSettingsLayout? appSettings = null;            
        try 
        {
            var appSettingsText = appSettingsFiles.FirstOrDefault();
            if( appSettingsText != null )
            {
                appSettings = JsonSerializer.Deserialize<AppSettingsLayout>(appSettingsText);
            }
        }
        catch( Exception ex )
        {
            ReportWarning
            ( 
                msgSeverity: DiagnosticSeverity.Error, 
                msgId: "LUC0911", 
                msgFormat: $"""
                    Can't parse the appsettings.json file:

                    {ex.Message}
                    """, 
                srcLocation: null 
            );
        }     
        appSettings ??= new AppSettingsLayout();   
        appSettings.LucUtil ??= new AppSettingsSectionLayout();   
        AppSettings = appSettings;
        
        
        if(typeSymbols.FirstOrDefault().Node is TypeDeclarationSyntax firstType)
        {
            ProjectDir = Path.GetDirectoryName(firstType.SyntaxTree.FilePath) ?? throw new InvalidOperationException("Project directory not found");
        }
        else
        {
            throw new InvalidOperationException("Project directory not found");
        }               

        if( typeSymbols.Length != 0 )
        {
            _assemblyName = typeSymbols[0].SemanticModel.Compilation.AssemblyName ?? throw new InvalidOperationException("Assembly name not found");                        
        }                
    }    

    public void Execute() 
    {
        foreach ( var typeSymbol in TypeSymbols )
        {            
            try 
            {
                var processClass = new LucUtilTypeProcessor(this, typeSymbol);                
                processClass.ExecutePhase1();                       
            }
            catch( Exception ex )
            {
                ReportWarning
                ( 
                    msgSeverity: DiagnosticSeverity.Error, 
                    msgId: "LUC0912", 
                    msgFormat: $"""
                        The LucUtilGenerator threw an exception: 
                        
                        {ex.Message},
                        {ex.StackTrace}
                        """, 
                    srcLocation: typeSymbol.Node.GetLocation() 
                );
            }    
        }

       
        GenerateEndpointMappings();
        GenerateAuthPolicyMappings();
        GenerateAuthSchemeMappings();
          
    }

    private void GenerateEndpointMappings() 
    {
        var generatedEndpointMappings = new StringBuilder();

        foreach( var group in GeneratedSrcEndpointMappings )
        {
            var generatedMethodBody = new StringBuilder();
            foreach (var methodSrc in group.Value)
            {
                generatedMethodBody.Append(methodSrc);
            }

            generatedEndpointMappings.Append($$"""
                
                    public static void {{group.Key}}(this IEndpointRouteBuilder app)
                    {
                        {{generatedMethodBody}}
                    }
                    
                """);
        }         
    
        var srcBuilder = new StringBuilder();
        srcBuilder.AppendLine($$"""
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Threading.Tasks;
            using Microsoft.AspNetCore.Http;
            using Microsoft.AspNetCore.Mvc;
            using Microsoft.Extensions.Logging;                 
            using Microsoft.AspNetCore.Builder;
            using Microsoft.AspNetCore.Routing;
            using Microsoft.AspNetCore.Authorization;
        
            namespace {{_assemblyName}}.Generated;

            public static class GeneratedEndpointMappings
            {
                {{generatedEndpointMappings}}
            }
            """);

        Context.AddSource("GeneratedEndpointMappings.g.cs", srcBuilder.ToString()); 
    }

    private void GenerateAuthPolicyMappings() 
    {
        var generatedAuthPolicyMappings = new StringBuilder();

        foreach( var group in GeneratedSrcAuthPolicyMappings )
        {
            var generatedMethodBody = new StringBuilder();
            foreach (var methodSrc in group.Value)
            {
                generatedMethodBody.Append(methodSrc);
            }

            generatedMethodBody.Clear();

            generatedAuthPolicyMappings.Append($$"""
                
                    public static void {{group.Key}}(this WebApplicationBuilder builder)
                    {
                        builder.Services.{{group.Key}}();
                    }

                    public static void {{group.Key}}(this IServiceCollection services)
                    {
                        services.AddAuthorization( options => 
                        {
                            {{generatedMethodBody}}
                        });
                    }
                    
                """);
        }         
     
        var srcBuilder = new StringBuilder();
        srcBuilder.AppendLine($$"""
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Threading.Tasks;                        
        
            namespace {{_assemblyName}}.Generated;

            public static class GeneratedAuthPolicyMappings
            {
                {{generatedAuthPolicyMappings}}
            }
            """);

        Context.AddSource("GeneratedAuthPolicyMappings.g.cs", srcBuilder.ToString()); 
    }


    private void GenerateAuthSchemeMappings() 
    {
        var generatedAuthSchemeMappings = new StringBuilder();

        foreach( var group in GeneratedSrcAuthSchemeMappings )
        {
            var generatedMethodBody = new StringBuilder();
            foreach (var methodSrc in group.Value)
            {
                generatedMethodBody.Append(methodSrc);
            }

            generatedMethodBody.Clear();

            generatedAuthSchemeMappings.Append($$"""
                
                    public static void {{group.Key}}(this WebApplicationBuilder builder)
                    {
                        builder.Services.{{group.Key}}();
                    }

                    public static void {{group.Key}}(this IServiceCollection services)
                    {
                        // pega o singleton LucFramwork
                        var framework = services.Any(serviceDescriptor => serviceDescriptor.ServiceType == typeof(LucFramework)).ImplementationInstance as LucFramework;
                        if( framework == null )
                        {
                            services.AddSingleton<LucFramework>(new LucFramework());
                        }   
                        if( !framework.IsAuthSchemeInititlized )
                        {
                            services.AddAuthentication("Unauthenticated")
                                .AddScheme<AuthenticationSchemeOptions, CustomAuthenticationHandler>("CustomScheme", options => { });


                            services.AddAuthentication( options => 
                            {
                                options.DefaultAuthenticateScheme = "Unauthenticated";
                                options.DefaultChallengeScheme = "Unauthenticated";                                                           
                            });

                            framework.IsAuthSchemeInititlized = true;
                        }

                        {{generatedMethodBody}}
                    }
                    
                """);
        }         
     
        var srcBuilder = new StringBuilder();
        srcBuilder.AppendLine($$"""
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Threading.Tasks;                        
        
            namespace {{_assemblyName}}.Generated;

            public static class GeneratedAuthPolicyMappings
            {
                {{generatedAuthSchemeMappings}}
            }
            """);

        Context.AddSource("GeneratedAuthSchemeMappings.g.cs", srcBuilder.ToString()); 
    }


    internal void ReportWarning( DiagnosticSeverity msgSeverity, string msgId, string msgFormat, Location? srcLocation, params object[] msgArgs ) 
    {
        Context.ReportDiagnostic
        ( 
            Diagnostic.Create
            (
                new DiagnosticDescriptor
                (
                    id: msgId,
                    title: msgFormat,
                    messageFormat: msgFormat,
                    category: LucUtilGenerator.LucEndpointCategory,
                    msgSeverity,
                    isEnabledByDefault: true
                ), 
                srcLocation, 
                msgArgs
            )
        );
    }
}
