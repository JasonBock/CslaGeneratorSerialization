using CslaGeneratorSerialization.Analysis.Extensions;
using CslaGeneratorSerialization.Analysis.Models;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class UnionBuilder
{
	internal static void BuildUnionReader(IndentedTextWriter indentWriter, 
		ITypeReferenceModel unionType, ITypeReferenceModel unionCaseType)
	{
		var readOperation = BuilderHelpers.GetReadOperation(unionCaseType);
		indentWriter.WriteLine($"return ({unionType.FullyQualifiedName}){readOperation};");
	}

	internal static void BuildPropertyReader(IndentedTextWriter indentWriter, SerializationItemModel item)
	{
		var loadProperty = BuilderHelpers.GetLoadProperty(item,
			$"{BuilderHelpers.GetReadOperation(item.PropertyInfoDataType)}");
		indentWriter.WriteLines(
			$$"""
			{
				var typeIdentifiers = context.Reader.ReadUInt32Array();
				var typeIdentifiersIndex = -1;
				{{loadProperty}}
			}
			""");
	}

	internal static void BuildWriter(IndentedTextWriter indentWriter, string valueVariable, string typeIdentifiersVariable) =>
		indentWriter.WriteLine($"context.WriteUnion({valueVariable}, {typeIdentifiersVariable});");
}