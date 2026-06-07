using CslaGeneratorSerialization.Analysis.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace CslaGeneratorSerialization.Analysis.Tests.Extensions;

internal static class ITypeSymbolExtensionsIsGeneratorSerializableTests
{
	[Test]
	public static async Task IsMobileObjectWhenTypeIsMobileObjectAsync()
	{
		var code =
			"""
			using CslaGeneratorSerialization;

			public sealed class Customer
				: IGeneratorSerializable
			{
				public void GetState(GeneratorFormatterReaderContext reader) { }
				public void SetState(GeneratorFormatterWriterContext writer) { }
			}
			""";

		var typeSymbol = await ITypeSymbolExtensionsIsGeneratorSerializableTests.GetTypeSymbolAsync(code);
		Assert.That(typeSymbol.IsGeneratorSerializable(), Is.True);
	}

	[Test]
	public static async Task IsMobileObjectWhenTypeIsNotMobileObjectAsync()
	{
		var code =
			"""
			public sealed class Customer { }
			""";
		var typeSymbol = await ITypeSymbolExtensionsIsGeneratorSerializableTests.GetTypeSymbolAsync(code);
		Assert.That(typeSymbol.IsGeneratorSerializable(), Is.False);
	}

	private static async Task<ITypeSymbol> GetTypeSymbolAsync(string source)
	{
		var syntaxTree = CSharpSyntaxTree.ParseText(source);
		var compilation = CSharpCompilation.Create("generator", [syntaxTree],
			Shared.References.Value.Concat([MetadataReference.CreateFromFile(typeof(IGeneratorSerializable).Assembly.Location)]),
			new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
		var model = compilation.GetSemanticModel(syntaxTree, true);

		var typeSyntax = (await syntaxTree.GetRootAsync()).DescendantNodes(_ => true)
			.OfType<TypeDeclarationSyntax>().Single();
		return (model.GetDeclaredSymbol(typeSyntax) as ITypeSymbol)!;
	}
}