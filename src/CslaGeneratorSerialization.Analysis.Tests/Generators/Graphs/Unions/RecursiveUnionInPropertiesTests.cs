using NUnit.Framework;

namespace CslaGeneratorSerialization.Analysis.Tests.Generators.Graphs.Unions;

internal static class RecursiveUnionInPropertiesTests
{
	[Test, Ignore("for now")]
	public static async Task GenerateAsync()
	{
		var code =
			"""
			using Csla;
			using CslaGeneratorSerialization;
			using System;

			#nullable enable
			
			namespace Domains;

			public union DataOne(int, string, DataTwo);
			public union DataTwo(int, string, DataOne);
			
			[GeneratorSerializable]
			public partial class Customer
				: BusinessBase<Customer>
			{
				[Create]
				private void Create() { }

				public static readonly PropertyInfo<DataOne> IdentifierProperty =
					Customer.RegisterProperty<DataOne>(nameof(Customer.Identifier));
				public DataOne Identifier
				{
					get => this.GetProperty(Customer.IdentifierProperty);
					set => this.SetProperty(Customer.IdentifierProperty, value);
				}
			}
			""";

		var generatedCode = "";

		var readerExtensionsCode = "";

		var writerExtensionsCode = "";

		await TestAssistants.RunGeneratorAsync<GeneratorSerializationGenerator>(code,
			[
				("Domains.Customer_GeneratorSerialization.g.cs", generatedCode),
				("GeneratorFormatterReaderContextExtensions.g.cs", readerExtensionsCode),
				("GeneratorFormatterWriterContextExtensions.g.cs", writerExtensionsCode)
			],
			[]);
	}
}