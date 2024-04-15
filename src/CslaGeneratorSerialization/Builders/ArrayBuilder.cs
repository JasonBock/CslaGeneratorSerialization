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

		if (propertyType.Array!.Rank == 1 &&
			(propertyType.Array.ElementType.SpecialType == SpecialType.System_Byte || propertyType.Array.ElementType.SpecialType == SpecialType.System_Char))
		{
			var elementSpecialType = propertyType.Array.ElementType.SpecialType;
			var readType = elementSpecialType == SpecialType.System_Byte ? "Byte" : "Char";
			var loadProperty = BuilderHelpers.GetLoadProperty(item, $"context.Reader.Read{readType}Array()");

			indentWriter.WriteLines(
				$$"""
				if (context.Reader.ReadStateValue() == global::CslaGeneratorSerialization.SerializationState.Value)
				{
					{{loadProperty}}
				}
				""");
		}
		else
		{
			CustomBuilder.BuildReader(indentWriter, item);
		}
	}

	internal static void BuildWriter(IndentedTextWriter indentWriter, TypeReferenceModel propertyType, string managedBackingField, string valueVariable)
	{
		if (propertyType.Array!.Rank == 1 &&
			(propertyType.Array.ElementType.SpecialType == SpecialType.System_Byte || propertyType.Array.ElementType.SpecialType == SpecialType.System_Char))
		{
			indentWriter.WriteLines(
				$$"""
				var {{valueVariable}} = this.ReadProperty<{{propertyType.FullyQualifiedName}}>({{managedBackingField}});

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
			CustomBuilder.BuildWriter(indentWriter, propertyType, managedBackingField);
		}
	}
}