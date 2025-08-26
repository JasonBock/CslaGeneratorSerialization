using CslaGeneratorSerialization.Analysis.Extensions;
using CslaGeneratorSerialization.Analysis.Models;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class EnumBuilder
{
	internal static void BuildReader(IndentedTextWriter indentWriter, SerializationItemModel item) 
	{
		var loadProperty = BuilderHelpers.GetLoadProperty(item,
			$"({item.PropertyInfoDataType.FullyQualifiedName}){BuilderHelpers.GetReadOperation(item.PropertyInfoDataType.EnumUnderlyingType!)}");
		indentWriter.WriteLines(
			$$"""
			{{loadProperty}}
			""");
	}

	internal static void BuildWriter(IndentedTextWriter indentWriter, TypeReferenceModel propertyType, string managedBackingField) =>
		indentWriter.WriteLine($"context.Writer.Write(({propertyType.EnumUnderlyingType!.FullyQualifiedName})this.ReadProperty<{propertyType.FullyQualifiedName}>({managedBackingField}));");
}