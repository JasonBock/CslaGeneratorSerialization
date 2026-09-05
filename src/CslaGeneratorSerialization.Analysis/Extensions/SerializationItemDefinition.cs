using CslaGeneratorSerialization.Analysis.Models;
using Microsoft.CodeAnalysis;

namespace CslaGeneratorSerialization.Analysis.Extensions;

internal sealed record SerializationItemDefinition(
	string PropertyInfoFieldName, INamedTypeSymbol PropertyInfoContainingType, ITypeSymbol PropertyInfoDataType);