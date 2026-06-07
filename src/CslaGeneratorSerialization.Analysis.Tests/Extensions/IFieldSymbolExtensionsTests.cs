using Csla;
using CslaGeneratorSerialization.Analysis.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace CslaGeneratorSerialization.Analysis.Tests.Extensions;

internal static class IFieldSymbolExtensionsTests
{
	[Test]
	public static async Task IsPropertyInfoWhenFieldIsPropertyInfoAsync()
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

		var fieldSymbol = await IFieldSymbolExtensionsTests.GetFieldSymbolAsync(code);
		Assert.That(fieldSymbol.IsPropertyInfo(), Is.True);
	}

	[Test]
	public static async Task IsPropertyInfoWhenFieldIsNotPropertyInfoAsync()
	{
		var code =
			"""
			using Csla;

			public sealed class Customer
			{
				public static readonly uint AgeProperty;
			}
			""";

		var fieldSymbol = await IFieldSymbolExtensionsTests.GetFieldSymbolAsync(code);
		Assert.That(fieldSymbol.IsPropertyInfo(), Is.False);
	}

	private static async Task<IFieldSymbol> GetFieldSymbolAsync(string source)
	{
		var syntaxTree = CSharpSyntaxTree.ParseText(source);
		var compilation = CSharpCompilation.Create("generator", [syntaxTree],
			Shared.References.Value.Concat([MetadataReference.CreateFromFile(typeof(BusinessBase<>).Assembly.Location)]), 
			new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
		var model = compilation.GetSemanticModel(syntaxTree, true);

		var typeSyntax = (await syntaxTree.GetRootAsync()).DescendantNodes(_ => true)
			.OfType<TypeDeclarationSyntax>().Single();
		return (model.GetDeclaredSymbol(typeSyntax)!.GetMembers("AgeProperty")[0] as IFieldSymbol)!;
	}
}