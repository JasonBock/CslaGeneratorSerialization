using CslaGeneratorSerialization.Analysis.Extensions;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace CslaGeneratorSerialization.Analysis.Models;

internal sealed record SerializationModel
{
	internal static bool TryCreate(INamedTypeSymbol type, ModelContext modelContext, out SerializationModel? model)
	{
		if (type.HasErrors())
		{
			// This one will stop everything. There's no need to move on
			// if the given type is in error.
			model = null;
			return false;
		}

		if (type.IsGeneratorSerializable())
		{
			// If for some reason the marked type implements IGeneratorSerializable,
			// there's no reason to try and generate code.
			model = null;
			return false;
		}

		if (type.TypeKind == TypeKind.Class)
		{
			model = new SerializationModel(type, type.GetPropertyInfoDefinitions(), modelContext);
			return true;
		}
		else if (type.TypeKind == TypeKind.Interface)
		{
			model = new SerializationModel(type, [], modelContext);
			return true;
		}
		else
		{
			model = null;
			return false;
		}
	}

	private SerializationModel(INamedTypeSymbol businessObjectType, List<SerializationItemDefinition> serializationDefinitions, ModelContext modelContext)
	{
		this.IsCustomizable = businessObjectType.DerivesFrom("IGeneratorSerializableCustomization", "CslaGeneratorSerialization");
		this.RequiresDeserializationNotification = businessObjectType.DerivesFrom("ISerializationNotification", "Csla.Serialization.Mobile");
		this.ImplementsMetastate = businessObjectType.DerivesFrom("IMobileObjectMetastate", "Csla.Serialization.Mobile")!;

		var stereotypes = new Stereotypes(modelContext.SemanticModel.Compilation);
		this.BusinessObject = modelContext.CreateTypeReference(businessObjectType, stereotypes);
		this.Items = serializationDefinitions.Select(definition =>
			new SerializationItemModel(definition.PropertyInfoFieldName,
				modelContext.CreateTypeReference(definition.PropertyInfoContainingType, stereotypes), 
				modelContext.CreateTypeReference(definition.PropertyInfoDataType, stereotypes)))
			.OrderBy(_ => _.PropertyInfoDataType.BusinessObjectKind).ThenBy(_ => _.PropertyInfoFieldName).ToImmutableArray();
	}

	internal ITypeReferenceModel BusinessObject { get; }
	internal bool IsCustomizable { get; }
	public bool ImplementsMetastate { get; }
	internal EquatableArray<SerializationItemModel> Items { get; }
	public bool RequiresDeserializationNotification { get; }
}