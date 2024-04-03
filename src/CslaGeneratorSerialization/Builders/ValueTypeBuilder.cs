using CslaGeneratorSerialization.Extensions;
using CslaGeneratorSerialization.Models;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Builders;

internal static class ValueTypeBuilder
{
   internal static void BuildReader(IndentedTextWriter indentWriter, SerializationItemModel item) => 
		indentWriter.WriteLine(
			$"{BuilderHelpers.GetLoadProperty(item, BuilderHelpers.GetReadOperation(item.PropertyInfoDataType))}");


   internal static void BuildWriter(IndentedTextWriter indentWriter, TypeReferenceModel propertyType, string managedBackingField) =>
		indentWriter.WriteLine($"context.Writer.Write(this.ReadProperty<{propertyType.FullyQualifiedName}>({managedBackingField}));");
}