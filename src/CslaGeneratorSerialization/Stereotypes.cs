using CslaGeneratorSerialization.Extensions;
using Microsoft.CodeAnalysis;

namespace CslaGeneratorSerialization;

internal sealed class Stereotypes
{
	internal Stereotypes(Compilation compilation)
	{
		this.BusinessBase = compilation.GetTypeByMetadataName("Csla.IBusinessBase")!;
		this.BusinessListBase = compilation.GetTypeByMetadataName("Csla.IBusinessListBase`1")!;
	}

	public (StereotypeKind, ITypeSymbol?) GetStereotype(ITypeSymbol type, Compilation compilation)
	{
		if (type.DerivesFrom(this.BusinessBase))
		{
			return (StereotypeKind.BusinessBase, type);
		}
		else if (type.DerivesFrom(this.BusinessListBase))
		{
			var target = type.AllInterfaces.Single(_ => SymbolEqualityComparer.Default.Equals(_.OriginalDefinition, this.BusinessListBase));

			return (StereotypeKind.BusinessListBase, target.TypeArguments[0]);
		}
		else
		{
			return (StereotypeKind.None, null);
		}
	}

	internal INamedTypeSymbol BusinessBase { get; }
	internal INamedTypeSymbol BusinessListBase { get; }
}