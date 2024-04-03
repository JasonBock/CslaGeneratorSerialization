using CslaGeneratorSerialization.Extensions;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace CslaGeneratorSerialization.Models;

internal sealed record TypeReferenceModel
{
	internal TypeReferenceModel(ITypeSymbol type, Compilation compilation, Stereotypes stereotypes)
	{
		this.Name = type.Name;
		this.Namespace = type.GetNamespace();
		this.FullName = !string.IsNullOrWhiteSpace(this.Namespace) ?
			$"{this.Namespace}.{this.Name}" : this.Name;
		this.FullyQualifiedName = type.GetFullyQualifiedName(compilation);
		this.FullyQualifiedNameNoNullableAnnotation = type.GetFullyQualifiedName(compilation, false);

		this.IsValueType = type.IsValueType;
		this.IsNullable = type.NullableAnnotation == NullableAnnotation.Annotated;
		this.IsSealed = type.IsSealed;
		this.IsAbstract = type.IsAbstract;

		(var kind, var targetType) = stereotypes.GetStereotype(type, compilation);

		(this.BusinessObjectKind, this.BusinessObjectTarget) = kind switch
		{
			StereotypeKind.BusinessListBase => (kind, new TypeReferenceModel(targetType!, compilation, stereotypes)),
			_ => (kind, null)
		};
	
		this.SpecialType = type.SpecialType;
		this.TypeKind = type.TypeKind;

		if (type is IArrayTypeSymbol arrayTypeSymbol)
		{
			this.Array = new ArrayTypeReferenceModel(arrayTypeSymbol, compilation, stereotypes);
		}

		if (type is INamedTypeSymbol namedTypeSymbol)
		{
			this.TypeArguments = namedTypeSymbol.TypeArguments.Select(
				_ => new TypeReferenceModel(_, compilation, stereotypes)).ToImmutableArray();

			if (namedTypeSymbol.EnumUnderlyingType is not null)
			{
				this.EnumUnderlyingType = new TypeReferenceModel(namedTypeSymbol.EnumUnderlyingType, compilation, stereotypes);
			}
		}
		else
		{
			this.TypeArguments = [];
		}
	}

	internal ArrayTypeReferenceModel? Array { get; }
	internal StereotypeKind BusinessObjectKind { get; }
	internal TypeReferenceModel? BusinessObjectTarget { get; }
	internal TypeReferenceModel? EnumUnderlyingType { get; }
	internal string FullName { get; }
	internal string FullyQualifiedName { get; }
	internal string FullyQualifiedNameNoNullableAnnotation { get; }
	internal bool IsAbstract { get; }
	internal bool IsArray { get; }
	internal bool IsNullable { get; }
	internal bool IsSealed { get; }
	internal bool IsValueType { get; }
	internal string Name { get; }
	internal string? Namespace { get; }
	internal EquatableArray<TypeReferenceModel> TypeArguments { get; }
	internal SpecialType SpecialType { get; }
	internal TypeKind TypeKind { get; }
}