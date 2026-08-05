using CslaGeneratorSerialization.Analysis.Extensions;
using CslaGeneratorSerialization.Analysis.Models;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class UnionBuilder
{
	internal static void BuildUnionReader(IndentedTextWriter indentWriter, 
		TypeReferenceModel parentUnionType, TypeReferenceModel childUnionCaseType)
	{
		var readOperation = BuilderHelpers.GetReadOperation(childUnionCaseType);
		var constructUnion = $"new {parentUnionType.FullyQualifiedName}(({childUnionCaseType.FullyQualifiedName}){readOperation})";
		indentWriter.WriteLines(
			$$"""
			{{constructUnion}}
			""");
	}

	internal static void BuildPropertyReader(IndentedTextWriter indentWriter, SerializationItemModel item)
	{
		var loadProperty = BuilderHelpers.GetLoadProperty(item,
			$"{BuilderHelpers.GetReadOperation(item.PropertyInfoDataType)}");
		indentWriter.WriteLines(
			$$"""
			{
				var typeIdentifiers = context.Reader.ReadByteArray();
				var typeIdentifiersIndex = 0;
				{{loadProperty}}
			}
			""");
	}

	internal static void BuildWriter(IndentedTextWriter indentWriter, string valueVariable, string typeIdentifiersVariable) =>
		indentWriter.WriteLine($"context.WriteUnion({valueVariable}, {typeIdentifiersVariable});");
}