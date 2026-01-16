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
	public static async Task VerifyGetFixesWhenUsingDoesNotExistAsync()
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

	[Test]
	public static async Task VerifyGetFixesWhenUsingExistsAsync()
	{
		var originalCode =
			"""
			using Csla;
			using CslaGeneratorSerialization;
						
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

	[Test]
	public static async Task VerifyGetFixesWhenUsingDoesNotExistAndClassIsNotPartialAsync()
	{
		var originalCode =
			"""
			using Csla;
			
			public class [|Customer|]
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

	[Test]
	public static async Task VerifyGetFixesWhenUsingExistsAndClassIsNotPartialAsync()
	{
		var originalCode =
			"""
			using Csla;
			using CslaGeneratorSerialization;
						
			public class [|Customer|]
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

	[Test]
	public static async Task VerifyGetFixesWhenClassExistsInNamespaceWithCorrectNameAsync()
	{
		var originalCode =
			"""
			using Csla;
			
			namespace CslaGeneratorSerialization.SomethingElse;

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
			
			namespace CslaGeneratorSerialization.SomethingElse;
			
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

	[Test]
	public static async Task VerifyGetFixesWhenClassExistsInNamespaceWithCorrectNameAndClassIsNotPartialAsync()
	{
		var originalCode =
			"""
			using Csla;
			
			namespace CslaGeneratorSerialization.SomethingElse;

			public class [|Customer|]
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
			
			namespace CslaGeneratorSerialization.SomethingElse;
			
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

	[Test]
	public static async Task VerifyGetFixesWhenNamespaceExistsWithoutCorrectNameAsync()
	{
		var originalCode =
			"""
			using Csla;
			
			namespace ABC.CslaGeneratorSerialization.SomethingElse;

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
			
			namespace ABC.CslaGeneratorSerialization.SomethingElse;
			
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

	[Test]
	public static async Task VerifyGetFixesWhenNamespaceExistsWithoutCorrectNameAndClassIsNotPartialAsync()
	{
		var originalCode =
			"""
			using Csla;
			
			namespace ABC.CslaGeneratorSerialization.SomethingElse;
			
			public class [|Customer|]
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
			
			namespace ABC.CslaGeneratorSerialization.SomethingElse;
			
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