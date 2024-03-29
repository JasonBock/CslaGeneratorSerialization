using Csla.Serialization.Mobile;
using Microsoft.CodeAnalysis;

namespace CslaGeneratorSerialization.Extensions;

internal static class ITypeSymbolExtensions
{
	internal static string GetNamespace(this ITypeSymbol self)
	{
		var namespaces = new List<INamespaceSymbol>();

		var @namespace = self.ContainingNamespace;

		while (@namespace is not null &&
			!@namespace.IsGlobalNamespace)
		{
			namespaces.Add(@namespace);
			@namespace = @namespace.ContainingNamespace;
		}

		namespaces.Reverse();

		return string.Join(".", namespaces.Select(_ => _.Name));
	}

	internal static string GetFullyQualifiedName(this ITypeSymbol self, Compilation compilation)
	{
		const string GlobalPrefix = "global::";

		var symbolFormatter = SymbolDisplayFormat.FullyQualifiedFormat
			.AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

		var symbolName = self.ToDisplayString(symbolFormatter);

		// If the symbol name has "global::" at the start,
		// then see if the type's assembly has at least one alias.
		// If there is one, then replace "global::" with "{alias}::",
		// but only the FIRST "global::"

		// TODO: self could be a closed generic where the
		// type arguments need aliases. I should add a test for that to see
		// what ToDisplayString() would do in that case.

		if (symbolName.StartsWith(GlobalPrefix))
		{
			var aliases = compilation.GetMetadataReference(self.ContainingAssembly)?.Properties.Aliases ?? [];

			if (aliases.Length > 0)
			{
				symbolName = $"{aliases[0]}::{symbolName.Remove(0, GlobalPrefix.Length)}";
			}
		}

		return symbolName;
	}

	internal static bool HasErrors(this ITypeSymbol self) =>
		self.TypeKind == TypeKind.Error ||
			(self is INamedTypeSymbol namedSelf && namedSelf.TypeArguments.Any(_ => _.HasErrors()));

	internal static bool IsMobileObject(this ITypeSymbol self) =>
		(self.Name == nameof(IMobileObject) && self.GetNamespace() == "Csla.Serialization.Mobile" && self.ContainingAssembly.Name == "Csla") ||
			self.AllInterfaces.Any(_ => _.IsMobileObject());

	internal static bool IsGeneratorSerializable(this ITypeSymbol self) =>
		(self.Name == nameof(IGeneratorSerializable) && self.GetNamespace() == "CslaGeneratorSerialization" && self.ContainingAssembly.Name == "CslaGeneratorSerialization") ||
			self.AllInterfaces.Any(_ => _.IsGeneratorSerializable());
}