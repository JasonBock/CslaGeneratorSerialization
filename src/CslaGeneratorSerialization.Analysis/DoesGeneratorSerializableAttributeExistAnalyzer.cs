using CslaGeneratorSerialization.Analysis.Descriptors;
using CslaGeneratorSerialization.Analysis.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace CslaGeneratorSerialization.Analysis;

/// <summary>
/// An analyzer that looks for business objects that do not have <c>[GeneratorSerializableAttribute]</c> on them.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DoesGeneratorSerializableAttributeExistAnalyzer
	: DiagnosticAnalyzer
{
	private static readonly DiagnosticDescriptor rule =
		BusinessObjectDoesNotHaveSerializationAttributeDescriptor.Create();

	/// <summary>
	/// Initializes the analyzer.
	/// </summary>
	/// <param name="context">An <see cref="AnalysisContext"/> instance.</param>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> is <see langword="null"/>.</exception>
	public override void Initialize(AnalysisContext context)
	{
		if (context is null)
		{
			throw new ArgumentNullException(nameof(context));
		}

		context.ConfigureGeneratedCodeAnalysis(
			GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
		context.EnableConcurrentExecution();
		context.RegisterSymbolAction(
			DoesGeneratorSerializableAttributeExistAnalyzer.AnalyzeNamedTypeSymbol, SymbolKind.NamedType);
	}

	private static void AnalyzeNamedTypeSymbol(SymbolAnalysisContext context)
	{
		if (context.Symbol is INamedTypeSymbol typeSymbol)
		{
			if (typeSymbol.IsMobileObject())
			{
				if (!typeSymbol.GetAttributes().Any(data => data is not null && data.AttributeClass is not null &&
					data.AttributeClass.Name == "GeneratorSerializableAttribute" &&
					data.AttributeClass.ContainingAssembly.Name == "CslaGeneratorSerialization"))
				{
					if (typeSymbol.Locations.Length > 0)
					{
						context.ReportDiagnostic(Diagnostic.Create(
							DoesGeneratorSerializableAttributeExistAnalyzer.rule,
							typeSymbol.Locations[0], typeSymbol.Name));
					}
				}
			}
		}
	}

	/// <summary>
	/// Gets an array of supported diagnostics from this analyzer.
	/// </summary>
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		[DoesGeneratorSerializableAttributeExistAnalyzer.rule];
}