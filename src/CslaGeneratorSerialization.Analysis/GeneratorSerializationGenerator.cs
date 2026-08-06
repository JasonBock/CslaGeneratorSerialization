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

		context.RegisterSourceOutput(provider.Collect(),
			(context, source) => GeneratorSerializationGenerator.CreateOutput(source, context));
	}

	private static void CreateOutput(ImmutableArray<SerializationModel> models, SourceProductionContext context)
	{
		var propertyUnionTypes = new HashSet<TypeReferenceModel>();

		foreach (var model in models.Distinct())
		{
			var builder = new GeneratorSerializationBuilder(model);
			context.AddSource(builder.FileName, builder.Text);

			propertyUnionTypes.AddRange(
				model.Items
					.Where(model => model.PropertyInfoDataType.UnionCaseTypes.Length > 0)
					.Select(model => model.PropertyInfoDataType));
		}

		if (propertyUnionTypes.Count > 0)
		{
			var unionTypes = propertyUnionTypes.ToList();

			var startingIndex = 0;
			var endingIndex = unionTypes.Count;
			var startingCount = unionTypes.Count;

			do
			{
				startingCount = unionTypes.Count;

				for (var i = startingIndex; i <= endingIndex - 1; i++)
				{
					var unionType = unionTypes[i];

					foreach (var unionCaseType in unionType.UnionCaseTypes)
					{
						if (unionCaseType.UnionCaseTypes.Length > 0 &&
							!unionTypes.Contains(unionCaseType))
						{
							unionTypes.Add(unionCaseType);
						}
					}
				}

				if (unionTypes.Count > startingCount)
				{
					startingIndex = endingIndex;
					endingIndex = unionTypes.Count;
				}
			} while (startingCount != unionTypes.Count);

			var readerExtensionsBuilder = new GeneratorFormatterReaderContextExtensionsBuilder(unionTypes);
			context.AddSource(readerExtensionsBuilder.FileName, readerExtensionsBuilder.Text);

			var writerExtensionsBuilder = new GeneratorFormatterWriterContextExtensionsBuilder(unionTypes);
			context.AddSource(writerExtensionsBuilder.FileName, writerExtensionsBuilder.Text);
		}
	}
}