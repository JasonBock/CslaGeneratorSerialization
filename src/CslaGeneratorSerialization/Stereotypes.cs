using CslaGeneratorSerialization.Extensions;
using Microsoft.CodeAnalysis;

namespace CslaGeneratorSerialization;

internal sealed class Stereotypes
{
	internal Stereotypes(Compilation compilation)
	{
		this.BusinessBase = compilation.GetTypeByMetadataName("Csla.BusinessBase`1")!;
		this.BusinessListBase = compilation.GetTypeByMetadataName("Csla.BusinessListBase`2")!;
		this.ReadOnlyBase = compilation.GetTypeByMetadataName("Csla.ReadOnlyBase`1")!;
		this.ReadOnlyListBase = compilation.GetTypeByMetadataName("Csla.ReadOnlyListBase`2")!;
		this.CommandBase = compilation.GetTypeByMetadataName("Csla.CommandBase`1")!;

		this.IBusinessBase = compilation.GetTypeByMetadataName("Csla.IBusinessBase")!;
		this.IBusinessListBase = compilation.GetTypeByMetadataName("Csla.IBusinessListBase`1")!;
		this.IReadOnlyBase = compilation.GetTypeByMetadataName("Csla.IReadOnlyBase")!;
		this.IReadOnlyListBase = compilation.GetTypeByMetadataName("Csla.IReadOnlyListBase`1")!;
		this.ICommandBase = compilation.GetTypeByMetadataName("Csla.ICommandBase")!;
	}

	public (StereotypeKind, ITypeSymbol?) GetStereotype(ITypeSymbol type)
	{
		if (type.DerivesFrom(this.BusinessBase) || type.DerivesFrom(this.IBusinessBase))
		{
			return (StereotypeKind.BusinessBase, type);
		}
		else if (type.DerivesFrom(this.ReadOnlyBase) || type.DerivesFrom(this.IReadOnlyBase))
		{
			return (StereotypeKind.ReadOnlyBase, type);
		}
		else if (type.DerivesFrom(this.CommandBase) || type.DerivesFrom(this.ICommandBase))
		{
			return (StereotypeKind.CommandBase, type);
		}
		else if (type.DerivesFrom(this.BusinessListBase) || type.DerivesFrom(this.IBusinessListBase))
		{
			var target = type.BaseType;

			while (target is not null)
			{
				if (SymbolEqualityComparer.Default.Equals(target.OriginalDefinition, this.BusinessListBase))
				{
					return (StereotypeKind.BusinessListBase, target.TypeArguments[1]);
				}

				target = target.BaseType;
			}

			throw new NotSupportedException();
		}
		else if (type.DerivesFrom(this.ReadOnlyListBase) || type.DerivesFrom(this.IReadOnlyListBase))
		{
			var target = type.BaseType;

			while (target is not null)
			{
				if (SymbolEqualityComparer.Default.Equals(target.OriginalDefinition, this.ReadOnlyListBase))
				{
					return (StereotypeKind.ReadOnlyListBase, target.TypeArguments[1]);
				}

				target = target.BaseType;
			}

			throw new NotSupportedException();
		}
		else
		{
			return (StereotypeKind.None, null);
		}
	}

	internal INamedTypeSymbol BusinessBase { get; }
	internal INamedTypeSymbol BusinessListBase { get; }
	internal INamedTypeSymbol CommandBase { get; }
	internal INamedTypeSymbol ReadOnlyBase { get; }
	internal INamedTypeSymbol ReadOnlyListBase { get; }

	internal INamedTypeSymbol IBusinessBase { get; }
	internal INamedTypeSymbol IBusinessListBase { get; }
	internal INamedTypeSymbol ICommandBase { get; }
	internal INamedTypeSymbol IReadOnlyBase { get; }
	internal INamedTypeSymbol IReadOnlyListBase { get; }
}