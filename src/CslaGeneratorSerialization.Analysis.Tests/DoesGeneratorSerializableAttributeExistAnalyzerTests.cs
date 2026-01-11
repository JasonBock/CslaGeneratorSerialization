using CslaGeneratorSerialization.Analysis.Descriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace CslaGeneratorSerialization.Analysis.Tests;

internal static class DoesGeneratorSerializableAttributeExistAnalyzerTests
{
	[Test]
	public static async Task AnalyzeWhenMobileObjectDoesNotExistAsync()
	{
		var code =
			"""
			public class Person { }
			""";

		await TestAssistants.RunAnalyzerAsync<DoesGeneratorSerializableAttributeExistAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeWhenMobileObjectExistsWithAttributeAsync()
	{
		var code =
			"""
			using Csla;
			using CslaGeneratorSerialization;

			[GeneratorSerializable]
			public partial class Customer
				: BusinessBase<Customer>
			{
				[Create]
				private void Create() { }

				public static readonly PropertyInfo<uint> AgeProperty =
					Customer.RegisterProperty<uint>(nameof(Customer.Age));
				public uint Age
				{
					get => this.GetProperty(Customer.AgeProperty);
					set => this.SetProperty(Customer.AgeProperty, value);
				}
			}			
			""";

		await TestAssistants.RunAnalyzerAsync<DoesGeneratorSerializableAttributeExistAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeWhenMobileObjectExistsWithoutAttributeAsync()
	{
		var code =
			"""
			using Csla;

			public partial class Customer
				: BusinessBase<Customer>
			{
				[Create]
				private void Create() { }

				public static readonly PropertyInfo<uint> AgeProperty =
					Customer.RegisterProperty<uint>(nameof(Customer.Age));
				public uint Age
				{
					get => this.GetProperty(Customer.AgeProperty);
					set => this.SetProperty(Customer.AgeProperty, value);
				}
			}			
			""";

		var diagnostic = new DiagnosticResult(BusinessObjectDoesNotHaveSerializationAttributeDescriptor.Id, DiagnosticSeverity.Error)
			.WithSpan(3, 1, 16, 2).WithArguments("Customer");
		await TestAssistants.RunAnalyzerAsync<DoesGeneratorSerializableAttributeExistAnalyzer>(code, [diagnostic]);
	}
}