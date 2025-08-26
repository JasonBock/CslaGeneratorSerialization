using CslaGeneratorSerialization.Analysis.Models;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class StringBuilder
{
	internal static void BuildReader(IndentedTextWriter indentWriter, SerializationItemModel item) =>
		indentWriter.WriteLine($"{BuilderHelpers.GetLoadProperty(item, "context.Reader.ReadString()")}");


	internal static void BuildWriter(IndentedTextWriter indentWriter, TypeReferenceModel propertyType, string managedBackingField) =>
		indentWriter.WriteLine($"context.Writer.Write(this.ReadProperty<{propertyType.FullyQualifiedName}>({managedBackingField}));");
}