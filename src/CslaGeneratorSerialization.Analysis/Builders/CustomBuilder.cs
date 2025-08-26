using CslaGeneratorSerialization.Analysis.Extensions;
using CslaGeneratorSerialization.Analysis.Models;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class CustomBuilder
{
	internal static void BuildReader(IndentedTextWriter indentWriter, SerializationItemModel item) 
	{
		var loadProperty = BuilderHelpers.GetLoadProperty(item,
			$"context.ReadCustom<{item.PropertyInfoDataType.FullyQualifiedNameNoNullableAnnotation}>()");
		indentWriter.WriteLines(
			$$"""
			{{loadProperty}}
			""");
	}

	internal static void BuildWriter(IndentedTextWriter indentWriter, TypeReferenceModel propertyType, string managedBackingField) =>
		indentWriter.WriteLine($"context.WriteCustom<{propertyType.FullyQualifiedNameNoNullableAnnotation}>(this.ReadProperty<{propertyType.FullyQualifiedNameNoNullableAnnotation}>({managedBackingField}));");
}