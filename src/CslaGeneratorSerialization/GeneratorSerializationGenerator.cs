using CslaGeneratorSerialization.Builders;
using CslaGeneratorSerialization.Extensions;
using CslaGeneratorSerialization.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace CslaGeneratorSerialization;

[Generator]
internal sealed class GeneratorSerializationGenerator
	: IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var modelInterfaceProvider = context.SyntaxProvider
			.CreateSyntaxProvider(
				(node, _) => node is InterfaceDeclarationSyntax,
				(context, token) =>
				{
					var symbol = context.SemanticModel.GetDeclaredSymbol(context.Node);

					if (symbol is INamedTypeSymbol type &&
						type.IsMobileObject() &&
						SerializationModel.TryCreate(type, context.SemanticModel.Compilation, out var model))
					{
						return model!;
					}

					return null;
				})
			.Where(_ => _ is not null);

		var modelClassProvider = context.SyntaxProvider
			.ForAttributeWithMetadataName("System.SerializableAttribute", (_, _) => true,
				(context, token) =>
				{
					var models = new List<SerializationModel>(context.Attributes.Length);

					foreach (var attribute in context.Attributes)
					{
						if (context.TargetSymbol is INamedTypeSymbol type &&
							type.IsMobileObject() &&
							SerializationModel.TryCreate(type, context.SemanticModel.Compilation, out var model))
						{
							models.Add(model!);
						}
					}

					return models;
				})
			.SelectMany((models, _) => models);

		context.RegisterSourceOutput(modelInterfaceProvider.Collect(),
			(context, source) => GeneratorSerializationGenerator.CreateInterfaceOutput(source, context));
		context.RegisterSourceOutput(modelClassProvider.Collect(),
			(context, source) => GeneratorSerializationGenerator.CreateClassOutput(source, context));
	}

	private static void CreateClassOutput(ImmutableArray<SerializationModel> models, SourceProductionContext context)
	{
		foreach (var model in models.Distinct())
		{
			var builder = new GeneratorSerializationBuilder(model);
			context.AddSource(builder.FileName, builder.Text);
		}
	}

	private static void CreateInterfaceOutput(ImmutableArray<SerializationModel?> models, SourceProductionContext context)
	{
		foreach (var model in models.Distinct())
		{
			var builder = new GeneratorSerializationBuilder(model!);
			context.AddSource(builder.FileName, builder.Text);
		}
	}
}