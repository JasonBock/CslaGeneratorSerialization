﻿using CslaGeneratorSerialization.Extensions;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace CslaGeneratorSerialization.Models;

internal sealed record TypeReferenceModel
{
	internal TypeReferenceModel(ITypeSymbol type, Compilation compilation)
	{

		this.Name = type.Name;
		this.Namespace = type.GetNamespace();
		this.FullName = !string.IsNullOrWhiteSpace(this.Namespace) ?
			$"{this.Namespace}.{this.Name}" : this.Name;
		this.FullyQualifiedName = type.GetFullyQualifiedName(compilation);

		this.IsValueType = type.IsValueType;
		this.IsNullable = type.NullableAnnotation == NullableAnnotation.Annotated;
		this.IsSealed = type.IsSealed;
		this.IsAbstract = type.IsAbstract;
		// The reason we have to check both conditions here is that a BO in this project
		// may not have code generated yet for serialization, but...
		// it may have been done in another project. So we have to check
		// for both conditions, and then hope (?) that this custom serializer
		// will target the type and gen the code for it.
		this.IsSerializable = type.IsGeneratorSerializable() || type.IsMobileObject();


		this.SpecialType = type.SpecialType;
		this.TypeKind = type.TypeKind;

		if (type is IArrayTypeSymbol arrayTypeSymbol)
		{
			this.Array = new ArrayTypeReferenceModel(arrayTypeSymbol, compilation);
		}

		if (type is INamedTypeSymbol namedTypeSymbol)
		{
			this.TypeArguments = namedTypeSymbol.TypeArguments.Select(
				_ => new TypeReferenceModel(_, compilation)).ToImmutableArray();

			if (namedTypeSymbol.EnumUnderlyingType is not null)
			{
				this.EnumUnderlyingType = new TypeReferenceModel(namedTypeSymbol.EnumUnderlyingType, compilation);
			}
		}
		else
		{
			this.TypeArguments = [];
		}
	}

	internal ArrayTypeReferenceModel? Array { get; }
	internal TypeReferenceModel? EnumUnderlyingType { get; }
	internal string FullName { get; }
	internal string FullyQualifiedName { get; }
	internal bool IsAbstract { get; }
	internal bool IsArray { get; }
	internal bool IsNullable { get; }
	internal bool IsSealed { get; }
	internal bool IsSerializable { get; }
	internal bool IsValueType { get; }
	internal string Name { get; }
	internal string? Namespace { get; }
	internal EquatableArray<TypeReferenceModel> TypeArguments { get; }
	internal SpecialType SpecialType { get; }
	internal TypeKind TypeKind { get; }
}