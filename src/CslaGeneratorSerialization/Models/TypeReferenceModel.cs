using CslaGeneratorSerialization.Extensions;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace CslaGeneratorSerialization.Models;

internal sealed record TypeReferenceModel
{
	internal TypeReferenceModel(ITypeSymbol type, Compilation compilation)
	{
		this.Name = type.Name;
		this.Namespace =
			type.ContainingNamespace is not null ?
				!type.ContainingNamespace.IsGlobalNamespace ?
					type.ContainingNamespace.ToDisplayString() :
					null :
				null;

		this.FullName = !string.IsNullOrWhiteSpace(this.Namespace) ?
			$"{this.Namespace}.{this.Name}" : this.Name;
		this.FullyQualifiedName = type.GetFullyQualifiedName(compilation);

		this.IsValueType = type.IsValueType;
		this.IsNullable = type.NullableAnnotation == NullableAnnotation.Annotated;
		// The reason we have to check both conditions here is that a BO in this project
		// may not have code generated yet for serialization, but...
		// it may have been done in another project. So we have to check
		// for both conditions, and then hope (?) that this custom serializer
		// will target the type and gen the code for it.
		this.IsSerializable = type.IsGeneratorSerializable() || type.IsMobileObject();
		this.SpecialType = type.SpecialType;

		if (type is IArrayTypeSymbol arrayTypeSymbol)
		{
			this.Array = new ArrayTypeReferenceModel(arrayTypeSymbol, compilation);
		}
	
		this.TypeArguments = type is INamedTypeSymbol namedTypeSymbol ?
			namedTypeSymbol.TypeArguments.Select(
				_ => new TypeReferenceModel(_, compilation)).ToImmutableArray() : [];
	}

	internal ArrayTypeReferenceModel? Array { get; }
	internal string FullName { get; }
	internal string FullyQualifiedName { get; }
	internal bool IsArray { get; }
	internal bool IsSerializable { get; }
   internal bool IsNullable { get; }
	internal bool IsValueType { get; }
	internal string Name { get; }
	internal string? Namespace { get; }
	internal EquatableArray<TypeReferenceModel> TypeArguments { get; }
	internal SpecialType SpecialType { get; }
}