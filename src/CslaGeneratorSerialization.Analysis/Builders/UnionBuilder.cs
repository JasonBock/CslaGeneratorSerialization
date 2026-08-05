using CslaGeneratorSerialization.Analysis.Extensions;
using CslaGeneratorSerialization.Analysis.Models;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class UnionBuilder
{
   internal static void BuildReader(IndentedTextWriter indentWriter, SerializationItemModel item) => 
		indentWriter.WriteLine(
			$"this.LoadProperty({item.PropertyInfoContainingType.FullyQualifiedName}.{item.PropertyInfoFieldName}, {BuilderHelpers.GetReadOperation(item.PropertyInfoDataType)});");

   internal static void BuildWriter(IndentedTextWriter indentWriter, TypeReferenceModel propertyType, string valueVariable) => 
		indentWriter.WriteLine($"context.WriteUnion({valueVariable});");
}