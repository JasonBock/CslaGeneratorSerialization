using CslaGeneratorSerialization.Extensions;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace CslaGeneratorSerialization.Models;

internal sealed record TypeReferenceModel
{
	internal TypeReferenceModel(INamedTypeSymbol type, Compilation compilation)
	{
		this.Name = type.Name;
		this.FullyQualifiedName = type.GetFullyQualifiedName(compilation);
		this.Namespace =
			type.ContainingNamespace is not null ?
				!type.ContainingNamespace.IsGlobalNamespace ?
					type.ContainingNamespace.ToDisplayString() :
					null :
				null;
		this.IsValueType = type.IsValueType;
		this.IsNullable = type.NullableAnnotation == NullableAnnotation.Annotated;
		this.IsGeneratorSerializable = type.IsGeneratorSerializable();
		this.SpecialType = type.SpecialType;
		this.TypeArguments = type.TypeArguments.Select(
			_ => new TypeReferenceModel((INamedTypeSymbol)_, compilation)).ToImmutableArray();
	}

	internal string FullyQualifiedName { get; }
	internal bool IsGeneratorSerializable { get; }
   internal bool IsNullable { get; }
	internal bool IsValueType { get; }
	internal string Name { get; }
	internal string? Namespace { get; }
	internal EquatableArray<TypeReferenceModel> TypeArguments { get; }
	internal SpecialType SpecialType { get; }
}