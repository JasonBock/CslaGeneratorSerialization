using CslaGeneratorSerialization.Analysis.Models;
using CslaGeneratorSerialization.Analysis.Builders;
using CslaGeneratorSerialization.Analysis.Extensions;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace CslaGeneratorSerialization.Analysis;

[Generator]
internal sealed class GeneratorSerializationGenerator
	: IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var provider = context.SyntaxProvider
			.ForAttributeWithMetadataName("CslaGeneratorSerialization.GeneratorSerializableAttribute", (_, _) => true,
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

		context.RegisterTypes();

		context.RegisterSourceOutput(provider.Collect(),
			(context, source) => GeneratorSerializationGenerator.CreateOutput(source, context));
	}

	private static void CreateOutput(ImmutableArray<SerializationModel> models, SourceProductionContext context)
	{
		foreach (var model in models.Distinct())
		{
			var builder = new GeneratorSerializationBuilder(model);
			context.AddSource(builder.FileName, builder.Text);
		}
	}
}