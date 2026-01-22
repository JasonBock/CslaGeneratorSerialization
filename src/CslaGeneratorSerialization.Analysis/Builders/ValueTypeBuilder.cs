using CslaGeneratorSerialization.Analysis.Models;
using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class ValueTypeBuilder
{
	internal static void BuildReader(IndentedTextWriter indentWriter, SerializationItemModel item) =>
		indentWriter.WriteLine(
			$"{BuilderHelpers.GetLoadProperty(item, BuilderHelpers.GetReadOperation(item.PropertyInfoDataType))}");

	internal static void BuildWriter(IndentedTextWriter indentWriter, TypeReferenceModel propertyType, string managedBackingField)
	{
		if (propertyType.FullyQualifiedName == "global::System.Guid" ||
			propertyType.FullyQualifiedName == "global::System.Decimal" ||
			propertyType.FullyQualifiedName == "global::System.TimeSpan" ||
			propertyType.FullyQualifiedName == "global::System.DateTimeOffset" ||
			propertyType.FullyQualifiedName == "global::System.Half" ||
			propertyType.SpecialType == SpecialType.System_Boolean ||
			propertyType.SpecialType == SpecialType.System_Char ||
			propertyType.SpecialType == SpecialType.System_String ||
			propertyType.SpecialType == SpecialType.System_SByte ||
			propertyType.SpecialType == SpecialType.System_Byte ||
			propertyType.SpecialType == SpecialType.System_Int16 ||
			propertyType.SpecialType == SpecialType.System_UInt16 ||
			propertyType.SpecialType == SpecialType.System_Int32 ||
			propertyType.SpecialType == SpecialType.System_UInt32 ||
			propertyType.SpecialType == SpecialType.System_Int64 ||
			propertyType.SpecialType == SpecialType.System_UInt64 ||
			propertyType.SpecialType == SpecialType.System_Single ||
			propertyType.SpecialType == SpecialType.System_Double ||
			propertyType.SpecialType == SpecialType.System_Decimal ||
			propertyType.SpecialType == SpecialType.System_DateTime)
		{
			indentWriter.WriteLine($"context.Writer.Write(this.ReadProperty<{propertyType.FullyQualifiedNameNoNullableAnnotation}>({managedBackingField}));");
		}
		else
		{
			indentWriter.WriteLine($"context.WriteCustom<{propertyType.FullyQualifiedNameNoNullableAnnotation}>(this.ReadProperty<{propertyType.FullyQualifiedNameNoNullableAnnotation}>({managedBackingField}));");
		}
	}
}