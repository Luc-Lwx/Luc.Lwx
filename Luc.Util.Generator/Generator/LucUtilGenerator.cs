using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Luc.Util.Generator;


[Generator]
internal class LucUtilGenerator : IIncrementalGenerator
{
    public const string LucEndpointCategory = "Luc.Util";    

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // locate the appsettings.json of this project that will include important configuration for source generation
        var settingsProvider = 
            context.AdditionalTextsProvider
                .Where(file => file.Path.EndsWith("appsettings.json"))
                .Select((file, cancellationToken) => file.GetText(cancellationToken)?.ToString())
                .Where(text => text != null)
                .Collect();

        // locate all classes in the project that will be processed by the generator
        var codeProvider = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: static (s, _) => s is ClassDeclarationSyntax,
                transform: static (ctx, _) => ctx
            )
            .Collect();
        
        var combinedProvider = settingsProvider.Combine(codeProvider);

        context.RegisterSourceOutput
        (
            combinedProvider, 
            static (sourceProductionContext, typeSymbols) => 
            {
                var generator = new LucUtilAssemblyProcessor( 
                    sourceProductionContext, 
                    typeSymbols.Left, 
                    typeSymbols.Right
                );
                generator.Execute(); 
            }
        );
    }  
}