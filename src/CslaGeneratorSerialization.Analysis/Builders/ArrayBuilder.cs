using CslaGeneratorSerialization.Analysis.Extensions;
using CslaGeneratorSerialization.Analysis.Models;
using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class ArrayBuilder
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

			return ({{unionType.FullyQualifiedName}})(null as {{unionCaseType.FullyQualifiedName}});
			""");
	}

	internal static void BuildPropertyReader(IndentedTextWriter indentWriter, SerializationItemModel item)
	{
		var propertyType = item.PropertyInfoDataType;
		var loadProperty = BuilderHelpers.GetLoadProperty(item, BuilderHelpers.GetReadOperation(propertyType));

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
		if (propertyType.Array!.Rank == 1 &&
			(propertyType.Array.ElementType.SpecialType == SpecialType.System_Byte || propertyType.Array.ElementType.SpecialType == SpecialType.System_Char))
		{
			indentWriter.WriteLines(
				$$"""
				if ({{valueVariable}} is not null)
				{
					context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Value);
					context.Writer.Write(({{valueVariable}}.Length, {{valueVariable}}));
				}
				else
				{
					context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Null);
				}
				""");
		}
		else
		{
			CustomBuilder.BuildWriter(indentWriter, propertyType, valueVariable);
		}
	}
}