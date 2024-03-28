using Microsoft.CodeAnalysis;

namespace CslaGeneratorSerialization.Models;

internal sealed record ArrayTypeReferenceModel
{
	internal ArrayTypeReferenceModel(IArrayTypeSymbol arrayType, Compilation compilation)
	{
		this.ElementType = new TypeReferenceModel((INamedTypeSymbol)arrayType.ElementType, compilation);
		this.Rank = arrayType.Rank;
	}

	internal TypeReferenceModel ElementType { get; }
	internal int Rank { get; }
}