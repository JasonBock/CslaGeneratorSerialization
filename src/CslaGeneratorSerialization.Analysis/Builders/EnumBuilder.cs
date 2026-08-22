using CslaGeneratorSerialization.Analysis.Extensions;
using CslaGeneratorSerialization.Analysis.Models;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class EnumBuilder
{
	internal static void BuildUnionReader(IndentedTextWriter indentWriter, 
		ITypeReferenceModel unionType, ITypeReferenceModel unionCaseType)
	{
		var readOperation = BuilderHelpers.GetReadOperation(unionCaseType.EnumUnderlyingType!);
		indentWriter.WriteLine($"return ({unionType.FullyQualifiedName})({unionCaseType.FullyQualifiedName}){readOperation};");
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

	internal static void BuildWriter(IndentedTextWriter indentWriter, ITypeReferenceModel propertyType, string valueVariable) =>
		indentWriter.WriteLine($"context.Writer.Write(({propertyType.EnumUnderlyingType!.FullyQualifiedName}){valueVariable});");
}