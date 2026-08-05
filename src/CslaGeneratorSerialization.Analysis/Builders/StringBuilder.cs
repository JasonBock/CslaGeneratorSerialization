using CslaGeneratorSerialization.Analysis.Extensions;
using CslaGeneratorSerialization.Analysis.Models;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class StringBuilder
{
   internal static void BuildReader(IndentedTextWriter indentWriter, SerializationItemModel item) => 
		indentWriter.WriteLines(
		   $$"""
			if (context.Reader.ReadStateValue() == global::CslaGeneratorSerialization.SerializationState.Value)
			{
				{{BuilderHelpers.GetLoadProperty(item, BuilderHelpers.GetReadOperation(item.PropertyInfoDataType))}}
			}
			""");

   internal static void BuildWriter(IndentedTextWriter indentWriter, TypeReferenceModel propertyType, string valueVariable) =>
		indentWriter.WriteLines(
			$$"""
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