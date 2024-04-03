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
			using System;

			namespace Domains;

			public partial interface IData
				: IBusinessBase
			{
				string Contents { get; set; }
			}

			[Serializable]
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

			[Serializable]
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
					var value0 = this.ReadProperty<global::Domains.IData>(global::Domains.ConsumeData.ContentsProperty);
					
					if (value0 is not null)
					{
						(var isReferenceDuplicate, var referenceId) = context.GetReference(value0);
					
						if (isReferenceDuplicate)
						{
							context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Duplicate);
							context.Writer.Write(referenceId);
						}
						else
						{
							context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Value);				
					
							var value0TypeName = value0.GetType().AssemblyQualifiedName!;
							(var isTypeNameDuplicate, var typeNameId) = context.GetTypeName(value0TypeName);
					
							if (isTypeNameDuplicate)
							{
								context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Duplicate);
								context.Writer.Write(typeNameId);
							}
							else
							{
								context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Value);
								context.Writer.Write(value0TypeName);
							}
					
							((global::CslaGeneratorSerialization.IGeneratorSerializable)value0).SetState(context);
						}
					}
					else
					{
						context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Null);
					}
					
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
					switch (context.Reader.ReadStateValue())
					{
						case global::CslaGeneratorSerialization.SerializationState.Duplicate:
							this.LoadProperty(global::Domains.ConsumeData.ContentsProperty, context.GetReference(context.Reader.ReadInt32()));
							break;
						case global::CslaGeneratorSerialization.SerializationState.Value:
							global::Domains.IData newValue;
									
							if (context.Reader.ReadStateValue() == global::CslaGeneratorSerialization.SerializationState.Duplicate)
							{
								newValue = context.CreateInstance<global::Domains.IData>(context.GetTypeName(context.Reader.ReadInt32()));
							}
							else
							{
								var newValueTypeName = context.Reader.ReadString();
								context.AddTypeName(newValueTypeName);
								newValue = context.CreateInstance<global::Domains.IData>(newValueTypeName);
							}
							((global::CslaGeneratorSerialization.IGeneratorSerializable)newValue).GetState(context);
							this.LoadProperty(global::Domains.ConsumeData.ContentsProperty, newValue);
							context.AddReference(newValue);
							break;
						case global::CslaGeneratorSerialization.SerializationState.Null:
							break;
					}
					
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