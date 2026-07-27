using CslaGeneratorSerialization.Analysis.Extensions;
using CslaGeneratorSerialization.Analysis.Models;
using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class OperationBuilder
{
	internal static void BuildReadOperation(IndentedTextWriter indentWriter, SerializationItemModel item, bool includeCustom)
	{
		indentWriter.WriteLine($"// {item.PropertyInfoContainingType.FullyQualifiedName}.{item.PropertyInfoFieldName}");
		var propertyType = item.PropertyInfoDataType;

		if (!propertyType.UnionCaseTypes.IsEmpty)
		{
			// This is a union type.
			//UnionBuilder.BuildReader(...);
		}
		else if (propertyType.TypeKind == TypeKind.Enum)
		{
			EnumBuilder.BuildReader(indentWriter, item);
		}
		else if (propertyType.Array is not null)
		{
			ArrayBuilder.BuildReader(indentWriter, item);
		}
		else if (propertyType.FullyQualifiedName == Shared.ClaimsPrincipalFullyQualifiedName)
		{
			ClaimsPrincipalBuilder.BuildReader(indentWriter, item);
		}
		else if (propertyType.BusinessObjectKind != StereotypeKind.None)
		{
			StereotypeBuilder.BuildReader(indentWriter, item);
		}
		else if (propertyType.IsNullable && propertyType.IsValueType)
		{
			NullableValueTypeBuilder.BuildReader(indentWriter, item);
		}
		else if (propertyType.SpecialType == SpecialType.System_String)
		{
			StringBuilder.BuildReader(indentWriter, item);
		}
		else if (propertyType.IsValueType)
		{
			ValueTypeBuilder.BuildReader(indentWriter, item);
		}
		else if (includeCustom)
		{
			CustomBuilder.BuildReader(indentWriter, item);
		}
	}

	internal static void BuildWriteOperation(IndentedTextWriter indentWriter, SerializationItemModel item, int itemId, bool includeCustom)
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
			// This is a union type.
			//UnionBuilder.BuildWriter(...);
		}
		else if (propertyType.TypeKind == TypeKind.Enum)
		{
			EnumBuilder.BuildWriter(indentWriter, propertyType, valueVariable);
		}
		else if (propertyType.Array is not null)
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