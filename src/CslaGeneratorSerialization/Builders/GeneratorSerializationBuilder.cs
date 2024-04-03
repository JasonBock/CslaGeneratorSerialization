﻿using CslaGeneratorSerialization.Extensions;
using CslaGeneratorSerialization.Models;
using Microsoft.CodeAnalysis;
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

		var derivation = model.BusinessObject.IsSealed ? "sealed " :
			model.BusinessObject.TypeKind != TypeKind.Interface && model.BusinessObject.IsAbstract ? 
				"abstract " : 
				string.Empty;
		var typeKind = model.BusinessObject.TypeKind == TypeKind.Interface ?
			"interface" : "class";

		indentWriter.WriteLines(
			$$"""
			public {{derivation}}partial {{typeKind}} {{this.Model.BusinessObject.Name}}
				: global::CslaGeneratorSerialization.IGeneratorSerializable
			{
			""");

		if (model.BusinessObject.TypeKind == TypeKind.Class)
		{
			switch (model.BusinessObject.BusinessObjectKind)
			{
				case StereotypeKind.BusinessBase:
					BusinessBaseBuilder.Build(indentWriter, model);
					break;
				case StereotypeKind.BusinessListBase:
					BusinessListBaseBuilder.Build(indentWriter, model);
					break;
				case StereotypeKind.CommandBase:
					CommandBaseBuilder.Build(indentWriter, model);
					break;
				case StereotypeKind.ReadOnlyBase:
					ReadOnlyBaseBuilder.Build(indentWriter, model);
					break;
				case StereotypeKind.ReadOnlyListBase:
					ReadOnlyListBaseBuilder.Build(indentWriter, model);
					break;
			}
		}

		indentWriter.WriteLine("}");

		this.Text = SourceText.From(writer.ToString(), Encoding.UTF8);
		this.FileName = $"{this.Model.BusinessObject.FullyQualifiedName.GenerateFileName()}_GeneratorSerialization.g.cs";
	}

	internal string FileName { get; }
	internal SerializationModel Model { get; }
	internal SourceText Text { get; private set; }
}