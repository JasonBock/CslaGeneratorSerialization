﻿using GeneratorSerialization.Tests;
using NUnit.Framework;

namespace CslaGeneratorSerialization.Tests.Generators.Graphs;

public static class ListTests
{
	[Test]
	public static async Task GenerateAsync()
	{
		var code =
			"""
			using Csla;
			using System;

			#nullable enable
			
			namespace Domains;

			[Serializable]
			public partial class Experiments
				: BusinessBase<Experiments>
			{
				[Create]
				private void Create() =>
					this.Values = this.ApplicationContext.GetRequiredService<IChildDataPortal<Datum>>().CreateChild();

				public static readonly PropertyInfo<Datum> ValuesProperty =
					Experiments.RegisterProperty<Datum>(_ => _.Values);
				public Datum Values
				{
					get => this.GetProperty(Experiments.ValuesProperty);
					private set => this.SetProperty(Experiments.ValuesProperty, value);
				}
			}

			[Serializable]
			public partial class Datum
				: BusinessListBase<Datum, Data>
			{
				[CreateChild]
				private void CreateChild() { }
			}

			[Serializable]
			public partial class Data
				: BusinessBase<Data>
			{
				[CreateChild]
				private void CreateChild() { }

				public static readonly PropertyInfo<string> ValueProperty =
					Data.RegisterProperty<string>(_ => _.Value);
				public string Value
				{
					get => this.GetProperty(Data.ValueProperty);
					set => this.SetProperty(Data.ValueProperty, value);
				}
			}
			""";

		var experimentsGeneratedCode =
			"""
			// <auto-generated/>
			
			using CslaGeneratorSerialization.Extensions;
			
			#nullable enable
			
			namespace Domains;
			
			public partial class Experiments
				: global::CslaGeneratorSerialization.IGeneratorSerializable
			{
				void global::CslaGeneratorSerialization.IGeneratorSerializable.SetState(global::CslaGeneratorSerialization.GeneratorFormatterWriterContext context)
				{
					// global::Domains.Experiments.ValuesProperty
					var value0 = this.ReadProperty<global::Domains.Datum>(global::Domains.Experiments.ValuesProperty);
					
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
					// global::Domains.Experiments.ValuesProperty
					switch (context.Reader.ReadStateValue())
					{
						case global::CslaGeneratorSerialization.SerializationState.Duplicate:
							this.LoadProperty(global::Domains.Experiments.ValuesProperty, context.GetReference(context.Reader.ReadInt32()));
							break;
						case global::CslaGeneratorSerialization.SerializationState.Value:
							global::Domains.Datum newValue;
									
							if (context.Reader.ReadStateValue() == global::CslaGeneratorSerialization.SerializationState.Duplicate)
							{
								newValue = context.CreateInstance<global::Domains.Datum>(context.GetTypeName(context.Reader.ReadInt32()));
							}
							else
							{
								var newValueTypeName = context.Reader.ReadString();
								context.AddTypeName(newValueTypeName);
								newValue = context.CreateInstance<global::Domains.Datum>(newValueTypeName);
							}
							((global::CslaGeneratorSerialization.IGeneratorSerializable)newValue).GetState(context);
							this.LoadProperty(global::Domains.Experiments.ValuesProperty, newValue);
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

		var datumGeneratedCode =
			"""
			// <auto-generated/>
			
			using CslaGeneratorSerialization.Extensions;
			
			#nullable enable
			
			namespace Domains;
			
			public partial class Datum
				: global::CslaGeneratorSerialization.IGeneratorSerializable
			{
				void global::CslaGeneratorSerialization.IGeneratorSerializable.SetState(global::CslaGeneratorSerialization.GeneratorFormatterWriterContext context)
				{
					context.Writer.Write(this.Count);
					
					foreach (var item in this)
					{
						if (item is not null)
						{
							(var isReferenceDuplicate, var referenceId) = context.GetReference(item);
						
							if (isReferenceDuplicate)
							{
								context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Duplicate);
								context.Writer.Write(referenceId);
							}
							else
							{
								context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Value);
					
								var itemTypeName = item.GetType().AssemblyQualifiedName!;
								(var isTypeNameDuplicate, var typeNameId) = context.GetTypeName(itemTypeName);
					
								if (isTypeNameDuplicate)
								{
									context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Duplicate);
									context.Writer.Write(typeNameId);
								}
								else
								{
									context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Value);
									context.Writer.Write(itemTypeName);
								}
					
								((global::CslaGeneratorSerialization.IGeneratorSerializable)item).SetState(context);			
							}			
						}
						else
						{
							context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Null);
						}
					}
					
					context.Writer.Write(this.DeletedList.Count);
					
					foreach (var deletedItem in this.DeletedList)
					{
						(_, var deletedReferenceId) = context.GetReference(deletedItem);
						context.Writer.Write(deletedReferenceId);
					}
					
					var type = this.GetType();
					context.Writer.Write((bool)type.GetFieldInHierarchy("_isChild")!.GetValue(this)!);
					context.Writer.Write((int)type.GetFieldInHierarchy("_editLevel")!.GetValue(this)!);
					context.Writer.Write((int)type.GetFieldInHierarchy("_identity")!.GetValue(this)!);
					
					context.Writer.Write(this.AllowEdit);
					context.Writer.Write(this.AllowNew);
					context.Writer.Write(this.AllowRemove);
					context.Writer.Write(this.RaiseListChangedEvents);
					context.Writer.Write((bool)type.GetFieldInHierarchy("_supportsChangeNotificationCore")!.GetValue(this)!);
				}
				
				void global::CslaGeneratorSerialization.IGeneratorSerializable.GetState(global::CslaGeneratorSerialization.GeneratorFormatterReaderContext context)
				{
					var count = context.Reader.ReadInt32();
					
					for (var i = 0; i < count; i++)
					{
						switch (context.Reader.ReadStateValue())
						{
							case global::CslaGeneratorSerialization.SerializationState.Duplicate:
								this.Add((global::Domains.Data)context.GetReference(context.Reader.ReadInt32()));
								break;
							case global::CslaGeneratorSerialization.SerializationState.Value:
								global::Domains.Data newValue;
									
								if (context.Reader.ReadStateValue() == global::CslaGeneratorSerialization.SerializationState.Duplicate)
								{
									newValue = context.CreateInstance<global::Domains.Data>(context.GetTypeName(context.Reader.ReadInt32()));
								}
								else
								{
									var newValueTypeName = context.Reader.ReadString();
									context.AddTypeName(newValueTypeName);
									newValue = context.CreateInstance<global::Domains.Data>(newValueTypeName);
								}
								((global::CslaGeneratorSerialization.IGeneratorSerializable)newValue).GetState(context);
								this.Add(newValue);
								break;
							case global::CslaGeneratorSerialization.SerializationState.Null:
								break;
						}
					}
					
					var deletedCount = context.Reader.ReadInt32();
					
					for (var d = 0; d < deletedCount; d++)
					{
						this.DeletedList.Add((global::Domains.Data)context.GetReference(context.Reader.ReadInt32()));
					}
					
					var type = this.GetType();
					type.GetFieldInHierarchy("_isChild")!.SetValue(this, context.Reader.ReadBoolean());
					type.GetFieldInHierarchy("_editLevel")!.SetValue(this, context.Reader.ReadInt32());
					type.GetFieldInHierarchy("_identity")!.SetValue(this, context.Reader.ReadInt32());
					
					this.AllowEdit = context.Reader.ReadBoolean();
					this.AllowNew = context.Reader.ReadBoolean();
					this.AllowRemove = context.Reader.ReadBoolean();
					this.RaiseListChangedEvents = context.Reader.ReadBoolean();
					type.GetFieldInHierarchy("_supportsChangeNotificationCore")!.SetValue(this, context.Reader.ReadBoolean());
				}
			}
			
			""";

		var dataGeneratedCode =
			"""
			// <auto-generated/>
			
			using CslaGeneratorSerialization.Extensions;
			
			#nullable enable
			
			namespace Domains;
			
			public partial class Data
				: global::CslaGeneratorSerialization.IGeneratorSerializable
			{
				void global::CslaGeneratorSerialization.IGeneratorSerializable.SetState(global::CslaGeneratorSerialization.GeneratorFormatterWriterContext context)
				{
					// global::Domains.Data.ValueProperty
					context.Writer.Write(this.ReadProperty<string>(global::Domains.Data.ValueProperty));
					
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
					// global::Domains.Data.ValueProperty
					this.LoadProperty(global::Domains.Data.ValueProperty, context.Reader.ReadString());
					
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
				(typeof(GeneratorSerializationGenerator), "Domains.Experiments_GeneratorSerialization.g.cs", experimentsGeneratedCode),
				(typeof(GeneratorSerializationGenerator), "Domains.Datum_GeneratorSerialization.g.cs", datumGeneratedCode),
				(typeof(GeneratorSerializationGenerator), "Domains.Data_GeneratorSerialization.g.cs", dataGeneratedCode),
			],
			[]);
	}
}