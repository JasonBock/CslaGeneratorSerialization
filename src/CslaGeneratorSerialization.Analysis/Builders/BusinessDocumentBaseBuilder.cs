using CslaGeneratorSerialization.Analysis.Extensions;
using CslaGeneratorSerialization.Analysis.Models;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class BusinessDocumentBaseBuilder
{
	internal static void Build(IndentedTextWriter indentWriter, SerializationModel model)
	{
		indentWriter.Indent++;
		BusinessDocumentBaseBuilder.BuildWriter(indentWriter, model);
		indentWriter.WriteLine();
		BusinessDocumentBaseBuilder.BuildReader(indentWriter, model);
		indentWriter.Indent--;
	}

	private static void BuildReader(IndentedTextWriter indentWriter, SerializationModel model)
	{
		indentWriter.WriteLines(
			"""
			void global::CslaGeneratorSerialization.IGeneratorSerializable.GetState(global::CslaGeneratorSerialization.GeneratorFormatterReaderContext context)
			{
			""");
		indentWriter.Indent++;

		for (var i = 0; i < model.Items.Length; i++)
		{
			var item = model.Items[i];
			OperationBuilder.BuildReadOperation(indentWriter, item, true);
			indentWriter.WriteLine();
		}

		// Read the child items
		indentWriter.WriteLines(
			$$"""
			var count = context.Reader.ReadInt32();
			
			for (var i = 0; i < count; i++)
			{
				this.Add(context.Read<{{model.BusinessObject.BusinessObjectTarget!.FullyQualifiedNameNoNullableAnnotation}}>({{model.BusinessObject.BusinessObjectTarget!.IsSealed.ToString().ToLower()}})!);
			}
			""");

		DeserializationBuilder.Build(indentWriter, model);

		indentWriter.Indent--;
		indentWriter.WriteLine("}");
	}

	private static void BuildWriter(IndentedTextWriter indentWriter, SerializationModel model)
	{
		indentWriter.WriteLines(
			"""
			void global::CslaGeneratorSerialization.IGeneratorSerializable.SetState(global::CslaGeneratorSerialization.GeneratorFormatterWriterContext context)
			{
			""");
		indentWriter.Indent++;

		var itemId = 0;

		for (var i = 0; i < model.Items.Length; i++)
		{
			var item = model.Items[i];
			OperationBuilder.BuildWriteOperation(indentWriter, item, itemId++, true);
			indentWriter.WriteLine();
		}

		// Write the child items
		indentWriter.WriteLines(
			$$"""
			context.Writer.Write(this.Count);
			
			foreach (var item in this)
			{
				context.Write(item, {{model.BusinessObject.BusinessObjectTarget!.IsSealed.ToString().ToLower()}});
			}
			""");

		SerializationBuilder.Build(indentWriter, model);
		indentWriter.Indent--;
		indentWriter.WriteLine("}");
	}
}