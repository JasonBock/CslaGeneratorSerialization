using CslaGeneratorSerialization.Extensions;
using CslaGeneratorSerialization.Models;
using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Builders;

internal static class NullableValueTypeBuilder
{
	internal static void BuildReader(IndentedTextWriter indentWriter, SerializationItemModel item)
	{
		var propertyType = item.PropertyInfoDataType;

		var nullableType = propertyType.TypeArguments[0];
		var enumCast = string.Empty;

		if (nullableType.TypeKind == TypeKind.Enum)
		{
			nullableType = nullableType.EnumUnderlyingType!;
			enumCast = $"({propertyType.TypeArguments[0].FullyQualifiedName})";
		}

		var loadProperty = BuilderHelpers.GetLoadProperty(item,
			$"{enumCast}{BuilderHelpers.GetReadOperation(nullableType)}");

		indentWriter.WriteLines(
			$$"""
			if (context.Reader.ReadStateValue() == global::CslaGeneratorSerialization.SerializationState.Value)
			{
				{{loadProperty}}
			}
			""");
	}

	internal static void BuildWriter(IndentedTextWriter indentWriter, TypeReferenceModel propertyType, string managedBackingField, string valueVariable)
	{
		var valueToWrite = $"{valueVariable}.Value";

		var enumCast = propertyType.IsNullable && propertyType.TypeArguments[0].TypeKind == TypeKind.Enum ?
			$"({propertyType.TypeArguments[0].EnumUnderlyingType!.FullyQualifiedName})" : string.Empty;

		indentWriter.WriteLines(
			$$"""
			var {{valueVariable}} = this.ReadProperty<{{propertyType.FullyQualifiedName}}>({{managedBackingField}});

			if ({{valueVariable}} is not null)
			{
				context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Value);
				context.Writer.Write({{enumCast}}{{valueToWrite}});
			}
			else
			{
				context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Null);
			}
			""");
	}
}