using CslaGeneratorSerialization.Analysis.Extensions;
using CslaGeneratorSerialization.Analysis.Models;
using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class NullableValueTypeBuilder
{
	internal static void BuildUnionReader(IndentedTextWriter indentWriter,
		TypeReferenceModel unionType, TypeReferenceModel unionCaseType)
	{
		var readOperation = BuilderHelpers.GetReadOperation(unionCaseType);
		indentWriter.WriteLines(
			$$"""
			if (context.Reader.ReadStateValue() == global::CslaGeneratorSerialization.SerializationState.Value)
			{
				return ({{unionType.FullyQualifiedName}}){{readOperation}};
			}

			return ({{unionType.FullyQualifiedName}})(null as {{unionCaseType.FullyQualifiedName}})!;
			""");
	}

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

	internal static void BuildUnionWriter(IndentedTextWriter indentWriter, TypeReferenceModel unionCaseType, string valueVariable)
	{
		var enumCast = unionCaseType.IsNullable && unionCaseType.TypeArguments[0].TypeKind == TypeKind.Enum ?
			$"({unionCaseType.TypeArguments[0].EnumUnderlyingType!.FullyQualifiedName})" : 
			string.Empty;

		indentWriter.WriteLines(
			$$"""
			context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Value);
			context.Writer.Write({{enumCast}}{{valueVariable}});
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