﻿using NUnit.Framework;

namespace CslaGeneratorSerialization.Tests.Generators.Graphs;

public static class ChildBusinessObjectGeneratorTests
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
			public sealed partial class Data
				: BusinessBase<Data>
			{
				public static readonly PropertyInfo<ChildData> ContentsProperty =
					RegisterProperty<ChildData>(_ => _.Contents);
				public ChildData Contents
				{
					get => this.GetProperty(Data.ContentsProperty);
					set => this.SetProperty(Data.ContentsProperty, value);
				}
			}

			[GeneratorSerializable]
			public sealed partial class ChildData
				: BusinessBase<ChildData>
			{
				public static readonly PropertyInfo<string> ChildContentsProperty =
					RegisterProperty<string>(_ => _.ChildContents);
				public string ChildContents
				{
					get => this.GetProperty(ChildData.ChildContentsProperty);
					set => this.SetProperty(ChildData.ChildContentsProperty, value);
				}
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
					context.Write(this.ReadProperty<global::Domains.ChildData>(global::Domains.Data.ContentsProperty), true);
					
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
					this.LoadProperty(global::Domains.Data.ContentsProperty, context.Read<global::Domains.ChildData>(true)!);
					
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

		var childGeneratedCode =
			"""
			// <auto-generated/>
			
			using CslaGeneratorSerialization.Extensions;
			
			#nullable enable
			
			namespace Domains;
			
			public sealed partial class ChildData
				: global::CslaGeneratorSerialization.IGeneratorSerializable
			{
				void global::CslaGeneratorSerialization.IGeneratorSerializable.SetState(global::CslaGeneratorSerialization.GeneratorFormatterWriterContext context)
				{
					// global::Domains.ChildData.ChildContentsProperty
					context.Writer.Write(this.ReadProperty<string>(global::Domains.ChildData.ChildContentsProperty));
					
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
					// global::Domains.ChildData.ChildContentsProperty
					this.LoadProperty(global::Domains.ChildData.ChildContentsProperty, context.Reader.ReadString());
					
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
			[
				("Domains.Data_GeneratorSerialization.g.cs", dataGeneratedCode),
				("Domains.ChildData_GeneratorSerialization.g.cs", childGeneratedCode)
			],
			[]);
	}

	[Test]
	public static async Task GenerateWhenChildObjectDoesNotParticipateInGeneratorSerializationAsync()
	{
		var code =
			"""
			using Csla;
			using CslaGeneratorSerialization;
			using System;

			namespace Domains;

			[GeneratorSerializable]
			public sealed partial class Data
				: BusinessBase<Data>
			{
				public static readonly PropertyInfo<ChildData> ContentsProperty =
					RegisterProperty<ChildData>(_ => _.Contents);
				public ChildData Contents
				{
					get => this.GetProperty(Data.ContentsProperty);
					set => this.SetProperty(Data.ContentsProperty, value);
				}
			}

			public sealed class ChildData
				: BusinessBase<ChildData>
			{
				public static readonly PropertyInfo<string> ChildContentsProperty =
					RegisterProperty<string>(_ => _.ChildContents);
				public string ChildContents
				{
					get => this.GetProperty(ChildData.ChildContentsProperty);
					set => this.SetProperty(ChildData.ChildContentsProperty, value);
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
					context.WriteMobileObject(this.ReadProperty<global::Domains.ChildData>(global::Domains.Data.ContentsProperty));
					
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
					this.LoadProperty(global::Domains.Data.ContentsProperty, context.ReadMobileObject<global::Domains.ChildData>()!);
					
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