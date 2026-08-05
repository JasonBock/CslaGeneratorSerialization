using CslaGeneratorSerialization.Analysis.Extensions;
using CslaGeneratorSerialization.Analysis.Models;
using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class NullableValueTypeBuilder
{
	internal static void BuildPropertyReader(IndentedTextWriter indentWriter, SerializationItemModel item)
	{
		var propertyType = item.PropertyInfoDataType;

		var loadProperty = BuilderHelpers.GetLoadProperty(item,
			$"{BuilderHelpers.GetReadOperation(propertyType)}");

		indentWriter.WriteLines(
			$$"""
			if (context.Reader.ReadStateValue() == global::CslaGeneratorSerialization.SerializationState.Value)
			{
				{{loadProperty}}
			}
			""");
	}

	internal static void BuildWriter(IndentedTextWriter indentWriter, TypeReferenceModel propertyType, string valueVariable)
	{
		var valueToWrite = $"{valueVariable}.Value";

		var enumCast = propertyType.IsNullable && propertyType.TypeArguments[0].TypeKind == TypeKind.Enum ?
			$"({propertyType.TypeArguments[0].EnumUnderlyingType!.FullyQualifiedName})" : string.Empty;

		indentWriter.WriteLines(
			$$"""
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