using CslaGeneratorSerialization.Analysis.Extensions;
using CslaGeneratorSerialization.Analysis.Models;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class CustomBuilder
{
	internal static void BuildUnionReader(IndentedTextWriter indentWriter,
		TypeReferenceModel unionType, TypeReferenceModel unionCaseType)
	{
		var readOperation = BuilderHelpers.GetReadOperation(unionCaseType);
		indentWriter.WriteLine($"return ({unionType.FullyQualifiedName}){readOperation};");
	}

	internal static void BuildPropertyReader(IndentedTextWriter indentWriter, SerializationItemModel item) 
	{
		var loadProperty = BuilderHelpers.GetLoadProperty(item,
			BuilderHelpers.GetReadOperation(item.PropertyInfoDataType));
		indentWriter.WriteLines(
			$$"""
			{{loadProperty}}
			""");
	}

	internal static void BuildWriter(IndentedTextWriter indentWriter, TypeReferenceModel propertyType, string valueVariable) =>
		indentWriter.WriteLine($"context.WriteCustom<{propertyType.FullyQualifiedNameNoNullableAnnotation}>({valueVariable});");
}