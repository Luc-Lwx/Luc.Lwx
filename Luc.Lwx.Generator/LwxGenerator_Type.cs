using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Luc.Lwx.Generator;

[SuppressMessage("","S101")]
internal partial class LwxGenerator_Type 
{
    internal LwxGenerator_Assembly TheAssembly { get; private set; }
    internal GeneratorSyntaxContext TypeContext { get; private set; }
    internal ClassDeclarationSyntax Type { get; private set; }
    internal SemanticModel TypeSemanticModel { get; private set; }
    internal INamedTypeSymbol TypeSymbol { get; private set; }
    internal string TypeName { get; private set; }
    internal string TypeNameFull { get; private set; }
    internal string TypeNamespaceName { get; private set; }
    internal string TypeInFile { get; private set; }
    internal string TypeAssemblyName { get; private set; }

    internal string? AuthPolicyName { get; private set; } = null;
    internal string? AuthPolicySrcMethodBody { get; private set; } = null;
    internal string? AuthPolicySrcIdClass { get; private set; } = null;

    internal string? AuthSchemeName { get; private set; } = null;
    internal string? AuthSchemeSrcMethodBody { get; private set; } = null;
    internal string? AuthSchemeSrcIdClass { get; private set; } = null;
    
    public LwxGenerator_Type( LwxGenerator_Assembly assemblyProcessor, GeneratorSyntaxContext context ) 
    {        
        TheAssembly = assemblyProcessor;
        TypeContext = context; 
        Type = (ClassDeclarationSyntax)context.Node;
        TypeSemanticModel = context.SemanticModel;
        TypeSymbol = (TypeSemanticModel.GetDeclaredSymbol(Type) as INamedTypeSymbol) ?? throw new InvalidOperationException("Type symbol not found");
        TypeName = TypeSymbol?.Name ?? throw new InvalidOperationException("Type name not found");
        TypeNameFull = TypeSymbol?.ToDisplayString() ?? throw new InvalidOperationException("Type name not found");
        TypeNamespaceName = TypeSymbol?.ContainingNamespace.ToDisplayString() ?? throw new InvalidOperationException("Namespace name not found");
        TypeInFile = Type.SyntaxTree.FilePath;    
        TypeAssemblyName = TypeContext.SemanticModel.Compilation.AssemblyName ?? throw new InvalidOperationException("Assembly name not found");                        
    }

    


    public void DoProccess() 
    {        
        DoProcessNamingConventions();
        DoBlockOldStyleEndpoints();                
        DoProccessMethods();
    }
  
    public void DoProcessPassTwo()
    {
        DoValidateReservedNamespace();
    }

    private void DoValidateReservedNamespace() 
    {
        if (!TypeSymbol.DeclaredAccessibility.HasFlag(Accessibility.Public))
        {
            // ignore if not public
            return;
        }
        if( !TypeNameFull.StartsWith( $"{TypeAssemblyName}.Web." ) )
        {
            // ignore if not in the reserved namespace
            return;
        }
        if( TypeSymbol.ContainingType != null) 
        {
            // ignore inner types
            return;
        }
        if( TypeNameFull.StartsWith($"{TypeAssemblyName}.Web.Model.") && TypeNameFull.EndsWith("Dto") )
        {
            if( !TheAssembly.AllowedWebClasses.Contains( TypeNameFull ) )
            {
                ReportWarning
                ( 
                    msgSeverity: DiagnosticSeverity.Warning, 
                    msgId: "LUC007", 
                    msgFormat: $$"""
                        The type {{TypeNameFull}} is unused by any endpoint.
                        """, 
                    srcLocation: Type.GetLocation() 
                );
                return;
            }
            else
            {
                return;   
            }
        }
        if( !TheAssembly.AllowedWebClasses.Contains( TypeNameFull ) )
        {
            ReportWarning
            ( 
                msgSeverity: DiagnosticSeverity.Error, 
                msgId: "LUC006", 
                msgFormat: $$"""
                    LWX: The type {{TypeNameFull}} is not allowed in the '{{TypeNamespaceName}}.Web.' namespace.
                    
                    The namespace {{TypeNamespaceName}}.Web. is reserved for the Lwx Web

                    Classes with attributes such as:
                    * [LwxEndpoint]
                    * [LwxAuthPolicy]
                    * [LwxAuthScheme]

                    {{TheAssembly.AllowedWebClasses}}
                    """, 
                srcLocation: Type.GetLocation() 
            );
            return;
        }
    }


    private void DoProccessMethods() 
    {
        foreach (var member in Type.Members)
        {   
            if (member is MethodDeclarationSyntax method)
            {
                DoProcessMethodAttributes(method);
            }
        }
    }

    private void DoProcessMethodAttributes(MethodDeclarationSyntax method) 
    {        
        var attributes = TypeSemanticModel.GetDeclaredSymbol(method)?.GetAttributes();
        if (attributes == null) return;

        foreach (var attribute in attributes)
        {
            var attr = attribute.AttributeClass;
            if (attr?.ToDisplayString() == LwxConstants.LwxEndpointAttribute_FullName)
            {                        
                var processor = new LwxGenerator_Method_Endpoint(this, method, attribute);
                processor.Execute();
            }
            if (attr?.ToDisplayString() == LwxConstants.LwxAuthPolicyAttribute_FullName)
            {                        
                var processor = new LwxGenerator_Method_AuthPolicy(this, method, attribute);
                processor.Execute();
            }
            if (attr?.ToDisplayString() == LwxConstants.LwxAuthSchemeAttribute_FullName)
            {                        
                var processor = new LwxGenerator_Method_AuthScheme(this, method, attribute);
                processor.Execute();
            }
        }
    }

    
    [SuppressMessage("","S3776")]
    [SuppressMessage("","S3626")]
    private void DoProcessNamingConventions() 
    {
        var wrongNamespace = $"{TypeAssemblyName}.src.";

        if( TypeNameFull.StartsWith( wrongNamespace, StringComparison.InvariantCultureIgnoreCase ) ) 
        {      
            ReportWarning
            ( 
                msgSeverity: DiagnosticSeverity.Error, 
                msgId: "LUC005", 
                msgFormat: """LWX: The namespace can't contain the 'src' element.""", 
                srcLocation: Type.GetLocation() 
            );
            return;
        }
        
        if( !TypeNameFull.StartsWith( $"{TypeAssemblyName}.", StringComparison.InvariantCulture ) )
        {
            ReportWarning
            ( 
                msgSeverity: DiagnosticSeverity.Error, 
                msgId: "LUC003", 
                msgFormat: $"""LWX: The type name '{TypeName}' must start with '{TypeAssemblyName}'.""", 
                srcLocation: Type.GetLocation() 
            );
            return;
        }

        if( TypeSymbol.DeclaredAccessibility == Accessibility.Public && TypeSymbol.ContainingType == null )
        {      
            var relativeTypeName = TypeNameFull.Replace( TypeAssemblyName+".", "");
            var expectedFile = $"{relativeTypeName.Replace(".","/")}";
            var expectedFileDir = Path.GetDirectoryName( expectedFile ) ?? "";
            var expectedFileName = Path.GetFileName( expectedFile ) ?? "";
            expectedFile += ".cs";

            var typeInFileDir = Path.GetDirectoryName( TypeInFile ) ?? "";
            var typeInFileName = Path.GetFileNameWithoutExtension( TypeInFile ) ?? "";
                      
            if( TypeSymbol.LucIsPartialType() ) 
            {
                if( ! ( typeInFileDir.EndsWith( expectedFileDir ) && ( typeInFileName.StartsWith( $"{expectedFileName}_" ) || typeInFileName == expectedFileName ) ) )
                {                 
                    ReportWarning
                    ( 
                        msgSeverity: DiagnosticSeverity.Error, 
                        msgId: "LUC012", 
                        msgFormat: $"""LWX: The type {TypeNameFull} must be in the source file {expectedFileDir}/{expectedFileName}.cs or {expectedFileDir}/{expectedFileName}_*.cs""", 
                        srcLocation: Type.GetLocation() 
                    );
                    return;
                }
            }
            else 
            {
                if( !TypeInFile.EndsWith( expectedFile ) )
                {                 
                    ReportWarning
                    ( 
                        msgSeverity: DiagnosticSeverity.Error, 
                        msgId: "LUC004", 
                        msgFormat: $"""LWX: The type {TypeNameFull} must be in the source file {expectedFile}""", 
                        srcLocation: Type.GetLocation() 
                    );
                    return;
                }           
            }                                 
        }
    }

    private void DoBlockOldStyleEndpoints() 
    {
        if (TypeSymbol.BaseType?.ToDisplayString() == "Microsoft.AspNetCore.Mvc.ControllerBase")
        {            
            ReportWarning
            ( 
                msgSeverity: DiagnosticSeverity.Error, 
                msgId: "LUC001", 
                msgFormat: $"""LWX: The utilization of Controller is forbidden! Use [LwxEndpoint] instead.""", 
                srcLocation: Type.GetLocation() 
            );
        }

        foreach( var attr in TypeSymbol.GetAttributes() )
        {
            if (attr.AttributeClass?.ToDisplayString() == "Microsoft.AspNetCore.Mvc.ControllerAttribute")
            {                
                ReportWarning
                ( 
                    msgSeverity: DiagnosticSeverity.Error, 
                    msgId: "LUC002", 
                    msgFormat: $"""LWX: The utilization of [Controller] is forbidden! Use [LwxEndpoint] instead.""", 
                    srcLocation: attr.ApplicationSyntaxReference?.GetSyntax().GetLocation() 
                );
            }
        }
    }

    internal void ReportWarning( DiagnosticSeverity msgSeverity, string msgId, string msgFormat, Location? srcLocation, params object[] msgArgs ) 
    {
        TheAssembly.ReportWarning( 
            msgSeverity: msgSeverity, 
            msgId: msgId, 
            msgFormat: msgFormat, 
            srcLocation: srcLocation, 
            msgArgs: msgArgs 
        );
    }
}