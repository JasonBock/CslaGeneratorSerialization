﻿using CslaGeneratorSerialization.Extensions;
using CslaGeneratorSerialization.Models;
using Microsoft.CodeAnalysis.Text;
using System.CodeDom.Compiler;
using System.Text;

namespace CslaGeneratorSerialization.Builders;

internal sealed class GeneratorSerializationBuilder
{
	internal GeneratorSerializationBuilder(SerializationModel model)
	{
		this.Model = model;

		using var writer = new StringWriter();
		using var indentWriter = new IndentedTextWriter(writer, "\t");

		indentWriter.WriteLines(
			"""
			// <auto-generated/>
			
			using CslaGeneratorSerialization.Extensions;

			#nullable enable

			""");

		var boNamespace = this.Model.BusinessObject.Namespace;

		if (boNamespace is not null)
		{
			indentWriter.WriteLines(
				$"""
				namespace {boNamespace};
				
				""");
		}

		indentWriter.WriteLines(
			$$"""
			public sealed partial class {{this.Model.BusinessObject.Name}}
				: global::CslaGeneratorSerialization.IGeneratorSerializable
			{
			""");

		indentWriter.Indent++;
		GeneratorSerializationBuilderWriter.Build(indentWriter, model);
		indentWriter.WriteLine();
		GeneratorSerializationBuilderReader.Build(indentWriter, model);
		indentWriter.Indent--;

		indentWriter.WriteLine("}");

		this.Text = SourceText.From(writer.ToString(), Encoding.UTF8);
		this.FileName = $"{this.Model.BusinessObject.FullyQualifiedName.GenerateFileName()}_GeneratorSerialization.g.cs";
	}

	internal string FileName { get; }
	internal SerializationModel Model { get; }
	internal SourceText Text { get; private set; }
}