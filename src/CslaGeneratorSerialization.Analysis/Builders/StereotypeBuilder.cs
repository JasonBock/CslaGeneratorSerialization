using CslaGeneratorSerialization.Analysis.Extensions;
using CslaGeneratorSerialization.Analysis.Models;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class StereotypeBuilder
{
	internal static void BuildReader(IndentedTextWriter indentWriter, SerializationItemModel item)
	{
		if (item.PropertyInfoDataType.ParticipatesInGeneratorSerialization)
		{
			indentWriter.WriteLines(
				$$"""
				this.LoadProperty({{item.PropertyInfoContainingType.FullyQualifiedName}}.{{item.PropertyInfoFieldName}}, context.Read<{{item.PropertyInfoDataType.FullyQualifiedNameNoNullableAnnotation}}>({{item.PropertyInfoDataType.IsSealed.ToString().ToLower()}})!);
				""");
		}
		else
		{
			indentWriter.WriteLines(
				$$"""
				this.LoadProperty({{item.PropertyInfoContainingType.FullyQualifiedName}}.{{item.PropertyInfoFieldName}}, context.ReadMobileObject<{{item.PropertyInfoDataType.FullyQualifiedNameNoNullableAnnotation}}>()!);
				""");
		}
	}

	internal static void BuildWriter(IndentedTextWriter indentWriter, TypeReferenceModel propertyType, string valueVariable)
	{
		if (propertyType.ParticipatesInGeneratorSerialization)
		{
			indentWriter.WriteLine(
				$"context.Write({valueVariable}, {propertyType.IsSealed.ToString().ToLower()});");
		}
		else
		{
			indentWriter.WriteLine(
				$"context.WriteMobileObject({valueVariable});");
		}
	}
}