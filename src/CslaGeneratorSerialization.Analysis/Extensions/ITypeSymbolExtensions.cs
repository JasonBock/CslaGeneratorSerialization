using Microsoft.CodeAnalysis;

namespace CslaGeneratorSerialization.Analysis.Extensions;

internal static class ITypeSymbolExtensions
{
	extension(ITypeSymbol self)
	{
		internal bool DerivesFrom(ITypeSymbol other)
		{
			var target = self.OriginalDefinition;

			while (target is not null)
			{
				if (SymbolEqualityComparer.Default.Equals(target, other) ||
					target.AllInterfaces.Any(_ => _.DerivesFrom(other)))
				{
					return true;
				}

				target = target.BaseType?.OriginalDefinition ?? null;
			}

			return false;
		}

		internal bool DerivesFrom(string otherName, string otherNamespace)
		{
			var target = self.OriginalDefinition;

			while (target is not null)
			{
				if ((target.Name == otherName && target.GetNamespace() == otherNamespace) ||
					target.AllInterfaces.Any(_ => _.DerivesFrom(otherName, otherNamespace)))
				{
					return true;
				}

				target = target.BaseType?.OriginalDefinition ?? null;
			}

			return false;
		}

		internal string GetNamespace()
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

		internal string GetFullyQualifiedName(Compilation compilation, bool includeNullableAnnotation = true)
		{
			const string GlobalPrefix = "global::";

			var symbolFormatter = SymbolDisplayFormat.FullyQualifiedFormat.AddMiscellaneousOptions(
					SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

			if (includeNullableAnnotation)
			{
				symbolFormatter = symbolFormatter.AddMiscellaneousOptions(
					SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);
			}
			else
			{
				symbolFormatter = symbolFormatter.RemoveMiscellaneousOptions(
					SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);
			}

			var symbolName = self.ToDisplayString(symbolFormatter);

			if (!includeNullableAnnotation && symbolName.EndsWith("?"))
			{
				symbolName = symbolName.Substring(0, symbolName.Length - 1);
			}

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

		internal bool HasErrors() =>
			self.TypeKind == TypeKind.Error ||
				(self is INamedTypeSymbol namedSelf && namedSelf.TypeArguments.Any(_ => _.HasErrors()));

		internal bool IsMobileObject() =>
			(self.Name == "IMobileObject" &&
				self.GetNamespace() == "Csla.Serialization.Mobile" &&
				self.ContainingAssembly.Name == "Csla") ||
				self.AllInterfaces.Any(_ => _.IsMobileObject());

		internal bool IsGeneratorSerializable() =>
			(self.Name == "IGeneratorSerializable" &&
				self.GetNamespace() == "CslaGeneratorSerialization" &&
				self.ContainingAssembly.Name == "CslaGeneratorSerialization") ||
				self.AllInterfaces.Any(_ => _.IsGeneratorSerializable());
	}
}