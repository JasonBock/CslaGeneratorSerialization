﻿using NUnit.Framework;

namespace CslaGeneratorSerialization.Tests.Generators.Graphs;

public static class CustomizationGeneratorTests
{
	[Test]
	public static async Task GenerateClassAsync()
	{
		var code =
			"""
			using Csla;
			using CslaGeneratorSerialization;
			using System;
			using System.IO;
			
			namespace Domains;

			[GeneratorSerializable]
			public sealed partial class Data
				: BusinessBase<Data>, IGeneratorSerializableCustomization
			{
				public void GetCustomState(BinaryReader reader)
				{
					ArgumentNullException.ThrowIfNull(reader);
					this.Custom = reader.ReadInt32();
				}

				public void SetCustomState(BinaryWriter writer)
				{
					ArgumentNullException.ThrowIfNull(writer);
					writer.Write(this.Custom);
				}

				public static readonly PropertyInfo<string> ContentsProperty =
					RegisterProperty<string>(_ => _.Contents);
				public string Contents
				{
					get => this.GetProperty(Data.ContentsProperty);
					set => this.SetProperty(Data.ContentsProperty, value);
				}

				public int Custom { get; set; }
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
					// global::Domains.Data.ContentsProperty
					context.Writer.Write(this.ReadProperty<string>(global::Domains.Data.ContentsProperty));
					
					this.SetCustomState(context.Writer);
					
					context.Writer.Write(this.IsNew);
					context.Writer.Write(this.IsDeleted);
					context.Writer.Write(this.IsDirty);
					context.Writer.Write(this.IsChild);
					context.Writer.Write(this.DisableIEditableObject);
								
					context.Writer.Write(global::CslaGeneratorSerialization.BusinessBaseAccessors.GetSetNeverCommittedField(this));
					context.Writer.Write(global::CslaGeneratorSerialization.BusinessBaseAccessors.GetSetEditLevelAddedField(this));
					context.Writer.Write(global::CslaGeneratorSerialization.BusinessBaseAccessors.GetSetIdentityField(this));
				}
				
				void global::CslaGeneratorSerialization.IGeneratorSerializable.GetState(global::CslaGeneratorSerialization.GeneratorFormatterReaderContext context)
				{
					// global::Domains.Data.ContentsProperty
					this.LoadProperty(global::Domains.Data.ContentsProperty, context.Reader.ReadString());
					
					this.GetCustomState(context.Reader);
					
					global::CslaGeneratorSerialization.BusinessBaseAccessors.SetIsNewProperty(this, context.Reader.ReadBoolean());
					global::CslaGeneratorSerialization.BusinessBaseAccessors.SetIsDeletedProperty(this, context.Reader.ReadBoolean());
					global::CslaGeneratorSerialization.BusinessBaseAccessors.GetSetIsDirtyField(this) = context.Reader.ReadBoolean();
					global::CslaGeneratorSerialization.BusinessBaseAccessors.GetSetIsChildField(this) = context.Reader.ReadBoolean();
					this.DisableIEditableObject = context.Reader.ReadBoolean();
					
					global::CslaGeneratorSerialization.BusinessBaseAccessors.GetSetNeverCommittedField(this) = context.Reader.ReadBoolean();
					global::CslaGeneratorSerialization.BusinessBaseAccessors.GetSetEditLevelAddedField(this) = context.Reader.ReadInt32();
					global::CslaGeneratorSerialization.BusinessBaseAccessors.GetSetIdentityField(this) = context.Reader.ReadInt32();
				}
			}
			
			""";

		await TestAssistants.RunGeneratorAsync<GeneratorSerializationGenerator>(code,
			[("Domains.Data_GeneratorSerialization.g.cs", generatedCode)], 
			[]);
	}

	[Test]
	public static async Task GenerateStructAsync()
	{
		var code =
			"""
			using Csla;
			using CslaGeneratorSerialization;
			using System;

			namespace Domains;

			public struct CustomData
			{
				public int Id { get; set; }
				public string Name { get; set; }
			}

			[GeneratorSerializable]
			public sealed partial class Data
				: BusinessBase<Data>
			{
				public static readonly PropertyInfo<CustomData> ContentsProperty =
					RegisterProperty<CustomData>(_ => _.Contents);
				public CustomData Contents
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
					// global::Domains.Data.ContentsProperty
					context.WriteCustom<global::Domains.CustomData>(this.ReadProperty<global::Domains.CustomData>(global::Domains.Data.ContentsProperty));
					
					context.Writer.Write(this.IsNew);
					context.Writer.Write(this.IsDeleted);
					context.Writer.Write(this.IsDirty);
					context.Writer.Write(this.IsChild);
					context.Writer.Write(this.DisableIEditableObject);
								
					context.Writer.Write(global::CslaGeneratorSerialization.BusinessBaseAccessors.GetSetNeverCommittedField(this));
					context.Writer.Write(global::CslaGeneratorSerialization.BusinessBaseAccessors.GetSetEditLevelAddedField(this));
					context.Writer.Write(global::CslaGeneratorSerialization.BusinessBaseAccessors.GetSetIdentityField(this));
				}
				
				void global::CslaGeneratorSerialization.IGeneratorSerializable.GetState(global::CslaGeneratorSerialization.GeneratorFormatterReaderContext context)
				{
					// global::Domains.Data.ContentsProperty
					this.LoadProperty(global::Domains.Data.ContentsProperty, context.ReadCustom<global::Domains.CustomData>());
					
					global::CslaGeneratorSerialization.BusinessBaseAccessors.SetIsNewProperty(this, context.Reader.ReadBoolean());
					global::CslaGeneratorSerialization.BusinessBaseAccessors.SetIsDeletedProperty(this, context.Reader.ReadBoolean());
					global::CslaGeneratorSerialization.BusinessBaseAccessors.GetSetIsDirtyField(this) = context.Reader.ReadBoolean();
					global::CslaGeneratorSerialization.BusinessBaseAccessors.GetSetIsChildField(this) = context.Reader.ReadBoolean();
					this.DisableIEditableObject = context.Reader.ReadBoolean();
					
					global::CslaGeneratorSerialization.BusinessBaseAccessors.GetSetNeverCommittedField(this) = context.Reader.ReadBoolean();
					global::CslaGeneratorSerialization.BusinessBaseAccessors.GetSetEditLevelAddedField(this) = context.Reader.ReadInt32();
					global::CslaGeneratorSerialization.BusinessBaseAccessors.GetSetIdentityField(this) = context.Reader.ReadInt32();
				}
			}
			
			""";

		await TestAssistants.RunGeneratorAsync<GeneratorSerializationGenerator>(code,
			[("Domains.Data_GeneratorSerialization.g.cs", generatedCode)], 
			[]);
	}
}