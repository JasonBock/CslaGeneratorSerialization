﻿using NUnit.Framework;

namespace CslaGeneratorSerialization.Tests.Generators.Graphs;

public static class InheritanceTests
{
	[Test]
	public static async Task GenerateFromBaseTypeAsync()
	{
		var code =
			"""
			using Csla;
			using CslaGeneratorSerialization;
			using System;

			namespace Domains;

			[GeneratorSerializable]
			public partial class BaseData
				: BusinessBase<BaseData>
			{
				public static readonly PropertyInfo<string> CoreProperty =
					BaseData.RegisterProperty<string>(_ => _.Core);
				public string Core
				{
					get => this.GetProperty(BaseData.CoreProperty);
					set => this.SetProperty(BaseData.CoreProperty, value);
				}
			}

			[GeneratorSerializable]
			public sealed partial class DerivedData
				: BaseData
			{
				public static readonly PropertyInfo<int> CustomProperty =
					DerivedData.RegisterProperty<int>(typeof(DerivedData), new PropertyInfo<int>(nameof(DerivedData.Custom)));
				public int Custom
				{
					get => this.GetProperty(DerivedData.CustomProperty);
					set => this.SetProperty(DerivedData.CustomProperty, value);
				}
			}
			""";

		var baseGeneratedCode =
			"""
			// <auto-generated/>
			
			using CslaGeneratorSerialization.Extensions;
			
			#nullable enable
			
			namespace Domains;
			
			public partial class BaseData
				: global::CslaGeneratorSerialization.IGeneratorSerializable
			{
				void global::CslaGeneratorSerialization.IGeneratorSerializable.SetState(global::CslaGeneratorSerialization.GeneratorFormatterWriterContext context)
				{
					// global::Domains.BaseData.CoreProperty
					context.Writer.Write(this.ReadProperty<string>(global::Domains.BaseData.CoreProperty));
					
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
					// global::Domains.BaseData.CoreProperty
					this.LoadProperty(global::Domains.BaseData.CoreProperty, context.Reader.ReadString());
					
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

		var derivedGeneratedCode =
			"""
			// <auto-generated/>
			
			using CslaGeneratorSerialization.Extensions;
			
			#nullable enable
			
			namespace Domains;
			
			public sealed partial class DerivedData
				: global::CslaGeneratorSerialization.IGeneratorSerializable
			{
				void global::CslaGeneratorSerialization.IGeneratorSerializable.SetState(global::CslaGeneratorSerialization.GeneratorFormatterWriterContext context)
				{
					// global::Domains.BaseData.CoreProperty
					context.Writer.Write(this.ReadProperty<string>(global::Domains.BaseData.CoreProperty));
					
					// global::Domains.DerivedData.CustomProperty
					context.Writer.Write(this.ReadProperty<int>(global::Domains.DerivedData.CustomProperty));
					
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
					// global::Domains.BaseData.CoreProperty
					this.LoadProperty(global::Domains.BaseData.CoreProperty, context.Reader.ReadString());
					
					// global::Domains.DerivedData.CustomProperty
					this.LoadProperty(global::Domains.DerivedData.CustomProperty, context.Reader.ReadInt32());
					
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
				("Domains.BaseData_GeneratorSerialization.g.cs", baseGeneratedCode),
				("Domains.DerivedData_GeneratorSerialization.g.cs", derivedGeneratedCode)
			],
			[]);
	}

	[Test]
	public static async Task GenerateFromAbstractTypeAsync()
	{
		var code =
			"""
			using Csla;
			using CslaGeneratorSerialization;
			using System;

			namespace Domains;

			[GeneratorSerializable]
			public abstract partial class BaseData
				: BusinessBase<BaseData>
			{
				public static readonly PropertyInfo<string> CoreProperty =
					BaseData.RegisterProperty<string>(_ => _.Core);
				public string Core
				{
					get => this.GetProperty(BaseData.CoreProperty);
					set => this.SetProperty(BaseData.CoreProperty, value);
				}
			}

			[GeneratorSerializable]
			public sealed partial class DerivedData
				: BaseData
			{
				public static readonly PropertyInfo<int> CustomProperty =
					DerivedData.RegisterProperty<int>(typeof(DerivedData), new PropertyInfo<int>(nameof(DerivedData.Custom)));
				public int Custom
				{
					get => this.GetProperty(DerivedData.CustomProperty);
					set => this.SetProperty(DerivedData.CustomProperty, value);
				}
			}
			""";

		var baseGeneratedCode =
			"""
			// <auto-generated/>
			
			using CslaGeneratorSerialization.Extensions;
			
			#nullable enable
			
			namespace Domains;
			
			public abstract partial class BaseData
				: global::CslaGeneratorSerialization.IGeneratorSerializable
			{
				void global::CslaGeneratorSerialization.IGeneratorSerializable.SetState(global::CslaGeneratorSerialization.GeneratorFormatterWriterContext context)
				{
					// global::Domains.BaseData.CoreProperty
					context.Writer.Write(this.ReadProperty<string>(global::Domains.BaseData.CoreProperty));
					
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
					// global::Domains.BaseData.CoreProperty
					this.LoadProperty(global::Domains.BaseData.CoreProperty, context.Reader.ReadString());
					
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

		var derivedGeneratedCode =
			"""
			// <auto-generated/>
			
			using CslaGeneratorSerialization.Extensions;
			
			#nullable enable
			
			namespace Domains;
			
			public sealed partial class DerivedData
				: global::CslaGeneratorSerialization.IGeneratorSerializable
			{
				void global::CslaGeneratorSerialization.IGeneratorSerializable.SetState(global::CslaGeneratorSerialization.GeneratorFormatterWriterContext context)
				{
					// global::Domains.BaseData.CoreProperty
					context.Writer.Write(this.ReadProperty<string>(global::Domains.BaseData.CoreProperty));
					
					// global::Domains.DerivedData.CustomProperty
					context.Writer.Write(this.ReadProperty<int>(global::Domains.DerivedData.CustomProperty));
					
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
					// global::Domains.BaseData.CoreProperty
					this.LoadProperty(global::Domains.BaseData.CoreProperty, context.Reader.ReadString());
					
					// global::Domains.DerivedData.CustomProperty
					this.LoadProperty(global::Domains.DerivedData.CustomProperty, context.Reader.ReadInt32());
					
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
				("Domains.BaseData_GeneratorSerialization.g.cs", baseGeneratedCode),
				("Domains.DerivedData_GeneratorSerialization.g.cs", derivedGeneratedCode)
			],
			[]);
	}
}