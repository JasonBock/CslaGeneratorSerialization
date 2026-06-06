using CslaGeneratorSerialization.Analysis.Extensions;
using CslaGeneratorSerialization.Analysis.Models;
using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class BusinessBaseBuilder
{
	internal static void Build(IndentedTextWriter indentWriter, SerializationModel model)
	{
		indentWriter.Indent++;
		BusinessBaseBuilder.BuildWriter(indentWriter, model);
		indentWriter.WriteLine();
		BusinessBaseBuilder.BuildReader(indentWriter, model);
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

			if (i < model.Items.Length - 1)
			{
				indentWriter.WriteLine();
			}
		}

		DeserializationBuilder.Build(indentWriter, model);

		indentWriter.Indent--;
		indentWriter.WriteLine("}");
	}

	internal static string GetLoadProperty(SerializationItemModel item, string readerInvocation) =>
		$"this.LoadProperty({item.PropertyInfoContainingType.FullyQualifiedName}.{item.PropertyInfoFieldName}, {readerInvocation});";

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