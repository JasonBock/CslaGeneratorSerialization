using CslaGeneratorSerialization.Analysis.Extensions;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace CslaGeneratorSerialization.Analysis.Models;

internal sealed record SerializationModel
{
	internal static bool TryCreate(INamedTypeSymbol type, Compilation compilation, out SerializationModel? model)
	{
		if (type.HasErrors())
		{
			// This one will stop everything. There's no need to move on
			// if the given type is in error.
			model = null;
			return false;
		}

		if (type.TypeKind == TypeKind.Class)
		{
			model = new SerializationModel(type, type.GetPropertyInfoFields(), compilation);
			return true;
		}
		else if (type.TypeKind == TypeKind.Interface)
		{
			model = new SerializationModel(type, [], compilation);
			return true;
		}
		else
		{
			model = null;
			return false;
		}
	}

	private SerializationModel(INamedTypeSymbol businessObjectType, List<IFieldSymbol> propertyInfoFields, Compilation compilation)
	{
		this.IsCustomizable = businessObjectType.DerivesFrom(
			compilation.GetTypeByMetadataName(typeof(IGeneratorSerializableCustomization).FullName)!);

		var stereotypes = new Stereotypes(compilation);
		this.BusinessObject = new TypeReferenceModel(businessObjectType, compilation, stereotypes);
		this.Items = propertyInfoFields.Select(_ =>
		{
			var fieldContainingType = _.ContainingType;
			var fieldType = (INamedTypeSymbol)_.Type;
			var propertyInfoType = fieldType.TypeArguments[0];
			return new SerializationItemModel(_.Name,
				new TypeReferenceModel(fieldContainingType, compilation, stereotypes), new TypeReferenceModel(propertyInfoType, compilation, stereotypes));
		}).OrderBy(_ => _.PropertyInfoDataType.BusinessObjectKind).ThenBy(_ => _.PropertyInfoFieldName).ToImmutableArray();
	}

	internal TypeReferenceModel BusinessObject { get; }
	internal bool IsCustomizable { get; }
	internal EquatableArray<SerializationItemModel> Items { get; }
}