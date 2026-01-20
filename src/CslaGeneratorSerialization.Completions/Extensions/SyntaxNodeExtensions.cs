using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CslaGeneratorSerialization.Completions.Extensions;

internal static class SyntaxNodeExtensions
{
	internal static bool RequiresUsingDirective(this SyntaxNode self, string qualifiedName, INamespaceSymbol? namespaceSymbol)
	{
		if (self is null) { throw new ArgumentNullException(nameof(self)); }

		if (!namespaceSymbol?.IsGlobalNamespace ?? false)
		{
			while (!namespaceSymbol!.ContainingNamespace?.IsGlobalNamespace ?? false)
			{
				namespaceSymbol = namespaceSymbol.ContainingNamespace;
			}

			if (namespaceSymbol?.Name == qualifiedName)
			{
				return false;
			}
		}

		var root = self;

		while (true)
		{
			if (root.Parent is not null)
			{
				root = root.Parent;
			}
			else
			{
				break;
			}
		}

		var usingNodes = root.DescendantNodes(_ => true).OfType<UsingDirectiveSyntax>();

		foreach (var usingNode in usingNodes)
		{
			if (usingNode.Name!.ToFullString().Split('.')[0] == qualifiedName)
			{
				return false;
			}
		}

		return true;
	}
}