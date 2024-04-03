using CslaGeneratorSerialization.Extensions;
using CslaGeneratorSerialization.Models;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Builders;

internal static class ListOfIntBuilder
{
	internal static void BuildReader(IndentedTextWriter indentWriter, SerializationItemModel item) =>
		indentWriter.WriteLines(
			$$"""
			if (context.Reader.ReadStateValue() == global::CslaGeneratorSerialization.SerializationState.Value)
			{
				{{BuilderHelpers.GetLoadProperty(item, "context.Reader.ReadListOfInt32()")}}
			}
			""");

	internal static void BuildWriter(IndentedTextWriter indentWriter, TypeReferenceModel propertyType, string managedBackingField, string valueVariable) => 
		indentWriter.WriteLines(
			$$"""
			var {{valueVariable}} = this.ReadProperty<{{propertyType.FullyQualifiedName}}>({{managedBackingField}});

			if ({{valueVariable}} is not null)
			{
				context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Value);
				context.Writer.Write({{valueVariable}});
			}
			else
			{
				context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Null);
			}
			""");
}