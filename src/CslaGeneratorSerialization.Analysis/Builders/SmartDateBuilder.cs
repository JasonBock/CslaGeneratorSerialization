using CslaGeneratorSerialization.Analysis.Extensions;
using CslaGeneratorSerialization.Analysis.Models;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class SmartDateBuilder
{
	internal static void BuildReader(IndentedTextWriter indentWriter, SerializationItemModel item) =>
		indentWriter.WriteLines(
				$$"""
				this.LoadProperty({{item.PropertyInfoContainingType.FullyQualifiedName}}.{{item.PropertyInfoFieldName}}, context.ReadMobileObjectStruct<{{item.PropertyInfoDataType.FullyQualifiedNameNoNullableAnnotation}}>()!);
				""");

	internal static void BuildWriter(IndentedTextWriter indentWriter, TypeReferenceModel propertyType, string managedBackingField) =>
			indentWriter.WriteLine(
				$"context.WriteMobileObject(this.ReadProperty<{propertyType.FullyQualifiedName}>({managedBackingField}));");
}