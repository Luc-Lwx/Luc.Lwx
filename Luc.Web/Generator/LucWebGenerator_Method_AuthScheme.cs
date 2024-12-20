using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Luc.Web.Generator;

[SuppressMessage("", "S101")]
internal class LucWebGenerator_Method_AuthScheme
(
    LucWebGenerator_Type _type,
    MethodDeclarationSyntax _method,
    AttributeData _attr
)
{
    internal LucWebGenerator_Type Type => _type;
    internal MethodDeclarationSyntax Method => _method;
    internal AttributeData Attr => _attr;

    internal string? AuthSchemeName { get; private set; } = null;
    internal string? AuthSchemeSrcMethodBody { get; private set; } = null;
    internal string? AuthSchemeSrcIdClass { get; private set; } = null;

    internal void Execute()
    {       
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

        if( !Type.TypeNameFull.StartsWith( $"{Type.TypeAssemblyName}.Web.AuthSchemes." ) )
        {
            Type.ReportWarning
            ( 
                msgSeverity: DiagnosticSeverity.Error, 
                msgId: "LUC0123", 
                msgFormat: $"""LWX: The type {Type.TypeNameFull} must be in the namespace {Type.TypeAssemblyName}.Web.AuthSchemes""", 
                srcLocation: Type.Type.GetLocation() 
            );
            return;
        } 

        if( !Type.TypeName.StartsWith( "AuthScheme" ) )
        {
            Type.ReportWarning
            ( 
                msgSeverity: DiagnosticSeverity.Error, 
                msgId: "LUC0123", 
                msgFormat: $"""
                    LWX: The type {Type.TypeNameFull} must be in the namespace {Type.TypeAssemblyName}.Web.AuthSchemes.AuthScheme<name>
                    
                    The <name> should be replaced by the desired scheme name.
                    """, 
                srcLocation: Type.Type.GetLocation()  
            );
            return;
        }

        
        AuthSchemeName = Type.TypeName.Substring( "AuthScheme".Length );

        var generatedMethodName = Attr.LucGetAttributeValueAsString( "GeneratedMethodName" ).LucIfNullOrEmptyReturn($"MapAuthSchemes_{Type.TypeAssemblyName.Replace(".","")}");

        if( !generatedMethodName.LucIsValidMethodName())
        {
            Type.ReportWarning
            ( 
                msgSeverity: DiagnosticSeverity.Error, 
                msgId: "LUC01232", 
                msgFormat: $"""
                    LWX: The generatedMethodName '{generatedMethodName}' is not a valid method name
                    """, 
                srcLocation: Attr.LucGetAttributeArgumentLocation("GeneratedMethodName") 
            );
            return;
        }

        if( !AuthSchemeName.LucIsValidMethodName())
        {
            Type.ReportWarning
            ( 
                msgSeverity: DiagnosticSeverity.Error, 
                msgId: "LUC00418", 
                msgFormat: $"""
                    LWX: The Name '{AuthSchemeName}' needs to be a valid property name
                    """, 
                srcLocation: Type.Type.GetLocation() 
            );
            return;
        }                
        
        if( Method.Identifier.Text != "Configure" ) 
        {
            Type.ReportWarning
            ( 
                msgSeverity: DiagnosticSeverity.Error, 
                msgId: "LUC00419", 
                msgFormat: $"""
                    LWX: The method '{Method.Identifier.Text}' must be named 'Configure'
                    """, 
                srcLocation: Method.GetLocation() 
            );
            return;
        }

        

        AuthSchemeSrcMethodBody = $$"""

                        // The code bellow is generated based on:
                        //   AUTH SCHEME {{AuthSchemeName}}
                        //   File: {{Type.TypeInFile}} 
                        //   Line: {{Type.Type.GetLocation().GetLineSpan().StartLinePosition.Line}}
                        //    Col: {{Type.Type.GetLocation().GetLineSpan().StartLinePosition.Character}}
                        //   Type: {{Type.TypeNameFull}}

                        {{Type.TypeNameFull}}.{{Method.Identifier.Text}}
                        (
                            services.AddAuthentication
                            (
                               "{{AuthSchemeName}}"
                            )
                        );
                                 
 
            """;      

        AuthSchemeSrcIdClass = $$"""
            // The code bellow is generated based on:
            //   AUTH SCHEME {{AuthSchemeName}}
            //   File: {{Type.TypeInFile}} 
            //   Line: {{Type.Type.GetLocation().GetLineSpan().StartLinePosition.Line}}
            //    Col: {{Type.Type.GetLocation().GetLineSpan().StartLinePosition.Character}}
            //   Type: {{Type.TypeNameFull}}

            namespace {{Type.TypeNamespaceName}}
            {
                public static partial class {{Type.TypeName}}
                {
                    public const string Id = "{{Type.AuthSchemeName}}";
                }
            }
            """;
        
        Type.TheAssembly.AddSchemeType(generatedMethodName,this);        

        Type.ReportWarning
        ( 
            msgSeverity: DiagnosticSeverity.Info, 
            msgId: "LUC008", 
            msgFormat: $$"""
                LWX: Fragment includedd in the generated method                                

                Method Body Fragment:

                {{AuthSchemeSrcMethodBody}}

                Id Class Fragment:

                {{AuthSchemeSrcIdClass}}
                """, 
            srcLocation: Attr.ApplicationSyntaxReference?.GetSyntax().GetLocation()
        );  
    }
}