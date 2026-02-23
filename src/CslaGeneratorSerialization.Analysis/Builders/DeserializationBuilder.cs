using CslaGeneratorSerialization.Analysis.Extensions;
using CslaGeneratorSerialization.Analysis.Models;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class DeserializationBuilder
{
	internal static void Build(IndentedTextWriter indentWriter, SerializationModel model)
	{
		if (model.ImplementsMetastate || model.IsCustomizable || model.RequiresDeserializationNotification)
		{
			indentWriter.WriteLine();
		}

		if (model.ImplementsMetastate)
		{
			indentWriter.WriteLines(
				"""
				((global::Csla.Serialization.Mobile.IMobileObjectMetastate)this).SetMetastate(context.Reader.ReadByteArray());
				""");
		}

		if (model.IsCustomizable)
		{
			indentWriter.WriteLines(
				"""
				this.GetCustomState(context.Reader);
				""");
		}

		if (model.RequiresDeserializationNotification)
		{
			indentWriter.WriteLines(
				"""
				((global::Csla.Serialization.Mobile.ISerializationNotification)this).Deserialized();
				""");
		}
	}
}