using Csla;
using CslaGeneratorSerialization.Analysis.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace CslaGeneratorSerialization.Analysis.Tests.Extensions;

internal static class INamedTypeSymbolExtensionsTests
{
	[Test]
	public static async Task GetPropertyInfoFieldsWhenTypeHasPropertyInfoFieldsAsync()
	{
		var code =
			"""
			using Csla;

			public sealed class Customer
				: BusinessBase<Customer>
			{
				public static readonly PropertyInfo<uint> AgeProperty =
					Customer.RegisterProperty<uint>(nameof(Customer.Age));
				public uint Age
				{
					get => this.GetProperty(Customer.AgeProperty);
					set => this.SetProperty(Customer.AgeProperty, value);
				}
			}
			""";

		var typeSymbol = await INamedTypeSymbolExtensionsTests.GetNamedTypeSymbolAsync(code);
		var fields = typeSymbol.GetPropertyInfoFields()!;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(fields, Has.Count.EqualTo(1));
			Assert.That(fields[0].Name, Is.EqualTo("AgeProperty"));
		}
	}

	[Test]
	public static async Task GetPropertyInfoFieldsWhenTypeDoesNotHavePropertyInfoFieldsAsync()
	{
		var code =
			"""
			using Csla;

			public sealed class Customer
			{
				public static readonly uint AgeProperty;
			}
			""";

		var typeSymbol = await INamedTypeSymbolExtensionsTests.GetNamedTypeSymbolAsync(code);
		var fields = typeSymbol.GetPropertyInfoFields()!;
		Assert.That(fields, Has.Count.EqualTo(0));
	}

	private static async Task<INamedTypeSymbol> GetNamedTypeSymbolAsync(string source)
	{
		var syntaxTree = CSharpSyntaxTree.ParseText(source);
		var compilation = CSharpCompilation.Create("generator", [syntaxTree],
			Shared.References.Value.Concat([MetadataReference.CreateFromFile(typeof(BusinessBase<>).Assembly.Location)]),
			new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
		var model = compilation.GetSemanticModel(syntaxTree, true);

		var typeSyntax = (await syntaxTree.GetRootAsync()).DescendantNodes(_ => true)
			.OfType<TypeDeclarationSyntax>().Single();
		return (model.GetDeclaredSymbol(typeSyntax) as INamedTypeSymbol)!;
	}
}