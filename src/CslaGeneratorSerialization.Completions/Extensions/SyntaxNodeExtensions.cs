using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CslaGeneratorSerialization.Completions.Extensions;

internal static class SyntaxNodeExtensions
{
	internal static bool HasUsing(this SyntaxNode self, string qualifiedName)
	{
		if (self is null) { throw new ArgumentNullException(nameof(self)); }

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
			if (usingNode.Name!.ToFullString().Contains(qualifiedName))
			{
				return true;
			}
		}

		return false;
	}
}