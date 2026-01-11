using CslaGeneratorSerialization.Analysis;
using NUnit.Framework;

namespace CslaGeneratorSerialization.Completions.Tests;

internal static class DoesGeneratorSerializableAttributeExistCodeFixTests
{
	[Test]
	public static void VerifyGetFixableDiagnosticIds()
	{
		var fix = new DoesGeneratorSerializableAttributeExistCodeFix();
		var ids = fix.FixableDiagnosticIds;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(ids, Has.Length.EqualTo(1), nameof(ids.Length));
			Assert.That(ids[0], Is.EqualTo("CGSA1"));
		}
	}

	[Test]
	public static async Task VerifyGetFixesWhenWhenUsingDoesNotExistAsync()
	{
		var originalCode =
			"""
			using Csla;
			
			public partial class [|Customer|]
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
		var fixedCode =
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

		await TestAssistants.RunCodeFixAsync<DoesGeneratorSerializableAttributeExistAnalyzer, DoesGeneratorSerializableAttributeExistCodeFix>(
			originalCode, fixedCode, 0);
	}
}