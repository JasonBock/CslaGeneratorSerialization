using CslaGeneratorSerialization.Analysis.Extensions;
using CslaGeneratorSerialization.Analysis.Models;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class SerializationBuilder
{
	internal static void Build(IndentedTextWriter indentWriter, SerializationModel model)
	{
		if(model.ImplementsMetastate || model.IsCustomizable)
		{
			indentWriter.WriteLine();
		}

		if (model.ImplementsMetastate)
		{
			indentWriter.WriteLines(
				"""
				var metastate = ((global::Csla.Serialization.Mobile.IMobileObjectMetastate)this).GetMetastate();
				context.Writer.Write((metastate.Length, metastate));
				""");
		}

		if (model.IsCustomizable)
		{
			indentWriter.WriteLines(
				"""
				this.SetCustomState(context.Writer);
				""");
		}
	}
}