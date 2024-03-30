﻿using CslaGeneratorSerialization.Extensions;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace CslaGeneratorSerialization.Models;

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

		var propertyInfoFields = type.GetPropertyInfoFields();

		if (propertyInfoFields.Count > 0)
		{
			model = new SerializationModel(type, propertyInfoFields, compilation);
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
		this.BusinessObject = new TypeReferenceModel(businessObjectType, compilation);
		this.Items = propertyInfoFields.Select(_ =>
		{
			var fieldContainingType = _.ContainingType;
			var fieldType = (INamedTypeSymbol)_.Type;
			var propertyInfoType = fieldType.TypeArguments[0];
			return new SerializationItemModel(_.Name,
				new TypeReferenceModel(fieldContainingType, compilation), new TypeReferenceModel(propertyInfoType, compilation));
		}).OrderBy(_ => _.PropertyInfoDataType.IsSerializable).ThenBy(_ => _.PropertyInfoFieldName).ToImmutableArray();
	}

	internal TypeReferenceModel BusinessObject { get; }
	internal EquatableArray<SerializationItemModel> Items { get; }
}