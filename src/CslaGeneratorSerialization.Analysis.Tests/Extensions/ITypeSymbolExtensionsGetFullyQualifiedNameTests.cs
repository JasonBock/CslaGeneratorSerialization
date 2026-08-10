using CslaGeneratorSerialization.Analysis.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace CslaGeneratorSerialization.Analysis.Tests.Extensions;

internal static class ITypeSymbolExtensionsGetFullyQualifiedNameTests
{
	[Test]
	public static async Task GenerateFullyQualifiedNameAsync()
	{
		var syntaxTree = CSharpSyntaxTree.ParseText(
			"""
			namespace Outer.Inner;

			public class Data { }
			""");
		var compilation = CSharpCompilation.Create("generator", [syntaxTree],
			Shared.References.Value, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
		var model = compilation.GetSemanticModel(syntaxTree, true);
		var typeSyntax = (await syntaxTree.GetRootAsync()).DescendantNodes(_ => true)
			.OfType<TypeDeclarationSyntax>().Single();
		var typeSymbol = model.GetDeclaredSymbol(typeSyntax)!;

		Assert.That(typeSymbol.GetFullyQualifiedName(compilation), Is.EqualTo("global::Outer.Inner.Data"));
	}
}