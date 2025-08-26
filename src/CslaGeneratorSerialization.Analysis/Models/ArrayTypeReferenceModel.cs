using Microsoft.CodeAnalysis;

namespace CslaGeneratorSerialization.Analysis.Models;

internal sealed record ArrayTypeReferenceModel
{
	internal ArrayTypeReferenceModel(IArrayTypeSymbol arrayType, Compilation compilation, Stereotypes stereotypes)
	{
		(var elementType, var rank) = GetElementType(arrayType);
		(this.ElementType, this.Rank) = (new TypeReferenceModel(elementType, compilation, stereotypes), rank);
	}

	private static (ITypeSymbol element, int ranks) GetElementType(IArrayTypeSymbol arrayType)
	{
		var rank = 1;

		var elementType = arrayType.ElementType;

		while (elementType is IArrayTypeSymbol rootElementType)
		{
			rank++;
			elementType = rootElementType.ElementType;
		}

		return (elementType, rank);
	}

	internal TypeReferenceModel ElementType { get; }
	internal int Rank { get; }
}