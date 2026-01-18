using CslaGeneratorSerialization.Analysis.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
			model = new SerializationModel(type, type.GetPropertyInfoFields(), compilation)
			{
				IsNullableEnabled = IsNullable(type, compilation)
			};

			return true;
		}
		else if (type.TypeKind == TypeKind.Interface)
		{
			model = new SerializationModel(type, [], compilation)
			{
				IsNullableEnabled = IsNullable(type, compilation)
			};
			return true;
		}
		else
		{
			model = null;
			return false;
		}
	}

	private static bool IsNullable(INamedTypeSymbol symbol, Compilation compilation)
	{
		// 1. Get the syntax reference for the symbol
		var syntaxRef = symbol.DeclaringSyntaxReferences.FirstOrDefault();
		if (syntaxRef != null)
		{
			// 2. Get the semantic model for that specific syntax tree
			var model = compilation.GetSemanticModel(syntaxRef.SyntaxTree);

			// 3. Get the NullableContext at the symbol's position
			var context = model.GetNullableContext(syntaxRef.Span.Start);

			// 4. Check if nullable annotations and warnings are enabled
			var annotationsEnabled = context.HasFlag(NullableContext.AnnotationsEnabled);
			var warningsEnabled = context.HasFlag(NullableContext.WarningsEnabled);

			return annotationsEnabled && warningsEnabled;
		}
		return false;
	}

	private SerializationModel(INamedTypeSymbol businessObjectType, List<IFieldSymbol> propertyInfoFields, Compilation compilation)
	{
		this.IsCustomizable = businessObjectType.DerivesFrom("IGeneratorSerializableCustomization", "CslaGeneratorSerialization");
		this.ImplementsMetastate = businessObjectType.DerivesFrom("IMobileObjectMetastate", "Csla.Serialization.Mobile")!;

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
	public bool ImplementsMetastate { get; }
	internal EquatableArray<SerializationItemModel> Items { get; }
	public bool IsNullableEnabled { get; init; }
}