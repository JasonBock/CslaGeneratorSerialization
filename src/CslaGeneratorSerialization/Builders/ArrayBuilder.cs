using CslaGeneratorSerialization.Extensions;
using CslaGeneratorSerialization.Models;
using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Builders;

internal static class ArrayBuilder
{
	internal static void BuildReader(IndentedTextWriter indentWriter, SerializationItemModel item)
	{
		var propertyType = item.PropertyInfoDataType;
		var elementSpecialType = propertyType.Array!.ElementType.SpecialType;

		var arrayMethod = propertyType.Array.Rank == 1 ? "Array" : "ArrayArray";
		var readType = elementSpecialType == SpecialType.System_Byte ? "Byte" : "Char";
		var loadProperty = BuilderHelpers.GetLoadProperty(item, $"context.Reader.Read{readType}{arrayMethod}()");

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
		var valueToWrite = propertyType.Array!.Rank == 1 ?
			$"({valueVariable}.Length, {valueVariable})" : valueVariable;

		indentWriter.WriteLines(
			$$"""
			var {{valueVariable}} = this.ReadProperty<{{propertyType.FullyQualifiedName}}>({{managedBackingField}});

			if ({{valueVariable}} is not null)
			{
				context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Value);
				context.Writer.Write({{valueToWrite}});
			}
			else
			{
				context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Null);
			}
			""");
	}
}