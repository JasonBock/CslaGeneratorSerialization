﻿using GeneratorSerialization.Tests;
using NUnit.Framework;

namespace CslaGeneratorSerialization.Tests.Generators.Collections;

public static class ByteArrayTests
{
	[Test]
	public static async Task GenerateAsync()
	{
		var code =
			"""
			using Csla;
			using System;

			namespace Domains;

			[Serializable]
			public sealed partial class Data
				: BusinessBase<Data>
			{
				public static readonly PropertyInfo<byte[]> ContentsProperty =
					RegisterProperty<byte[]>(_ => _.Contents);
				public byte[] Contents
				{
					get => this.GetProperty(Data.ContentsProperty);
					set => this.SetProperty(Data.ContentsProperty, value);
				}
			}
			""";

		var generatedCode =
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
					// Set custom object state
					var value0 = this.ReadProperty(global::Domains.Data.ContentsProperty);
					
					if (value0 is not null)
					{
						context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Value);
						context.Writer.Write((value0.Length, value0));
					}
					else
					{
						context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Null);
					}
					
					// Set base object state
					context.Writer.Write(this.IsNew);
					context.Writer.Write(this.IsDeleted);
					context.Writer.Write(this.IsDirty);
					context.Writer.Write(this.IsChild);
					context.Writer.Write(this.DisableIEditableObject);
					
					//The only way I can get these is through Reflection.
					//Ugly, but...means must.
					var type = this.GetType();
					context.Writer.Write((bool)type.GetFieldInHierarchy("_neverCommitted")!.GetValue(this)!);
					context.Writer.Write((int)type.GetFieldInHierarchy("_editLevelAdded")!.GetValue(this)!);
					context.Writer.Write((int)type.GetFieldInHierarchy("_identity")!.GetValue(this)!);
				}
				
				void global::CslaGeneratorSerialization.IGeneratorSerializable.GetState(global::CslaGeneratorSerialization.GeneratorFormatterReaderContext context)
				{
					// Get custom object state
					if (context.Reader.ReadStateValue() == global::CslaGeneratorSerialization.SerializationState.Value)
					{
						this.LoadProperty(global::Domains.Data.ContentsProperty, context.Reader.ReadByteArray());
					}
					
					//The only way I can get these (except for DisableIEditableObject) is through Reflection.
					//Ugly, but...means must.
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
			[(typeof(GeneratorSerializationGenerator), "Domains.Data_GeneratorSerialization.g.cs", generatedCode)],
			[]);
	}
}