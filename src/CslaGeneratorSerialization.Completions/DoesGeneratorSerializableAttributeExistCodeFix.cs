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
	/// Specifies the code fix title when adding a using statement.
	/// </summary>
	public const string AddAttributeAndUsingStatementTitle = "Add [GeneratorSerializable] And Using Statement";

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

		DoesGeneratorSerializableAttributeExistCodeFix.AddCodeFix(context, root, diagnostic, classNode!);
	}

	private static SyntaxNode AddAttribute(SyntaxNode root, ClassDeclarationSyntax classNode, string name)
	{
		var attribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName(name));
		var attributeList = SyntaxFactory.AttributeList(SyntaxFactory.SeparatedList<AttributeSyntax>().Add(attribute));
		var newClassNode = classNode.AddAttributeLists(attributeList);
		return root.ReplaceNode(classNode, newClassNode);
	}

	private static void AddCodeFix(CodeFixContext context, SyntaxNode root,
	  Diagnostic diagnostic, ClassDeclarationSyntax classNode)
	{
		var newRoot = DoesGeneratorSerializableAttributeExistCodeFix.AddAttribute(root, classNode, "GeneratorSerializable");

		var description = DoesGeneratorSerializableAttributeExistCodeFix.AddAttributeTitle;

		if (!newRoot.HasUsing("CslaGeneratorSerialization"))
		{
			newRoot = ((CompilationUnitSyntax)newRoot).AddUsings(
			  SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("CslaGeneratorSerialization")));
			description = DoesGeneratorSerializableAttributeExistCodeFix.AddAttributeAndUsingStatementTitle;
		}

		context.RegisterCodeFix(
			CodeAction.Create(
				description,
				_ => Task.FromResult(context.Document.WithSyntaxRoot(newRoot)),
				description), 
			diagnostic);
	}

	/// <summary>
	/// Gets a list of diagnostic identifiers that this code fixer can address.
	/// </summary>
	public override ImmutableArray<string> FixableDiagnosticIds => ["CGSA1"];
}