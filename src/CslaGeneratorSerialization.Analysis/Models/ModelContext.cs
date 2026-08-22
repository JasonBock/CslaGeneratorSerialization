using CslaGeneratorSerialization.Analysis.Extensions;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace CslaGeneratorSerialization.Analysis.Models;

internal sealed class ModelContext
{
	private readonly Dictionary<ITypeSymbol, TypeReferenceModel> typeMap =
		[with(SymbolEqualityComparer.IncludeNullability)];

	internal ModelContext(SemanticModel semanticModel) =>
		this.SemanticModel = semanticModel;

	internal ITypeReferenceModel CreateTypeReference(ITypeSymbol typeSymbol, Stereotypes stereotypes)
	{
		if (this.typeMap.TryGetValue(typeSymbol, out var model))
		{
			return model;
		}
		else
		{
			var newModel = new TypeReferenceModel(typeSymbol, stereotypes, this);
			newModel.Initialize(typeSymbol, stereotypes, this);
			this.typeMap.Add(typeSymbol, newModel);
			return newModel;
		}
	}

	internal SemanticModel SemanticModel { get; }

	private sealed record TypeReferenceModel
		: ITypeReferenceModel
	{
		public TypeReferenceModel(ITypeSymbol type, Stereotypes stereotypes, ModelContext modelContext)
		{
			var compilation = modelContext.SemanticModel.Compilation;
			this.Name = type.Name;
			this.DeclaredAccessibility = type.DeclaredAccessibility;
			this.Namespace = type.GetNamespace();
			this.FullName = !string.IsNullOrWhiteSpace(this.Namespace) ?
				$"{this.Namespace}.{this.Name}" : this.Name;
			this.FullyQualifiedName = type.GetFullyQualifiedName(compilation);
			this.FullyQualifiedNameNoNullableAnnotation = type.GetFullyQualifiedName(compilation, false);

			this.IsValueType = type.IsValueType;
			this.IsNullable = type.NullableAnnotation == NullableAnnotation.Annotated;
			this.IsSealed = type.IsSealed;
			this.IsAbstract = type.IsAbstract;

			var generatorSerializableTypeSymbol = compilation.GetTypeByMetadataName("CslaGeneratorSerialization.GeneratorSerializableAttribute");
			this.ParticipatesInGeneratorSerialization = type.GetAttributes().Any(
				_ => _.AttributeClass!.Equals(generatorSerializableTypeSymbol, SymbolEqualityComparer.Default));

			(var kind, var targetType) = stereotypes.GetStereotype(type);

			(this.BusinessObjectKind, this.BusinessObjectTarget) = kind switch
			{
				StereotypeKind.BusinessListBase or StereotypeKind.BusinessDocumentBase or StereotypeKind.ReadOnlyListBase => (kind, modelContext.CreateTypeReference(targetType!, stereotypes)),
				_ => (kind, null)
			};

			this.SpecialType = type.SpecialType;
			this.TypeKind = type.TypeKind;

			if (type is IArrayTypeSymbol arrayTypeSymbol)
			{
				this.Array = new ArrayTypeReferenceModel(arrayTypeSymbol, modelContext, stereotypes);
				this.IsSupportedArray = arrayTypeSymbol.Rank == 1 &&
					(arrayTypeSymbol.ElementType.SpecialType == SpecialType.System_Byte || arrayTypeSymbol.ElementType.SpecialType == SpecialType.System_Char);
			}

			if (type is INamedTypeSymbol namedTypeSymbol)
			{
				this.TypeArguments = namedTypeSymbol.TypeArguments.Select(
					_ => modelContext.CreateTypeReference(_, stereotypes)).ToImmutableArray<ITypeReferenceModel>();

				if (namedTypeSymbol.EnumUnderlyingType is not null)
				{
					this.EnumUnderlyingType = modelContext.CreateTypeReference(namedTypeSymbol.EnumUnderlyingType, stereotypes);
				}
			}
			else
			{
				this.TypeArguments = [];
				this.UnionCaseTypes = [];
			}
		}

		public void Initialize(ITypeSymbol type, Stereotypes stereotypes, ModelContext modelContext)
		{
			if (type is INamedTypeSymbol namedTypeSymbol)
			{
				this.UnionCaseTypes = namedTypeSymbol.GetUnionCaseTypes(modelContext, stereotypes);
			}
		}

		public string GetClassName()
		{
			if (this.TypeArguments.IsDefaultOrEmpty)
			{
				return this.Name;
			}
			var typeArgs = string.Join(", ", this.TypeArguments.Select(_ => _.GetClassName()));
			return $"{this.Name}<{typeArgs}>";
		}

		public override string ToString() => this.FullyQualifiedName;

		public bool Equals(ITypeReferenceModel other) =>
			this.Equals(other as TypeReferenceModel);

		public bool Equals(TypeReferenceModel? other) =>
			this.FullyQualifiedName == other?.FullyQualifiedName;

		public override int GetHashCode() => this.FullyQualifiedName.GetHashCode();

		public ArrayTypeReferenceModel? Array { get; }
		public StereotypeKind BusinessObjectKind { get; }
		public ITypeReferenceModel? BusinessObjectTarget { get; }
		public ITypeReferenceModel? EnumUnderlyingType { get; }
		public string FullName { get; }
		public string FullyQualifiedName { get; }
		public string FullyQualifiedNameNoNullableAnnotation { get; }
		public bool IsAbstract { get; }
		public bool IsArray { get; }
		public bool IsNullable { get; }
		public bool IsSealed { get; }
		public bool IsSupportedArray { get; }
		public bool IsValueType { get; }
		public string Name { get; }
		public Accessibility DeclaredAccessibility { get; }
		public string? Namespace { get; }
		public bool ParticipatesInGeneratorSerialization { get; }
		public EquatableArray<ITypeReferenceModel> TypeArguments { get; }
		public SpecialType SpecialType { get; }
		public TypeKind TypeKind { get; }
		public EquatableArray<ITypeReferenceModel> UnionCaseTypes { get; private set; }
	}
}