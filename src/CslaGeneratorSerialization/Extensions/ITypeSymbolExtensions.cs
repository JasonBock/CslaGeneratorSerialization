using Csla.Serialization.Mobile;
using Microsoft.CodeAnalysis;

namespace CslaGeneratorSerialization.Extensions;

internal static class ITypeSymbolExtensions
{
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

	internal static bool IsMobileObject(this ITypeSymbol self) =>
		(self.Name == nameof(IMobileObject) && self.ContainingAssembly.Name == "Csla") ||
			self.AllInterfaces.Any(_ => _.IsMobileObject());

	internal static bool IsGeneratorSerializable(this ITypeSymbol self) =>
		(self.Name == nameof(IGeneratorSerializable) && self.ContainingAssembly.Name == "CslaGeneratorSerializable") ||
			self.AllInterfaces.Any(_ => _.IsGeneratorSerializable());
}