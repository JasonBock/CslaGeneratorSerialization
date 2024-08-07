﻿using GeneratorSerialization.Tests;
using NUnit.Framework;

namespace CslaGeneratorSerialization.Tests.Generators.Graphs;

public static class InterfaceTests
{
	[Test]
	public static async Task GenerateAsync()
	{
		var code =
			"""
			using Csla;
			using CslaGeneratorSerialization;
			using System;

			namespace Domains;

			[GeneratorSerializable]
			public partial interface IData
				: IBusinessBase
			{
				string Contents { get; set; }
			}

			[GeneratorSerializable]
			public sealed partial class Data
				: BusinessBase<Data>, IData
			{
				public static readonly PropertyInfo<string> ContentsProperty =
					RegisterProperty<string>(_ => _.Contents);
				public string Contents
				{
					get => this.GetProperty(Data.ContentsProperty);
					set => this.SetProperty(Data.ContentsProperty, value);
				}
			}

			[GeneratorSerializable]
			public sealed partial class ConsumeData
				: BusinessBase<ConsumeData>
			{
				public static readonly PropertyInfo<IData> ContentsProperty =
					RegisterProperty<IData>(_ => _.Contents);
				public IData Contents
				{
					get => this.GetProperty(ConsumeData.ContentsProperty);
					set => this.SetProperty(ConsumeData.ContentsProperty, value);
				}
			}
			""";

		var iDataGeneratedCode =
			"""
			// <auto-generated/>
			
			using CslaGeneratorSerialization.Extensions;
			
			#nullable enable
			
			namespace Domains;
			
			public partial interface IData
				: global::CslaGeneratorSerialization.IGeneratorSerializable
			{
			}
			
			""";

		var dataGeneratedCode =
			"""
			// <auto-generated/>
			
			using CslaGeneratorSerialization.Extensions;
			
			#nullable enable
			
			namespace Domains;
			
			public sealed partial class Data
				: global::CslaGeneratorSerialization.IGeneratorSerializable
			{
				void global::CslaGeneratorSerialization.IGeneratorSerializable.SetState(global::CslaGeneratorSerialization.GeneratorFormatterWriterContext context)
				{
					// global::Domains.Data.ContentsProperty
					context.Writer.Write(this.ReadProperty<string>(global::Domains.Data.ContentsProperty));
					
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
					// global::Domains.Data.ContentsProperty
					this.LoadProperty(global::Domains.Data.ContentsProperty, context.Reader.ReadString());
					
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

		var consumeGeneratedCode =
			"""
			// <auto-generated/>
			
			using CslaGeneratorSerialization.Extensions;
			
			#nullable enable
			
			namespace Domains;
			
			public sealed partial class ConsumeData
				: global::CslaGeneratorSerialization.IGeneratorSerializable
			{
				void global::CslaGeneratorSerialization.IGeneratorSerializable.SetState(global::CslaGeneratorSerialization.GeneratorFormatterWriterContext context)
				{
					// global::Domains.ConsumeData.ContentsProperty
					context.Write(this.ReadProperty<global::Domains.IData>(global::Domains.ConsumeData.ContentsProperty), false);
					
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
					// global::Domains.ConsumeData.ContentsProperty
					this.LoadProperty(global::Domains.ConsumeData.ContentsProperty, context.Read<global::Domains.IData>(false)!);
					
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
				(typeof(GeneratorSerializationGenerator), "Domains.IData_GeneratorSerialization.g.cs", iDataGeneratedCode),
				(typeof(GeneratorSerializationGenerator), "Domains.Data_GeneratorSerialization.g.cs", dataGeneratedCode),
				(typeof(GeneratorSerializationGenerator), "Domains.ConsumeData_GeneratorSerialization.g.cs", consumeGeneratedCode)
			],
			[]);
	}
}