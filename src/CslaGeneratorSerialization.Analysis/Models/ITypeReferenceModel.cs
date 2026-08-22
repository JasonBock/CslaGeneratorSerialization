using Microsoft.CodeAnalysis;

namespace CslaGeneratorSerialization.Analysis.Models;

internal interface ITypeReferenceModel
	: IEquatable<ITypeReferenceModel>
{
	string GetClassName();

	ArrayTypeReferenceModel? Array { get; }
	StereotypeKind BusinessObjectKind { get; }
	ITypeReferenceModel? BusinessObjectTarget { get; }
	ITypeReferenceModel? EnumUnderlyingType { get; }
	string FullName { get; }
	string FullyQualifiedName { get; }
	string FullyQualifiedNameNoNullableAnnotation { get; }
	bool IsAbstract { get; }
	bool IsArray { get; }
	bool IsNullable { get; }
	bool IsSealed { get; }
	bool IsSupportedArray { get; }
	bool IsValueType { get; }
	string Name { get; }
	Accessibility DeclaredAccessibility { get; }
	string? Namespace { get; }
	bool ParticipatesInGeneratorSerialization { get; }
	EquatableArray<ITypeReferenceModel> TypeArguments { get; }
	SpecialType SpecialType { get; }
	TypeKind TypeKind { get; }
	EquatableArray<ITypeReferenceModel> UnionCaseTypes { get; }
}
