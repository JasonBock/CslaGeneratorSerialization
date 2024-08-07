﻿using GeneratorSerialization.Tests;
using NUnit.Framework;

namespace CslaGeneratorSerialization.Tests.Generators.Graphs;

public static class DuplicateTests
{
	[Test]
	public static async Task GenerateAsync()
	{
		var code =
			"""
			using Csla;
			using CslaGeneratorSerialization;
			using System;

			#nullable enable
			
			namespace Domains;

			[GeneratorSerializable]
			public sealed partial class Node
				: BusinessBase<Node>
			{
				[Create]
				private void Create() { }

				public static readonly PropertyInfo<Node?> LeftProperty =
					RegisterProperty<Node?>(_ => _.Left);
				public Node? Left
				{
					get => this.GetProperty(Node.LeftProperty);
					set => this.SetProperty(Node.LeftProperty, value);
				}

				public static readonly PropertyInfo<Node?> RightProperty =
					RegisterProperty<Node?>(_ => _.Right);
				public Node? Right
				{
					get => this.GetProperty(Node.RightProperty);
					set => this.SetProperty(Node.RightProperty, value);
				}

				public static readonly PropertyInfo<string> NameProperty =
					RegisterProperty<string>(_ => _.Name);
				public string Name
				{
					get => this.GetProperty(Node.NameProperty);
					set => this.SetProperty(Node.NameProperty, value);
				}
			}
			""";

		var generatedCode =
			"""
			// <auto-generated/>
			
			using CslaGeneratorSerialization.Extensions;
			
			#nullable enable
			
			namespace Domains;
			
			public sealed partial class Node
				: global::CslaGeneratorSerialization.IGeneratorSerializable
			{
				void global::CslaGeneratorSerialization.IGeneratorSerializable.SetState(global::CslaGeneratorSerialization.GeneratorFormatterWriterContext context)
				{
					// global::Domains.Node.NameProperty
					context.Writer.Write(this.ReadProperty<string>(global::Domains.Node.NameProperty));
					
					// global::Domains.Node.LeftProperty
					context.Write(this.ReadProperty<global::Domains.Node?>(global::Domains.Node.LeftProperty), true);
					
					// global::Domains.Node.RightProperty
					context.Write(this.ReadProperty<global::Domains.Node?>(global::Domains.Node.RightProperty), true);
					
					context.Writer.Write(this.IsNew);
					context.Writer.Write(this.IsDeleted);
					context.Writer.Write(this.IsDirty);
					context.Writer.Write(this.IsChild);
					context.Writer.Write(this.DisableIEditableObject);
					
					var type = this.GetType();
					context.Writer.Write((bool)type.GetFieldInHierarchy("_neverCommitted")!.GetValue(this)!);
					context.Writer.Write((int)type.GetFieldInHierarchy("_editLevelAdded")!.GetValue(this)!);
					context.Writer.Write((int)type.GetFieldInHierarchy("_identity")!.GetValue(this)!);
				}
				
				void global::CslaGeneratorSerialization.IGeneratorSerializable.GetState(global::CslaGeneratorSerialization.GeneratorFormatterReaderContext context)
				{
					// global::Domains.Node.NameProperty
					this.LoadProperty(global::Domains.Node.NameProperty, context.Reader.ReadString());
					
					// global::Domains.Node.LeftProperty
					this.LoadProperty(global::Domains.Node.LeftProperty, context.Read<global::Domains.Node>(true)!);
					
					// global::Domains.Node.RightProperty
					this.LoadProperty(global::Domains.Node.RightProperty, context.Read<global::Domains.Node>(true)!);
					
					var type = this.GetType();
					type.GetFieldInHierarchy("_isNew")!.SetValue(this, context.Reader.ReadBoolean());
					type.GetFieldInHierarchy("_isDeleted")!.SetValue(this, context.Reader.ReadBoolean());
					type.GetFieldInHierarchy("_isDirty")!.SetValue(this, context.Reader.ReadBoolean());
					type.GetFieldInHierarchy("_isChild")!.SetValue(this, context.Reader.ReadBoolean());
					this.DisableIEditableObject = context.Reader.ReadBoolean();
					
					type.GetFieldInHierarchy("_neverCommitted")!.SetValue(this, context.Reader.ReadBoolean());
					type.GetFieldInHierarchy("_editLevelAdded")!.SetValue(this, context.Reader.ReadInt32());
					type.GetFieldInHierarchy("_identity")!.SetValue(this, context.Reader.ReadInt32());
				}
			}
			
			""";

		await TestAssistants.RunGeneratorAsync<GeneratorSerializationGenerator>(code,
			[
				(typeof(GeneratorSerializationGenerator), "Domains.Node_GeneratorSerialization.g.cs", generatedCode),
			],
			[]);
	}
}