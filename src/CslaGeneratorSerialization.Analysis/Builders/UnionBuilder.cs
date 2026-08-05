using CslaGeneratorSerialization.Analysis.Extensions;
using CslaGeneratorSerialization.Analysis.Models;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class UnionBuilder
{
	internal static void BuildReader(IndentedTextWriter indentWriter, SerializationItemModel item)
	{
		var loadProperty = BuilderHelpers.GetLoadProperty(item,
			$"{BuilderHelpers.GetReadOperation(item.PropertyInfoDataType)}");
		indentWriter.WriteLines(
			$$"""
			{{loadProperty}}
			""");
	}

	internal static void BuildWriter(IndentedTextWriter indentWriter, TypeReferenceModel propertyType, string valueVariable) =>
		indentWriter.WriteLine($"context.WriteUnion({valueVariable});");
}