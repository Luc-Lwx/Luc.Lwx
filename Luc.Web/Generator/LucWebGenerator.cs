using Luc.Web.Interface;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Luc.Web.Generator;

[Generator]
internal class LucWebGenerator : IIncrementalGenerator
{
    public const string LucEndpointCategory = "Luc.Web";    

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // locate the appsettings.json of this project that will include important configuration for source generation
        var appSettingsProvider = 
            context.AdditionalTextsProvider
                .Where(file => file.Path.EndsWith("appsettings.json"))
                .Select((file, cancellationToken) => file.GetText(cancellationToken)?.ToString())
                .Where(text => text != null)
                .Collect();
       

        // locate all classes in the project that will be processed by the generator
        var allTypesProvider = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: static (s, _) => s is ClassDeclarationSyntax,
                transform: static (ctx, _) => ctx
            )
            .Collect();
        
   
        var combinedProvider = appSettingsProvider.Combine(allTypesProvider);
        
        context.RegisterSourceOutput
        (
            combinedProvider, 
            (sourceProductionContext, typeSymbols) => 
            {
                var (allAppSettings, allTypes) = typeSymbols;
                
                var processor = new LucWebGenerator_Assembly(sourceProductionContext, allAppSettings, allTypes);
                processor.Execute();
            }
        );
    }  

    
}

