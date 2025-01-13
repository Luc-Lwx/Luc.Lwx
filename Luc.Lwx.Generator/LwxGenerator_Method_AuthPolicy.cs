using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Luc.Lwx.Generator;

[SuppressMessage("", "S101")]
internal class LwxGenerator_Method_AuthPolicy
(
    LwxGenerator_Type _type,
    MethodDeclarationSyntax _method,
    AttributeData _attr
)
{
    internal LwxGenerator_Type Type => _type;
    internal MethodDeclarationSyntax Method => _method;
    internal AttributeData Attr => _attr;

    internal string? AuthPolicyName { get; private set; } = null;
    internal string? AuthPolicySrcMethodBody { get; private set; } = null;
    internal string? AuthPolicySrcIdClass { get; private set; } = null;

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

        // the type must be in the correct namespace
        if( !Type.TypeNameFull.StartsWith( $"{Type.TypeAssemblyName}.Web.AuthPolicies." ) )
        {
            Type.ReportWarning
            ( 
                msgSeverity: DiagnosticSeverity.Error, 
                msgId: "LUC006", 
                msgFormat: $"""
                    LWX: The authentication policies must be in the namespace {Type.TypeAssemblyName}.Web.AuthPolicies 

                    Found: {Type.TypeNamespaceName}                    
                    Assembly Name: {Type.TypeAssemblyName}
                    """, 
                srcLocation: Type.Type.GetLocation() 
            );
            return;
        } 

        // the type must start with AuthPolicy
        if( !Type.TypeName.StartsWith( "AuthPolicy" ) )
        {
            Type.ReportWarning
            ( 
                msgSeverity: DiagnosticSeverity.Error, 
                msgId: "LUC026", 
                msgFormat: $"""
                    LWX: Authentication policies types must start with AuthPolicy

                    Ex: AuthPolicyExample001
                    Found: {Type.TypeName}
                    """, 
                srcLocation: Type.Type.GetLocation() 
            );
            return;
        } 

        // retrieve the policy name
        AuthPolicyName = Type.TypeName.Substring( "AuthPolicy".Length );

        // retrieve or generate the method name
        var generatedMethodName = Attr.LucGetAttributeValueAsString( "GeneratedMethodName" ).LucIfNullOrEmptyReturn($"MapAuthPolicies_{Type.TypeAssemblyName.Replace(".","")}");
        
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
         
        AuthPolicySrcIdClass = $$"""
        
            // The code bellow is generated based on:
            //   POLICY {{AuthPolicyName}}
            //   File: {{Type.TypeInFile}} 
            //   Line: {{Type.Type.GetLocation().GetLineSpan().StartLinePosition.Line}}
            //    Col: {{Type.Type.GetLocation().GetLineSpan().StartLinePosition.Character}}
            //   Type: {{Type.TypeNameFull}}

            namespace {{Type.TypeNamespaceName}}
            {
                public static partial class {{Type.TypeName}}
                {
                    public const string Id = "{{Type.AuthPolicyName}}";
                }
            }
            """; 

        AuthPolicySrcMethodBody = $$"""

                        // The code bellow is generated based on:
                        //   POLICY {{AuthPolicyName}}
                        //   File: {{Type.TypeInFile}} 
                        //   Line: {{Type.Type.GetLocation().GetLineSpan().StartLinePosition.Line}}
                        //    Col: {{Type.Type.GetLocation().GetLineSpan().StartLinePosition.Character}}
                        //   Type: {{Type.TypeNameFull}}
                                
                        options.AddPolicy(
                            "{{AuthPolicyName}}", 
                            {{Type.TypeNameFull}}.{{Method.Identifier.Text}}
                        );            
 
            """;      

        Type.TheAssembly.AddPolicyType(generatedMethodName,this);                    
                
        Type.ReportWarning
        ( 
            msgSeverity: DiagnosticSeverity.Info, 
            msgId: "LUC008", 
            msgFormat: $$"""
                LWX: Fragment includedd in the generated method                                

                Method Fragment:

                {{AuthPolicySrcMethodBody}}

                Other Fragments:

                {{AuthPolicySrcIdClass}}

                """, 
            srcLocation: Attr.ApplicationSyntaxReference?.GetSyntax().GetLocation()
        );    
     
    }
}