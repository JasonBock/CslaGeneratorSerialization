using CslaGeneratorSerialization.Analysis.Extensions;
using CslaGeneratorSerialization.Analysis.Models;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class UnionBuilder
{
   internal static void BuildReader(IndentedTextWriter indentWriter, SerializationItemModel item) => 
		indentWriter.WriteLine($"context.ReadUnion<{item.PropertyInfoDataType.FullyQualifiedName}>();");

   internal static void BuildWriter(IndentedTextWriter indentWriter, TypeReferenceModel propertyType, string valueVariable) => 
		indentWriter.WriteLine($"context.WriteUnion({valueVariable});");
}