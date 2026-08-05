using CslaGeneratorSerialization.Analysis.Extensions;
using CslaGeneratorSerialization.Analysis.Models;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class StereotypeBuilder
{
   internal static void BuildPropertyReader(IndentedTextWriter indentWriter, SerializationItemModel item) => 
		indentWriter.WriteLines(
		   $$"""
			this.LoadProperty({{item.PropertyInfoContainingType.FullyQualifiedName}}.{{item.PropertyInfoFieldName}}, {{BuilderHelpers.GetReadOperation(item.PropertyInfoDataType)}});
			""");

   internal static void BuildWriter(IndentedTextWriter indentWriter, TypeReferenceModel propertyType, string valueVariable)
	{
		if (propertyType.ParticipatesInGeneratorSerialization)
		{
			indentWriter.WriteLine(
				$"context.Write({valueVariable}, {propertyType.IsSealed.ToString().ToLower()});");
		}
		else
		{
			indentWriter.WriteLine(
				$"context.WriteMobileObject({valueVariable});");
		}
	}
}