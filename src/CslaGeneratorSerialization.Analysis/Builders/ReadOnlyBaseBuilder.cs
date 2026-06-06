using CslaGeneratorSerialization.Analysis.Extensions;
using CslaGeneratorSerialization.Analysis.Models;
using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class ReadOnlyBaseBuilder
{
	internal static void Build(IndentedTextWriter indentWriter, SerializationModel model)
	{
		indentWriter.Indent++;
		ReadOnlyBaseBuilder.BuildWriter(indentWriter, model);
		indentWriter.WriteLine();
		ReadOnlyBaseBuilder.BuildReader(indentWriter, model);
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
			OperationBuilder.BuildReadOperation(indentWriter, item, false);

			if (i < model.Items.Length - 1)
			{
				indentWriter.WriteLine();
			}
		}

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
			OperationBuilder.BuildWriteOperation(indentWriter, item, itemId++, false);

			if (i < model.Items.Length - 1)
			{
				indentWriter.WriteLine();
			}
		}

		SerializationBuilder.Build(indentWriter, model);

		indentWriter.Indent--;
		indentWriter.WriteLine("}");
	}
}