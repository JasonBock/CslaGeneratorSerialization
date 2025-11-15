using CslaGeneratorSerialization.Analysis.Extensions;
using CslaGeneratorSerialization.Analysis.Models;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class BusinessListBaseBuilder
{
	internal static void Build(IndentedTextWriter indentWriter, SerializationModel model)
	{
		indentWriter.Indent++;
		BusinessListBaseBuilder.BuildWriter(indentWriter, model);
		indentWriter.WriteLine();
		BusinessListBaseBuilder.BuildReader(indentWriter, model);
		indentWriter.Indent--;
	}

	private static void BuildReader(IndentedTextWriter indentWriter, SerializationModel model)
	{
		indentWriter.WriteLines(
			$$"""
			void global::CslaGeneratorSerialization.IGeneratorSerializable.GetState(global::CslaGeneratorSerialization.GeneratorFormatterReaderContext context)
			{
				var count = context.Reader.ReadInt32();

				for (var i = 0; i < count; i++)
				{
					this.Add(context.Read<{{model.BusinessObject.BusinessObjectTarget!.FullyQualifiedNameNoNullableAnnotation}}>({{model.BusinessObject.BusinessObjectTarget!.IsSealed.ToString().ToLower()}})!);
				}

				var deletedCount = context.Reader.ReadInt32();

				for (var d = 0; d < deletedCount; d++)
				{
					this.DeletedList.Add(({{model.BusinessObject.BusinessObjectTarget!.FullyQualifiedNameNoNullableAnnotation}})context.GetReference(context.Reader.ReadInt32())!);
				}
			""");

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

		indentWriter.WriteLine("}");
	}

	// I assume that the items in the deleted list are in the list itself,
	// so they should be references.
	private static void BuildWriter(IndentedTextWriter indentWriter, SerializationModel model)
   {
		indentWriter.WriteLines(
			$$"""
			void global::CslaGeneratorSerialization.IGeneratorSerializable.SetState(global::CslaGeneratorSerialization.GeneratorFormatterWriterContext context)
			{
				context.Writer.Write(this.Count);
			
				foreach (var item in this)
				{
					context.Write(item, {{model.BusinessObject.BusinessObjectTarget!.IsSealed.ToString().ToLower()}});
				}

				context.Writer.Write(this.DeletedList.Count);

				foreach (var deletedItem in this.DeletedList)
				{
					(_, var deletedReferenceId) = context.GetReference(deletedItem);
					context.Writer.Write(deletedReferenceId);
				}
			""");

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

		indentWriter.WriteLine("}");
	}
}