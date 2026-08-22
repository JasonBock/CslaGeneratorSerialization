using CslaGeneratorSerialization.Analysis.Extensions;
using CslaGeneratorSerialization.Analysis.Models;
using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

// These are for reading and writing
// from properties with managed backing fields.
internal static class OperationBuilder
{
	internal static void BuildPropertyReadOperation(IndentedTextWriter indentWriter, SerializationItemModel item, bool includeCustom)
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

	internal static void BuildPropertyWriteOperation(IndentedTextWriter indentWriter, SerializationItemModel item, 
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
			UnionBuilder.BuildWriter(indentWriter, valueVariable, "new global::System.Collections.Generic.List<uint>()");
		}
		else if (propertyType.TypeKind == TypeKind.Enum)
		{
			EnumBuilder.BuildWriter(indentWriter, propertyType, valueVariable);
		}
		else if (propertyType.IsSupportedArray)
		{
			ArrayBuilder.BuildPropertyWriter(indentWriter, propertyType, valueVariable);
		}
		else if (propertyType.FullyQualifiedName == Shared.ClaimsPrincipalFullyQualifiedName)
		{
			ClaimsPrincipalBuilder.BuildPropertyWriter(indentWriter, propertyType, valueVariable);
		}
		else if (propertyType.BusinessObjectKind != StereotypeKind.None)
		{
			StereotypeBuilder.BuildWriter(indentWriter, propertyType, valueVariable);
		}
		else if (propertyType.IsNullable && propertyType.IsValueType)
		{
			NullableValueTypeBuilder.BuildPropertyWriter(indentWriter, propertyType, valueVariable);
		}
		else if (propertyType.SpecialType == SpecialType.System_String)
		{
			StringBuilder.BuildPropertyWriter(indentWriter, propertyType, valueVariable);
		}
		else if (propertyType.IsValueType)
		{
			ValueTypeBuilder.BuildWriter(indentWriter, propertyType, valueVariable);
		}
		else if (includeCustom)
		{
			CustomBuilder.BuildPropertyWriter(indentWriter, propertyType, valueVariable);
		}
	}

	internal static void BuildUnionReadOperation(
		IndentedTextWriter indentWriter,
		ITypeReferenceModel unionType, ITypeReferenceModel unionCaseType, bool includeCustom)
	{
		indentWriter.WriteLine($"// {unionCaseType.FullyQualifiedName}");

		if (!unionCaseType.UnionCaseTypes.IsEmpty)
		{
			UnionBuilder.BuildUnionReader(indentWriter, unionType, unionCaseType);
		}
		else if (unionCaseType.TypeKind == TypeKind.Enum)
		{
			EnumBuilder.BuildUnionReader(indentWriter, unionType, unionCaseType);
		}
		else if (unionCaseType.IsSupportedArray)
		{
			ArrayBuilder.BuildUnionReader(indentWriter, unionType, unionCaseType);
		}
		else if (unionCaseType.FullyQualifiedName == Shared.ClaimsPrincipalFullyQualifiedName)
		{
			ClaimsPrincipalBuilder.BuildUnionReader(indentWriter, unionType);
		}
		else if (unionCaseType.BusinessObjectKind != StereotypeKind.None)
		{
			StereotypeBuilder.BuildUnionReader(indentWriter, unionType, unionCaseType);
		}
		else if (unionCaseType.IsNullable && unionType.IsValueType)
		{
			NullableValueTypeBuilder.BuildUnionReader(indentWriter, unionType, unionCaseType);
		}
		else if (unionCaseType.SpecialType == SpecialType.System_String)
		{
			StringBuilder.BuildUnionReader(indentWriter, unionType, unionCaseType);
		}
		else if (unionCaseType.IsValueType)
		{
			ValueTypeBuilder.BuildUnionReader(indentWriter, unionType, unionCaseType);
		}
		else if (includeCustom)
		{
			CustomBuilder.BuildUnionReader(indentWriter, unionType, unionCaseType);
		}
	}

	internal static void BuildUnionWriteOperation(IndentedTextWriter indentWriter, ITypeReferenceModel unionCaseType, 
		string valueVariable, bool includeCustom)
	{
		if (!unionCaseType.UnionCaseTypes.IsEmpty)
		{
			UnionBuilder.BuildWriter(indentWriter, valueVariable, "typeIdentifiers");
		}
		else if (unionCaseType.TypeKind == TypeKind.Enum)
		{
			EnumBuilder.BuildWriter(indentWriter, unionCaseType, valueVariable);
		}
		else if (unionCaseType.IsSupportedArray)
		{
			ArrayBuilder.BuildUnionWriter(indentWriter, unionCaseType, valueVariable);
		}
		else if (unionCaseType.FullyQualifiedName == Shared.ClaimsPrincipalFullyQualifiedName)
		{
			ClaimsPrincipalBuilder.BuildUnionWriter(indentWriter, unionCaseType, valueVariable);
		}
		else if (unionCaseType.BusinessObjectKind != StereotypeKind.None)
		{
			StereotypeBuilder.BuildWriter(indentWriter, unionCaseType, valueVariable);
		}
		else if (unionCaseType.IsNullable && unionCaseType.IsValueType)
		{
			NullableValueTypeBuilder.BuildUnionWriter(indentWriter, unionCaseType, valueVariable);
		}
		else if (unionCaseType.SpecialType == SpecialType.System_String)
		{
			StringBuilder.BuildUnionWriter(indentWriter, unionCaseType, valueVariable);
		}
		else if (unionCaseType.IsValueType)
		{
			ValueTypeBuilder.BuildWriter(indentWriter, unionCaseType, valueVariable);
		}
		else if (includeCustom)
		{
			CustomBuilder.BuildUnionWriter(indentWriter, unionCaseType, valueVariable);
		}
	}
}