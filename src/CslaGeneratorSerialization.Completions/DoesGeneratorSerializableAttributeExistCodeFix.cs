using CslaGeneratorSerialization.Completions.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;

namespace CslaGeneratorSerialization.Completions;

/// <summary>
/// Defines a code fix to add <c>[GeneratorSerializable]</c> to a business object.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp)]
[Shared]
public sealed class DoesGeneratorSerializableAttributeExistCodeFix
  : CodeFixProvider
{
	/// <summary>
	/// Specifies the code fix title.
	/// </summary>
	public const string AddAttributeTitle = "Add [GeneratorSerializable]";

	/// <summary>
	/// Gets the <see cref="FixAllProvider"/> value.
	/// </summary>
	/// <returns><see cref="WellKnownFixAllProviders.BatchFixer"/></returns>
	public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

	/// <summary>
	/// Registers necessary code fixes.
	/// </summary>
	/// <param name="context">A <see cref="CodeFixContext"/> instance.</param>
	/// <returns>A <see cref="Task"/> instance.</returns>
	public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var root = (await context.Document.GetSyntaxRootAsync(context.CancellationToken))!;
		var diagnostic = context.Diagnostics.First();
		var diagnosticSpan = diagnostic.Location.SourceSpan;

		context.CancellationToken.ThrowIfCancellationRequested();

		var classNode = root.FindNode(diagnosticSpan) as ClassDeclarationSyntax;

		await DoesGeneratorSerializableAttributeExistCodeFix.AddCodeFixAsync(context, root, diagnostic, classNode!);
	}

	private static async Task AddCodeFixAsync(CodeFixContext context, SyntaxNode root, 
		Diagnostic diagnostic, ClassDeclarationSyntax classNode)
	{
		var typeSymbol = (await context.Document.GetSemanticModelAsync()).GetDeclaredSymbol(classNode)!;

		// Add the attribute
		var attribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName("GeneratorSerializable"));
		var attributeList = SyntaxFactory.AttributeList(SyntaxFactory.SeparatedList<AttributeSyntax>().Add(attribute));
		var newClassNode = classNode.AddAttributeLists(attributeList);

		// Add "partial" if it doesn't exist.
		if (!newClassNode.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword)))
		{
			newClassNode = newClassNode.AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));
		}

		root = root.ReplaceNode(classNode, newClassNode);

		// Check to see if a using directives needs to be added.
		if (root.RequiresUsingDirective("CslaGeneratorSerialization", typeSymbol.ContainingNamespace))
		{
			root = ((CompilationUnitSyntax)root).AddUsings(
			  SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("CslaGeneratorSerialization")));
		}

		context.RegisterCodeFix(
			CodeAction.Create(
				DoesGeneratorSerializableAttributeExistCodeFix.AddAttributeTitle,
				_ => Task.FromResult(context.Document.WithSyntaxRoot(root)),
				DoesGeneratorSerializableAttributeExistCodeFix.AddAttributeTitle),
			diagnostic);
	}

	/// <summary>
	/// Gets a list of diagnostic identifiers that this code fixer can address.
	/// </summary>
	public override ImmutableArray<string> FixableDiagnosticIds => ["CGSA1"];
}