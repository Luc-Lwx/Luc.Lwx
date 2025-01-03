using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Luc.Lwx.Generator;

[SuppressMessage("","S101")]
internal partial class LwxGenerator_Assembly
{  
    public SourceProductionContext Context { get; private set; }
    public ImmutableArray<GeneratorSyntaxContext> TypeSymbols { get; private set; }
    public string ProjectDir { get; private set; }
    public AppSettingsDto? AppSettings { get; private set; }
    private readonly string? _assemblyName;

    public Dictionary<string,List<LwxGenerator_Method_Endpoint>> EndpointMappingMethods { get; internal set; } = [];
    public Dictionary<string,List<LwxGenerator_Method_AuthPolicy>> PolicyTypes { get; internal set; } = [];
    public Dictionary<string,List<LwxGenerator_Method_AuthScheme>> SchemeTypes { get; internal set; } = [];
    
    public string? ApiManagerPath { get; set; } = null;
    public string? SwaggerTitle { get; set; } = null;
    public string? SwaggerDescription { get; set; } = null;
    public string? SwaggerContactEmail { get; set; } = null;
    public string? SwaggerContactPhone { get; set; } = null;
    public string? SwaggerAuthor { get; set; } = null;

    public void AddEndpointMappingMethod(string group, LwxGenerator_Method_Endpoint methodSrc)
    {
        List<LwxGenerator_Method_Endpoint> groupMap;
        if( EndpointMappingMethods.ContainsKey(group) )
        {
            groupMap = EndpointMappingMethods[group];
        }
        else
        {
            groupMap = [];
            EndpointMappingMethods.Add(group, groupMap);
        }
        groupMap.Add(methodSrc);        
    }

    public void AddPolicyType(string group, LwxGenerator_Method_AuthPolicy processor )
    {
        List<LwxGenerator_Method_AuthPolicy> groupMap;
        if( PolicyTypes.ContainsKey(group) )
        {
            groupMap = PolicyTypes[group];
        }
        else
        {
            groupMap = [];
            PolicyTypes.Add(group, groupMap);
        }
        groupMap.Add(processor);        
    }

    public void AddSchemeType(string group, LwxGenerator_Method_AuthScheme processor )
    {
        List<LwxGenerator_Method_AuthScheme> groupMap;
        if( SchemeTypes.ContainsKey(group) )
        {
            groupMap = SchemeTypes[group];
        }
        else
        {
            groupMap = [];
            SchemeTypes.Add(group, groupMap);
        }
        groupMap.Add(processor);        
    }



    public LwxGenerator_Assembly
    (
        SourceProductionContext sourceProductionContext, 
        ImmutableArray<string?> appSettingsFiles, 
        ImmutableArray<GeneratorSyntaxContext> typeSymbols
    ) 
    { 
        Context = sourceProductionContext;
        TypeSymbols = typeSymbols;
     
        AppSettingsDto? appSettings = null;            
        try 
        {
            var appSettingsText = appSettingsFiles.FirstOrDefault();
            if( appSettingsText != null )
            {
                appSettings = JsonSerializer.Deserialize<AppSettingsDto>(appSettingsText);
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
        appSettings ??= new AppSettingsDto();   
        appSettings.Lwx ??= new AppSettingsSectionDto();   
        AppSettings = appSettings;

        ApiManagerPath = AppSettings.Lwx?.ApiManagerPath;
        SwaggerTitle = AppSettings.Lwx?.SwaggerTitle;
        SwaggerDescription = AppSettings.Lwx?.SwaggerDescription;
        SwaggerContactEmail = AppSettings.Lwx?.SwaggerContactEmail;
        SwaggerContactPhone = AppSettings.Lwx?.SwaggerContactPhone;
        SwaggerAuthor = AppSettings.Lwx?.SwaggerAuthor;
        
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
                var processClass = new LwxGenerator_Type(this, typeSymbol);                
                processClass.DoProccess();                       
            }
            catch( Exception ex )
            {
                ReportWarning
                ( 
                    msgSeverity: DiagnosticSeverity.Error, 
                    msgId: "LUC0912", 
                    msgFormat: $"""
                        LwxGenerator Exception: {ex.Message}, {ex.StackTrace?.Replace("\n", " ")?.Replace("\r", " ")}
                        """, 
                    srcLocation: typeSymbol.Node.GetLocation() 
                );
            }    
        }

        try
        {
            GenerateEndpointMappings();
            GenerateAuthPolicyMappings();
            GenerateAuthSchemeMappings();

        }
        catch( Exception e )
        {
            ReportWarning
            ( 
                msgSeverity: DiagnosticSeverity.Error, 
                msgId: "LUC0914", 
                msgFormat: $"""
                    Unexpected exception: {e.Message} {e.StackTrace?.Replace("\n", " ")}
                    """, 
                srcLocation: null 
            );
        }
    }

    private void GenerateEndpointMappings() 
    {
        var srcMethods = new StringBuilder();

        foreach( var group in EndpointMappingMethods )
        {
            var srcMethodBody = new StringBuilder();
            foreach (var methodSrc in group.Value)
            {
                srcMethodBody.Append(methodSrc.EndpointSrcMethodBody);
            }

            srcMethods.Append($$"""
                
                        public static void {{group.Key}}(this IEndpointRouteBuilder app)
                        {
                            {{srcMethodBody}}
                        }
                    
                """);
        }         
    
        var srcExtension = new StringBuilder();
        srcExtension.AppendLine($$"""
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
        
            namespace {{_assemblyName}}.Generated
            {
                public static class GeneratedEndpointMappings
                {
                    {{srcMethods}}
                }
            }
            """);

        Context.AddSource("GeneratedEndpointMappings.g.cs", srcExtension.ToString()); 
    }

    private void GenerateAuthPolicyMappings() 
    {
        var srcMethods = new StringBuilder();
        var srcOthers = new StringBuilder();

        foreach( var group in PolicyTypes )
        {
            var srcMethodBody = new StringBuilder();
            foreach (var method in group.Value)
            {
                srcMethodBody.Append(method.AuthPolicySrcMethodBody);
                srcOthers.Append(method.AuthPolicySrcIdClass);
            }

            srcMethods.Append($$"""
                
                    public static void {{group.Key}}(this IHostApplicationBuilder builder)
                    {
                        builder.Services.{{group.Key}}();
                    }

                    public static void {{group.Key}}(this IServiceCollection services)
                    {
                        services.AddAuthorization( options => 
                        {
                            {{srcMethodBody}}
                        });
                    }
                    
                """);
        }         
     
        var srcExtension = new StringBuilder();
        srcExtension.AppendLine($$"""
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Threading.Tasks;                        
        
            namespace {{_assemblyName}}.Generated 
            {
                public static class GeneratedAuthPolicyMappings
                {
                    {{srcMethods}}
                }
            }

            {{srcOthers}}
            """);

        Context.AddSource("GeneratedAuthPolicyMappings.g.cs", srcExtension.ToString()); 
    }


    private void GenerateAuthSchemeMappings() 
    {        
        var srcMethods = new StringBuilder();
        var srcOthers = new StringBuilder();

        foreach( var group in SchemeTypes )
        {
            var srcMethodBody = new StringBuilder();
            foreach (var method in group.Value)
            {
                srcMethodBody.Append(method.AuthSchemeSrcMethodBody);              
                srcOthers.Append(method.AuthSchemeSrcIdClass);
            }

            srcMethods.Append($$"""
                    
                        public static void {{group.Key}}(this IHostApplicationBuilder builder)
                        {
                            builder.Services.{{group.Key}}();
                        }

                        public static void {{group.Key}}(this IServiceCollection services)
                        {
                            {{srcMethodBody}}
                        }
                    
                """);
        }         
     
        var srcExtension = new StringBuilder();
        srcExtension.AppendLine($$"""
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Threading.Tasks;   
            using Microsoft.AspNetCore.Authentication;
            using Luc.Lwx;        
            using Microsoft.Extensions.Logging;
            using Microsoft.Extensions.Options;
            using System.Text.Encodings.Web;
            using System.Threading.Tasks;             
        
            namespace {{_assemblyName}}.Generated 
            {
                public static class GeneratedAuthSchemeMappings
                {
                    {{srcMethods}}
                }
            }

            {{srcOthers}}
            """);

        Context.AddSource("GeneratedAuthSchemeMappings.g.cs", srcExtension.ToString()); 
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
                    category: LwxGenerator.LwxEndpointCategory,
                    msgSeverity,
                    isEnabledByDefault: true
                ), 
                srcLocation, 
                msgArgs
            )
        );
    }
}
