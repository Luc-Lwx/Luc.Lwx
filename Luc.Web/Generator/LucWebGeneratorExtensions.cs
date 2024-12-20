using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Luc.Web.Generator;

internal static partial class LucWebGeneratorExtensions
{
    public static string LucGetAttributeValueAsString
    (
        this AttributeData attributeData, 
        string argumentName
    ) 
    {
       return attributeData.NamedArguments
            .FirstOrDefault(arg => arg.Key == argumentName)
            .Value.Value?.ToString() ?? string.Empty;
    }

    public static ITypeSymbol? LucGetAttributeValueAsType
    (
        this AttributeData attributeData, 
        string argumentName
    ) 
    {
        return attributeData.NamedArguments
            .FirstOrDefault(arg => arg.Key == argumentName)
            .Value.Value as ITypeSymbol;
    }

    public static Location? LucGetAttributeArgumentLocation
    (
        this AttributeData attributeData, 
        string argumentName
    )
    {
        var syntaxReference = attributeData.ApplicationSyntaxReference;
        if (syntaxReference == null)
        {
            return null;
        }

        if (syntaxReference.GetSyntax() is not AttributeSyntax attributeSyntax)
        {
            return null;
        }

        var argumentSyntax = attributeSyntax.ArgumentList?.Arguments
            .FirstOrDefault(arg => arg.NameEquals?.Name.Identifier.Text == argumentName);

        return argumentSyntax?.GetLocation();
    }

    public static bool LucIsPartialType(this INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.DeclaringSyntaxReferences
            .Select(syntaxReference => syntaxReference.GetSyntax())
            .OfType<TypeDeclarationSyntax>()
            .Any(typeDeclarationSyntax => typeDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword));
    }


    public static string LucPathElementToCamelCase(this string path)
    {   
        var result = new StringBuilder();        

        var NextToUpper = true;
        for( var i = 0; i < path.Length; i++ ) 
        {
            switch( path[i] ) 
            {
                case '-':
                    NextToUpper = true;                    
                    break;
                default:
                    if (NextToUpper) 
                    {
                        result.Append( char.ToUpperInvariant( path[i] ) );
                        NextToUpper = false;
                    } 
                    else 
                    {
                        result.Append( char.ToLowerInvariant( path[i] ) );
                    }
                    break;
            }
        }        
        return result.ToString();
    }

    public static string LucPathToCamelCase( this string path ) 
    {
        var result = new List<string>();
        foreach( var pathElement in path.Split('/') ) 
        {
            if( !pathElement.LucIsNullOrEmpty() ) 
            {
                result.Add( LucPathElementToCamelCase( pathElement ) );
            }
        }        
        return string.Join('.', result);
    }

    /// <summary>
    /// Returns the base path of a path.
    /// </summary>
    /// <returns>
    /// * for the path "/abc/cde/def", return "/abc/cde".
    /// * for the path "/abc/cde/", return "/abc/cde".
    /// * for the path "/abc", return "".        
    /// </returns>
    public static string LucGetDirNameFromUrlPath( this string path ) 
    {
        var pos = path.LastIndexOf('/');
        if( pos == -1 )
        {
            return string.Empty;
        }
        else 
        {
            return path[..pos];
        }
    }

    /// <summary>
    /// Return the path without the trailing slash if it exists.
    /// </summary>
    /// <param name="path"></param>
    /// <returns>
    /// * for the path /abc/cde/: /abc/cde
    /// * for the path /abc/cde: /abc/cde    
    /// </returns>
    public static string LucRemoveTrailingSlashIfExists( this string path ) 
    {
        return path.EndsWith('/') ? path[..^1] : path;
    }

    public static string LucRemoveRootSlashIfExists( this string path ) 
    {
        return path.EndsWith('/') ? path[1..] : path;
    }


    /// <summary>
    /// Returns the base name of a path.
    /// </summary>    
    /// <returns>
    /// * for the path "/abc/cde/def", return "def".
    /// * for the path "/abc/cde/", return "".
    /// * for the path "/abc", return "abc".
    /// * for the path "/", return "".
    /// </returns>
    public static string LucGetFileNameFromUrl( this string path ) 
    {
        var pos = path.LastIndexOf('/');
        if( pos == -1 )
        {
            return path;
        }
        else 
        {
            return path[pos..];
        }
    }

    public static bool LucIsNullOrEmpty( this string value ) 
    {
        return string.IsNullOrEmpty( value );
    }

    public static string LucIfNullOrEmptyReturn( this string value, string defaultValue ) 
    {
        return value.LucIsNullOrEmpty() ? defaultValue : value;
    }


    public static bool LucIsValidMethodName( this string methodName ) 
    {     
        return RegexValidMethodName().IsMatch( methodName );
    }
    public static bool LucIsValidPropertyName( this string methodName ) 
    {     
        return RegexValidPropertyName().IsMatch( methodName );
    }


    internal static void LucReportWarning( this SourceProductionContext context, DiagnosticSeverity msgSeverity, string msgId, string msgFormat, Location? srcLocation, params object[] msgArgs ) 
    {
        context.ReportDiagnostic
        ( 
            Diagnostic.Create
            (
                new DiagnosticDescriptor
                (
                    id: msgId,
                    title: msgFormat,
                    messageFormat: msgFormat,
                    category: LucWebGenerator.LwxEndpointCategory,
                    msgSeverity,
                    isEnabledByDefault: true
                ), 
                srcLocation, 
                msgArgs
            )
        );
    }


    [GeneratedRegex("^[a-zA-Z_][a-zA-Z0-9_]*$")]
    private static partial Regex RegexValidMethodName();

    [GeneratedRegex("^[A-Z_][a-zA-Z0-9_]*$")]
    private static partial Regex RegexValidPropertyName();
}