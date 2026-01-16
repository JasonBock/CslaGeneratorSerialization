using CslaGeneratorSerialization.Analysis.Descriptors;
using CslaGeneratorSerialization.Analysis.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using System.Globalization;

namespace CslaGeneratorSerialization.Analysis.Tests.Diagnostics;

internal static class BusinessObjectDoesNotHaveSerializationAttributeDiagnosticTests
{
	[Test]
	public static async Task CreateAsync()
	{
		var syntaxTree = CSharpSyntaxTree.ParseText("public class X { }");
		var compilation = CSharpCompilation.Create("generator", [syntaxTree],
			Shared.References.Value, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
		var model = compilation.GetSemanticModel(syntaxTree, true);

		var typeSyntax = (await syntaxTree.GetRootAsync()).DescendantNodes(_ => true)
			.OfType<TypeDeclarationSyntax>().Single();

		var descriptor = BusinessObjectDoesNotHaveSerializationAttributeDiagnostic.Create(typeSyntax, model.GetDeclaredSymbol(typeSyntax)!);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(descriptor.GetMessage(CultureInfo.InvariantCulture), Is.EqualTo("The type X is not marked with GeneratorSerializableAttribute"));
			Assert.That(descriptor.Descriptor.Title.ToString(CultureInfo.CurrentCulture), Is.EqualTo(BusinessObjectDoesNotHaveSerializationAttributeDescriptor.Title));
			Assert.That(descriptor.Id, Is.EqualTo(BusinessObjectDoesNotHaveSerializationAttributeDescriptor.Id));
			Assert.That(descriptor.Severity, Is.EqualTo(DiagnosticSeverity.Error));
		}
	}
}