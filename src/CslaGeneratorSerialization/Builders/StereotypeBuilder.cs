using CslaGeneratorSerialization.Extensions;
using CslaGeneratorSerialization.Models;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Builders;

internal static class StereotypeBuilder
{
	internal static void BuildReader(IndentedTextWriter indentWriter, SerializationItemModel item) => 
		indentWriter.WriteLines(
			$$"""
			this.LoadProperty({{item.PropertyInfoContainingType.FullyQualifiedName}}.{{item.PropertyInfoFieldName}}, context.Read<{{item.PropertyInfoDataType.FullyQualifiedNameNoNullableAnnotation}}>({{item.PropertyInfoDataType.IsSealed.ToString().ToLower()}})!);
			""");

	internal static void BuildWriter(IndentedTextWriter indentWriter, TypeReferenceModel propertyType, string managedBackingField) =>
		indentWriter.WriteLine(
			$"context.Write(this.ReadProperty<{propertyType.FullyQualifiedName}>({managedBackingField}), {propertyType.IsSealed.ToString().ToLower()});");
}