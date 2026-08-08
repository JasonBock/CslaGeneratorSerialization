using CslaGeneratorSerialization.Analysis.Extensions;
using CslaGeneratorSerialization.Analysis.Models;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class EnumBuilder
{
	internal static void BuildUnionReader(IndentedTextWriter indentWriter, 
		TypeReferenceModel unionType, TypeReferenceModel unionCaseType)
	{
		var readOperation = BuilderHelpers.GetReadOperation(unionCaseType.EnumUnderlyingType!);
		indentWriter.WriteLine($"return ({unionType.FullyQualifiedName}){readOperation};");
	}

	internal static void BuildPropertyReader(IndentedTextWriter indentWriter, SerializationItemModel item)
	{
		var loadProperty = BuilderHelpers.GetLoadProperty(item,
			$"({item.PropertyInfoDataType.FullyQualifiedName}){BuilderHelpers.GetReadOperation(item.PropertyInfoDataType.EnumUnderlyingType!)}");
		indentWriter.WriteLines(
			$$"""
			{{loadProperty}}
			""");
	}

	internal static void BuildWriter(IndentedTextWriter indentWriter, TypeReferenceModel propertyType, string valueVariable) =>
		indentWriter.WriteLine($"context.Writer.Write(({propertyType.EnumUnderlyingType!.FullyQualifiedName}){valueVariable});");
}