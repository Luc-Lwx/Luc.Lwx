using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Luc.Lwx.Generator;

[SuppressMessage("","S101")]
internal partial class LwxGenerator_Method_Endpoint(
    LwxGenerator_Type _type,
    MethodDeclarationSyntax _method,
    AttributeData _attr
)
{    
    internal LwxGenerator_Type Type => _type;
    internal MethodDeclarationSyntax Method => _method;
    internal AttributeData Attr => _attr;

    internal string? EndpointSrcMethodBody { get; private set; } = null;
    
    [SuppressMessage("","S3776")]
    internal void Execute()
    {   
        Type.TheAssembly.AllowedWebClasses.Add( Type.TypeNameFull );

        if( !Type.TypeSymbol.LucIsPartialType() )
        {
            Type.ReportWarning
            ( 
                msgSeverity: DiagnosticSeverity.Error, 
                msgId: "LUC0011", 
                msgFormat: $"""LWX: The type {Type.TypeNameFull} must be a partial class""", 
                srcLocation: Type.Type.GetLocation() 
            );
            return;
        }

        if (!Type.TypeSymbol.DeclaredAccessibility.HasFlag(Accessibility.Public))
        {
            Type.ReportWarning
            ( 
                msgSeverity: DiagnosticSeverity.Error, 
                msgId: "LUC0012", 
                msgFormat: $"""LWX: The type {Type.TypeNameFull} must be public""", 
                srcLocation: Type.Type.GetLocation() 
            );
            return;
        }

        if (!Method.Modifiers.Any(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PublicKeyword))
        {
            Type.ReportWarning
            ( 
                msgSeverity: DiagnosticSeverity.Error, 
                msgId: "LUC0013", 
                msgFormat: $"""LWX: The method {Method.Identifier.Text} must be public""", 
                srcLocation: Method.GetLocation() 
            );
            return;
        }

        if (!Method.Modifiers.Any(Microsoft.CodeAnalysis.CSharp.SyntaxKind.StaticKeyword))
        {
            Type.ReportWarning
            ( 
                msgSeverity: DiagnosticSeverity.Error, 
                msgId: "LUC0014", 
                msgFormat: $"""LWX: The method {Method.Identifier.Text} must be static""", 
                srcLocation: Method.GetLocation() 
            );
            return;
        }

        if( !Type.TypeNameFull.StartsWith( $"{Type.TypeAssemblyName}.Web.Endpoints." ) )
        {
            Type.ReportWarning
            ( 
                msgSeverity: DiagnosticSeverity.Error, 
                msgId: "LUC006", 
                msgFormat: $"""LWX: The type {Type.TypeNameFull} must be in the namespace {Type.TypeAssemblyName}.Web.Endpoints""", 
                srcLocation: Type.Type.GetLocation() 
            );
            return;
        } 

        
        var attributes = Type.TypeSemanticModel.GetDeclaredSymbol(_method)?.GetAttributes();
        var lwxActivityLogAttribute = attributes?.FirstOrDefault( a => a.AttributeClass?.ToDisplayString() == LwxConstants.LwxActivityLogAttribute_FullName );
        if( lwxActivityLogAttribute == null )
        {
            Type.ReportWarning
            ( 
                msgSeverity: DiagnosticSeverity.Error, 
                msgId: "LUC0015", 
                msgFormat: $"""LWX: The method {Method.Identifier.Text} must have a [LwxActivityLog]""", 
                srcLocation: Method.GetLocation() 
            );
            return;
        }

        foreach( var parameter in Method.ParameterList.Parameters )
        {
            if( parameter.Type == null ) continue;            
            var parameterType = Type.TypeSemanticModel.GetTypeInfo(parameter.Type).Type;
            var parameterName = parameter.Identifier.Text;
            var parameterTypeName = parameterType?.ToDisplayString() ?? "";
            if( parameterTypeName.StartsWith( $"{Type.TypeAssemblyName}" ) )
            {
                if( !parameterTypeName.StartsWith( $"{Type.TypeAssemblyName}.Web.Model." ) || !parameterTypeName.EndsWith( $"RequestDto" ) )
                {
                    Type.ReportWarning
                    ( 
                        msgSeverity: DiagnosticSeverity.Error, 
                        msgId: "LUC0016", 
                        msgFormat: $"""LWX: The parameter {parameterName} must be in the namespace {Type.TypeAssemblyName}.Web.Model and ends with RequestDto""", 
                        srcLocation: parameter.GetLocation() 
                    );
                    return;
                }
                else 
                {
                    if( !Type.TheAssembly.AllowedWebClasses.Contains( parameterTypeName ) )
                    {
                        Type.TheAssembly.AllowedWebClasses.Add( parameterTypeName );
                    }
                }
            }        
        }

        // get method return value type
        var returnType = Type.TypeSemanticModel.GetTypeInfo(Method.ReturnType).Type;
        var returnTypeName = returnType?.ToDisplayString() ?? "";

        if( returnType is INamedTypeSymbol namedTypeSymbol ) 
        {
            var possibleReturnType = namedTypeSymbol.TypeArguments.FirstOrDefault();
            if( possibleReturnType != null ) 
            {
                var possibleReturnTypeName = possibleReturnType.ToDisplayString();
                if( possibleReturnTypeName.StartsWith( $"{Type.TypeAssemblyName}" ) )
                {
                    returnTypeName = possibleReturnTypeName;
                }
            }
        }                

        if (returnTypeName.StartsWith($"{Type.TypeAssemblyName}"))
        {
            if (!returnTypeName.StartsWith($"{Type.TypeAssemblyName}.Web.Model.") || !returnTypeName.EndsWith($"ResponseDto"))
            {
                Type.ReportWarning
                (
                    msgSeverity: DiagnosticSeverity.Error,
                    msgId: "LUC0016",
                    msgFormat: $"""
                        LWX: The return value type must be in the namespace {Type.TypeAssemblyName}.Web.Model and ends with Dto
                        """,
                    srcLocation: Method.GetLocation()
                );
                return;
            }
            else
            {
                if (!Type.TheAssembly.AllowedWebClasses.Contains(returnTypeName))
                {
                    Type.TheAssembly.AllowedWebClasses.Add(returnTypeName);
                }
            }
        }           

        // get attribute params
        var attrMethodAndPath = Attr.LucGetAttributeValueAsString( "Path" );
        var justificationForAttributeInPath = Attr.LucGetAttributeValueAsString( "LowMaintanability_ParameterInPath_Justification" );
        var justificationForPathNotInApiManagerPrefix = Attr.LucGetAttributeValueAsString( "LowMaintanability_NotInApiManagerPath_Justification" );        

        var generatedMethodName = Attr.LucGetAttributeValueAsString("GeneratedMethodName").LucIfNullOrEmptyReturn($"MapEndpoints_{Type.TypeAssemblyName.Replace(".","")}");
                
        // verifica se generatedMethodName é um nome de método válido
        if( !generatedMethodName.LucIsValidMethodName() )
        {
            Type.ReportWarning
            ( 
                msgSeverity: DiagnosticSeverity.Error, 
                msgId: "LUC0018", 
                msgFormat: $"""
                    LWX: The generatedMethodName '{generatedMethodName}' is not a valid method name
                    """, 
                srcLocation: Attr.LucGetAttributeArgumentLocation("GeneratedMethodName") 
            );
            return;
        }

        var attrPathMatcher = EndpointPathPattern().Match(attrMethodAndPath);
        if (!attrPathMatcher.Success)
        {
            Type.ReportWarning
            (
                msgId: "LUC0014",
                msgSeverity: DiagnosticSeverity.Error,
                msgFormat: """
                    LWX: The path should be 'POST /path'
                    
                    where the path should be:                    
                    * GET for operations without request body; 
                    * POST for operations with request body;
                    
                    You may other methods:
                    * PUT (not recomended);
                    * PATCH (not recomended);
                    * DELETE (not recomended);
                    * HEAD;                    
                    
                    The method is to select the input formats and the operation should be reflected on the path. 

                    The RESTful dissertation recomended an object oriented style where the http path would be the instance id and the http method the method, but this is impossible this days. The reason this doesn't work is because routing in based on the path prefix and most proxies, languages and frameworks have a very limited selection of suported methods. 
                    """,
                srcLocation: Attr.LucGetAttributeArgumentLocation("Path")
            );
            return;
        }

        var attrMethod = attrPathMatcher.Groups[1].Value;
        var attrPath = attrPathMatcher.Groups[2].Value;
        if( attrPath.Contains( '{' ) && string.IsNullOrWhiteSpace( justificationForAttributeInPath ) )
        {            
            Type.ReportWarning
            ( 
                msgSeverity: DiagnosticSeverity.Error, 
                msgId: "LUC0022", 
                msgFormat: $$"""
                    LWX: The utilization of parameters in path is not recomended

                    In place of: POST /myapp/mycollection/{collectionId}
                    Use          POST /myapp/mycollection?collectionId={collectionId}

                    Parameters on the path are not nammed. 
                    This is bad for mantainanbility and auditability of the applications. 
                                        
                    Browsers, CDNs, edge proxies, reverse proxies, API accelerators uses the complete URI as cache key.
                    For caching makes no difference if the parameter is in the path or in the query string. 

                    My recommendation is to put a proccess_id, user_id on the query_string even when using POST methods.
                    Even considering that POST/PUT/DELETE methods are not cached by default this is usefull to track the logs.

                    If you really need this, you can supress this warning using:
                    
                    [LwxEndpoint(
                        Path = "{attrMethod} {attrPath}",
                        LowMaintanability_ParameterInPath_Justification = "I need this because of ..."
                        ...
                    )]
                    """, 
                srcLocation: Attr.LucGetAttributeArgumentLocation("Path")
            );    
        }
        
        var apiManagerPath = Type.TheAssembly.ApiManagerPath;
        if( apiManagerPath != null )
        {
            if( apiManagerPath.EndsWith( "/" ) )      
            {
                Type.ReportWarning
                ( 
                    msgSeverity: DiagnosticSeverity.Error, 
                    msgId: "LUC06546", 
                    msgFormat: $$"""
                        The ApiManagerPath in appsettings.json cannot end with a slash.

                        Please set the ApiManagerPath in your appsettings.json like this:

                        {
                            "Lwx": {
                                "ApiManagerPath": "/api-manager-path" <-- CAN'T END WITH A SLASH
                            }
                        }
                        """, 
                    srcLocation: Attr.LucGetAttributeArgumentLocation("Path") 
                );
                return;
            }
            else if( attrPath.StartsWith( $"{apiManagerPath}/" ) ) 
            {                
                if( !justificationForPathNotInApiManagerPrefix.LucIsNullOrEmpty() ) 
                {
                    Type.ReportWarning
                    ( 
                        msgSeverity: DiagnosticSeverity.Error, 
                        msgId: "LUC0215", 
                        msgFormat: $$"""
                            LWX: Your API does not violate the rule NotInApiManagerPath!
                            
                            The use of LowMaintanability_NotInApiManagerPath_Justification is only allowed when the rule is violated

                            Api Manager Base: {{apiManagerPath}}
                            Api Path: {{attrPath}}
                            """, 
                        srcLocation: Attr.LucGetAttributeArgumentLocation("Path") 
                    );
                    return;
                }
            }
            else 
            {
                if( justificationForPathNotInApiManagerPrefix.LucIsNullOrEmpty() )
                {
                    Type.ReportWarning
                    ( 
                        msgSeverity: DiagnosticSeverity.Error, 
                        msgId: "LUC0015", 
                        msgFormat: $$"""
                            LWX: The path must start with {{apiManagerPath}}/

                            It is recommended that you use the same prefix that you will use to publish it.

                            If you need to disable this rule, you can use the justification LowMaintanability_NotInApiManagerPath_Justification

                            Api Manager Base: {{apiManagerPath}}
                            Api Path: {{attrPath}}

                            The Api Manager Base is set in the appsettings.json like this

                            {
                                "Lwx": {
                                    "ApiManagerPath": "/api-manager-path"
                                }
                            }
                            """, 
                        srcLocation: Attr.LucGetAttributeArgumentLocation("Path") 
                    );
                    return;
                }
            }
        }
        else 
        {
            Type.ReportWarning
            ( 
                msgSeverity: DiagnosticSeverity.Error, 
                msgId: "LUC0016", 
                msgFormat: $$"""
                    The ApiManagerPath is not found in appsettings.json.

                    Please set the ApiManagerPath in your appsettings.json like this:

                    {
                        "Lwx": {
                            "ApiManagerPath": "/api-manager-path"
                        }
                    }

                    You can find the ApiManagerPath in the configuration section of your project. 
                    It is typically located in the appsettings.json file or in the environment variables.
                    """, 
                srcLocation: Attr.LucGetAttributeArgumentLocation("Path") 
            );
            return;
        }

        var expectedTypeNameBase = $"{Type.TypeAssemblyName}.Web.Endpoints";      
        var expectedTypeNameReference = attrPath.Substring( apiManagerPath.Length ).Trim('/');        
        expectedTypeNameReference = expectedTypeNameReference.Replace( "{", "param-" );
        expectedTypeNameReference = expectedTypeNameReference.Replace( "}", "" );

        var expectedShortTypeName = expectedTypeNameReference.LucGetFileNameFromUrl().Trim('/').LucPathElementToCamelCase();   
        var expectedNamespace = expectedTypeNameReference.LucGetDirNameFromUrlPath().Trim('/').LucPathToCamelCase();
                
        var expectedFullTypeName = 
            expectedNamespace.LucIsNullOrEmpty() ?
                $"{expectedTypeNameBase}.Endpoint{expectedShortTypeName}"
            :
                $"{expectedTypeNameBase}.{expectedNamespace}.Endpoint{expectedShortTypeName}"        
        ;

        if( Type.TypeNameFull != expectedFullTypeName ) 
        {
            Type.ReportWarning
            ( 
                msgSeverity: DiagnosticSeverity.Error, 
                msgId: "LUC0013", 
                msgFormat: $"""
                    LWX: The path {attrPath} must be implemented in the type {expectedFullTypeName}

                    expectedTypeNameReference: {expectedTypeNameReference}
                    expectedShortTypeName: {expectedShortTypeName}
                    expectedNamespace: {expectedNamespace}
                    expectedFullTypeName: {expectedFullTypeName}
                """,
                srcLocation: Type.Type.GetLocation() 
            );
            return;
        }         

        switch( attrMethod.ToUpper() )
        {
            case "GET": attrMethod = "Get"; break;
            case "POST": attrMethod = "Post"; break;
            case "PUT": attrMethod = "Put"; break;
            case "PATCH": attrMethod = "Patch"; break;
            case "DELETE": attrMethod = "Delete"; break;
            case "HEAD": attrMethod = "Head"; break;
            default: 
                Type.ReportWarning
                ( 
                    msgSeverity: DiagnosticSeverity.Error, 
                    msgId: "LUC0017", 
                    msgFormat: $"""LWX: The method {attrMethod} is not supported by dotnet core minimal APIs""", 
                    srcLocation: Attr.LucGetAttributeArgumentLocation("Path") 
                );
                return;
        }


        var swaggerGroupTitle = Attr.LucGetAttributeValueAsString("SwaggerGroupTitle");
        var swaggerFuncSummary = Attr.LucGetAttributeValueAsString("SwaggerFuncSummary");
        var swaggerFuncDescription = Attr.LucGetAttributeValueAsString("SwaggerFuncDescription");
        var swaggerFuncName = Attr.LucGetAttributeValueAsString("SwaggerFuncName");
        var authPolicy = Attr.LucGetAttributeValueAsType(LwxConstants.AuthPolicy);

        if( authPolicy == null ) 
        {
            Type.ReportWarning
            ( 
                msgSeverity: DiagnosticSeverity.Error, 
                msgId: "LUC0019", 
                msgFormat: $"""
                    LWX: The AuthPolicy must be defined as the example bellow
                    
                    [LwxEndpoint(
                        Path = "{attrMethodAndPath}",
                        AuthPolicy = typeof(AuthPolicyExample001),
                        ...
                    )]
                    """, 
                srcLocation: Attr.LucGetAttributeArgumentLocation(LwxConstants.AuthPolicy) 
            );
            return;
        }

        if( !authPolicy.Name.StartsWith(LwxConstants.AuthPolicy) ) 
        {
            Type.ReportWarning
            ( 
                msgSeverity: DiagnosticSeverity.Error, 
                msgId: "LUC0019", 
                msgFormat: $"""
                    LWX: The AuthPolicy must be a class that starts with AuthPolicy as in the example bellow
                    
                    [LwxEndpoint(
                        Path = "{attrMethodAndPath}",
                        AuthPolicy = typeof(AuthPolicyExample001),
                        ...
                    )]
                    """, 
                srcLocation: Attr.LucGetAttributeArgumentLocation("AuthPolicy") 
            );
            return;
        }

        var authPolicyName = authPolicy.Name.Substring( "AuthPolicy".Length );

        if( Method.Identifier.Text != "Execute" ) 
        {
            Type.ReportWarning
            ( 
                msgSeverity: DiagnosticSeverity.Error, 
                msgId: "LUC00419", 
                msgFormat: $"""
                    LWX: The method '{Method.Identifier.Text}' must be named 'Execute'
                    """, 
                srcLocation: Method.GetLocation() 
            );
            return;
        }

        var srcFuncNameFragment = 
            swaggerFuncName.LucIsNullOrEmpty() ? 
                "" 
            : 
                $$"""
                .WithDisplayName( {{SymbolDisplay.FormatLiteral(swaggerFuncName,true)}} )
                """;


        EndpointSrcMethodBody = $$"""

                        // The code bellow is generated based on:
                        //   ENDPOINT {{attrMethod.ToUpper()}} {{attrPath}}
                        //   File: {{Type.TypeInFile}} 
                        //   Line: {{Type.Type.GetLocation().GetLineSpan().StartLinePosition.Line}}
                        //    Col: {{Type.Type.GetLocation().GetLineSpan().StartLinePosition.Character}}
                        //   Type: {{Type.TypeNameFull}}
            
                        app.Map{{attrMethod}}(
                            "{{attrPath}}", 
                            {{expectedFullTypeName}}.Execute
                        )
                        {{srcFuncNameFragment}}
                        .WithTags( [ {{SymbolDisplay.FormatLiteral(swaggerGroupTitle,true)}} ] )
                        .WithSummary( {{SymbolDisplay.FormatLiteral(swaggerFuncSummary,true)}} )
                        .WithDescription( {{SymbolDisplay.FormatLiteral(swaggerFuncDescription,true)}} )
                        .RequireAuthorization( {{SymbolDisplay.FormatLiteral(authPolicyName,true)}} )                          
                        ;            

            """;

       
        
        Type.TheAssembly.AddEndpointMappingMethod(generatedMethodName,this);                    
                

        Type.ReportWarning
        ( 
            msgSeverity: DiagnosticSeverity.Info, 
            msgId: "LUC008", 
            msgFormat: $$"""
                LWX: Fragment includedd in the generated method
                                
                Mapping: {{attrMethodAndPath}}
                Method: {{expectedFullTypeName}}.Execute
                
                The generated fragment is:
                
                {{EndpointSrcMethodBody}}
                """, 
            srcLocation: Attr.ApplicationSyntaxReference?.GetSyntax().GetLocation()
        );        
    }
         
    public static Regex EndpointPathPattern()
    {
        string pattern = "^(\\S*) (/.*)$";
        return new Regex(pattern, RegexOptions.CultureInvariant|RegexOptions.IgnoreCase|RegexOptions.Compiled);
    }
}