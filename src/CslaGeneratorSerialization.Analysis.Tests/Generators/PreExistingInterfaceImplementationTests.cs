using NUnit.Framework;

namespace CslaGeneratorSerialization.Analysis.Tests.Generators;

internal static class PreExistingInterfaceImplementationTests
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
				: BusinessBase<Data>, IGeneratorSerializable
			{
				public static readonly PropertyInfo<string> ContentsProperty =
					RegisterProperty<string>(_ => _.Contents);
				public string Contents
				{
					get => this.GetProperty(Data.ContentsProperty);
					set => this.SetProperty(Data.ContentsProperty, value);
				}

				public void GetState(GeneratorFormatterReaderContext context) { }
			
				public void SetState(GeneratorFormatterWriterContext context) { }
			}
			""";

		await TestAssistants.RunGeneratorAsync<GeneratorSerializationGenerator>(code,
			[],
			[]);
	}
}