using CslaGeneratorSerialization.Analysis.Models;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class UnionBuilder
{
	internal static void BuildReader(IndentedTextWriter indentWriter, SerializationItemModel item) { }

	internal static void BuildWriter(IndentedTextWriter indentWriter, TypeReferenceModel propertyType, string managedBackingField, string valueVariable) { }
}