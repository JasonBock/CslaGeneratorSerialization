using CslaGeneratorSerialization.Analysis.Extensions;
using CslaGeneratorSerialization.Analysis.Models;
using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

// These are for reading and writing
// from properties with managed backing fields.
internal static class OperationBuilder
{
	internal static void BuildUnionReadOperation(
		IndentedTextWriter indentWriter,
		TypeReferenceModel parentUnionType, TypeReferenceModel childUnionCaseType)
	{
		indentWriter.WriteLine($"// {childUnionCaseType.FullyQualifiedName}");

		UnionBuilder.BuildUnionReader(indentWriter, parentUnionType, childUnionCaseType);
	}

	internal static void BuildPropertyReadOperation(IndentedTextWriter indentWriter, SerializationItemModel item, 
		int itemId, bool includeCustom)
	{
		indentWriter.WriteLine($"// {item.PropertyInfoContainingType.FullyQualifiedName}.{item.PropertyInfoFieldName}");
		var propertyType = item.PropertyInfoDataType;

		if (!propertyType.UnionCaseTypes.IsEmpty)
		{
			UnionBuilder.BuildPropertyReader(indentWriter, item);
		}
		else if (propertyType.TypeKind == TypeKind.Enum)
		{
			EnumBuilder.BuildPropertyReader(indentWriter, item);
		}
		else if (propertyType.IsSupportedArray)
		{
			ArrayBuilder.BuildPropertyReader(indentWriter, item);
		}
		else if (propertyType.FullyQualifiedName == Shared.ClaimsPrincipalFullyQualifiedName)
		{
			ClaimsPrincipalBuilder.BuildPropertyReader(indentWriter, item);
		}
		else if (propertyType.BusinessObjectKind != StereotypeKind.None)
		{
			StereotypeBuilder.BuildPropertyReader(indentWriter, item);
		}
		else if (propertyType.IsNullable && propertyType.IsValueType)
		{
			NullableValueTypeBuilder.BuildPropertyReader(indentWriter, item);
		}
		else if (propertyType.SpecialType == SpecialType.System_String)
		{
			StringBuilder.BuildPropertyReader(indentWriter, item);
		}
		else if (propertyType.IsValueType)
		{
			ValueTypeBuilder.BuildPropertyReader(indentWriter, item);
		}
		else if (includeCustom)
		{
			CustomBuilder.BuildPropertyReader(indentWriter, item);
		}
	}

	internal static void BuildWriteOperation(IndentedTextWriter indentWriter, SerializationItemModel item, 
		int itemId, bool includeCustom)
	{
		// Note that all of the "Write" invocations should either be handled
		// natively by BinaryWriter or by an extension method I've created.
		var managedBackingField = $"{item.PropertyInfoContainingType.FullyQualifiedName}.{item.PropertyInfoFieldName}";
		var valueVariable = $"value{itemId}";
		var propertyType = item.PropertyInfoDataType;

		indentWriter.WriteLines(
			$"""
			// {managedBackingField}
			var {valueVariable} = this.ReadProperty<{propertyType.FullyQualifiedName}>({managedBackingField})!;
			""");
			
		if (!propertyType.UnionCaseTypes.IsEmpty)
		{
			UnionBuilder.BuildWriter(indentWriter, valueVariable, "new global::System.Collections.Generic.List<byte>()");
		}
		else if (propertyType.TypeKind == TypeKind.Enum)
		{
			EnumBuilder.BuildWriter(indentWriter, propertyType, valueVariable);
		}
		else if (propertyType.IsSupportedArray)
		{
			ArrayBuilder.BuildWriter(indentWriter, propertyType, valueVariable);
		}
		else if (propertyType.FullyQualifiedName == Shared.ClaimsPrincipalFullyQualifiedName)
		{
			ClaimsPrincipalBuilder.BuildWriter(indentWriter, propertyType, valueVariable);
		}
		else if (propertyType.BusinessObjectKind != StereotypeKind.None)
		{
			StereotypeBuilder.BuildWriter(indentWriter, propertyType, valueVariable);
		}
		else if (propertyType.IsNullable && propertyType.IsValueType)
		{
			NullableValueTypeBuilder.BuildWriter(indentWriter, propertyType, valueVariable);
		}
		else if (propertyType.SpecialType == SpecialType.System_String)
		{
			StringBuilder.BuildWriter(indentWriter, propertyType, valueVariable);
		}
		else if (propertyType.IsValueType)
		{
			ValueTypeBuilder.BuildWriter(indentWriter, propertyType, valueVariable);
		}
		else if (includeCustom)
		{
			CustomBuilder.BuildWriter(indentWriter, propertyType, valueVariable);
		}
	}
}