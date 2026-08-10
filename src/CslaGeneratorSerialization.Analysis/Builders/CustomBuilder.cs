using CslaGeneratorSerialization.Analysis.Extensions;
using CslaGeneratorSerialization.Analysis.Models;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class CustomBuilder
{
	internal static void BuildPropertyReader(IndentedTextWriter indentWriter, SerializationItemModel item) =>
		indentWriter.WriteLines(
			$$"""
			if (context.Reader.ReadStateValue() == global::CslaGeneratorSerialization.SerializationState.Value)
			{
				{{BuilderHelpers.GetLoadProperty(item, BuilderHelpers.GetReadOperation(item.PropertyInfoDataType))}}
			}
			""");

   internal static void BuildPropertyWriter(IndentedTextWriter indentWriter, TypeReferenceModel propertyType, string valueVariable) => 
		indentWriter.WriteLines(
		   $$"""
			if ({{valueVariable}} is not null)
			{
				context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Value);
				context.WriteCustom<{{propertyType.FullyQualifiedNameNoNullableAnnotation}}>({{valueVariable}});
			}
			else
			{
				context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Null);
			}
			""");

	internal static void BuildUnionReader(IndentedTextWriter indentWriter,
		TypeReferenceModel unionType, TypeReferenceModel unionCaseType)
	{
		var readOperation = BuilderHelpers.GetReadOperation(unionCaseType);
		indentWriter.WriteLines(
			$$"""
			return ({{unionType.FullyQualifiedName}}){{readOperation}};
			""");
	}

	internal static void BuildUnionWriter(IndentedTextWriter indentWriter, TypeReferenceModel propertyType, string valueVariable) =>
		indentWriter.WriteLines(
			$$"""
			context.WriteCustom<{{propertyType.FullyQualifiedNameNoNullableAnnotation}}>({{valueVariable}});
			""");
}