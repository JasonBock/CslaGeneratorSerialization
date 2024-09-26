﻿using NUnit.Framework;

namespace CslaGeneratorSerialization.Tests.Generators.Graphs;

public static class ReadOnlyListBaseTests
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
			public partial class Experiments
				: ReadOnlyBase<Experiments>
			{
				[Create]
				private void Create() =>
					this.Values = this.ApplicationContext.GetRequiredService<IChildDataPortal<Datum>>().CreateChild();

				public static readonly PropertyInfo<Datum> ValuesProperty =
					Experiments.RegisterProperty<Datum>(_ => _.Values);
				public Datum Values
				{
					get => this.ReadProperty(Experiments.ValuesProperty);
					private set => this.LoadProperty(Experiments.ValuesProperty, value);
				}
			}

			[GeneratorSerializable]
			public partial class Datum
				: ReadOnlyListBase<Datum, Data>
			{
				[CreateChild]
				private void CreateChild() { }
			}

			[GeneratorSerializable]
			public partial class Data
				: ReadOnlyBase<Data>
			{
				[CreateChild]
				private void CreateChild() { }

				public static readonly PropertyInfo<string> ValueProperty =
					Data.RegisterProperty<string>(_ => _.Value);
				public string Value
				{
					get => this.ReadProperty(Data.ValueProperty);
					private set => this.LoadProperty(Data.ValueProperty, value);
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
					context.Write(this.ReadProperty<global::Domains.Datum>(global::Domains.Experiments.ValuesProperty), false);
				}
				
				void global::CslaGeneratorSerialization.IGeneratorSerializable.GetState(global::CslaGeneratorSerialization.GeneratorFormatterReaderContext context)
				{
					// global::Domains.Experiments.ValuesProperty
					this.LoadProperty(global::Domains.Experiments.ValuesProperty, context.Read<global::Domains.Datum>(false)!);
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
						context.Write(item, false);
					}
				
					context.Writer.Write(this.IsReadOnly);
				}
				
				void global::CslaGeneratorSerialization.IGeneratorSerializable.GetState(global::CslaGeneratorSerialization.GeneratorFormatterReaderContext context)
				{
					var count = context.Reader.ReadInt32();
				
					using (this.LoadListMode)
					{
						for (var i = 0; i < count; i++)
						{
							this.Add(context.Read<global::Domains.Data>(false)!);
						}
					}
				
					this.IsReadOnly = context.Reader.ReadBoolean();
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
				}
				
				void global::CslaGeneratorSerialization.IGeneratorSerializable.GetState(global::CslaGeneratorSerialization.GeneratorFormatterReaderContext context)
				{
					// global::Domains.Data.ValueProperty
					this.LoadProperty(global::Domains.Data.ValueProperty, context.Reader.ReadString());
				}
			}
			
			""";

		await TestAssistants.RunGeneratorAsync<GeneratorSerializationGenerator>(code,
			[
				("Domains.Experiments_GeneratorSerialization.g.cs", experimentsGeneratedCode),
				("Domains.Datum_GeneratorSerialization.g.cs", datumGeneratedCode),
				("Domains.Data_GeneratorSerialization.g.cs", dataGeneratedCode),
			],
			[]);
	}
}