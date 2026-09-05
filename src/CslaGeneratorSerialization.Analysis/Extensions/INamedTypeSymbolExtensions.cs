using CslaGeneratorSerialization.Analysis.Models;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace CslaGeneratorSerialization.Analysis.Extensions;

internal static class INamedTypeSymbolExtensions
{
	extension(INamedTypeSymbol self)
	{
		// TODO:
		// Hopefully at some point, I can remove this in favor of a UnionCaseTypes property.
		internal ImmutableArray<ITypeReferenceModel> GetUnionCaseTypes(ModelContext modelContext, Stereotypes stereotypes)
		{
			// First, check if the type has [Union] on it.
			if (self.IsUnion)
			{
				// Now look for all public constructors that have one parameter, those are the types.
				var constructors = self.Constructors.Where(
					constructor => constructor.Parameters.Length == 1).ToArray();

				if (constructors.Length > 0)
				{
					return [.. constructors.Select(
						constructor => modelContext.CreateTypeReference(constructor.Parameters[0].Type, stereotypes))];
				}
				else
				{
					return [];
				}
			}
			else
			{
				return [];
			}
		}

		internal List<SerializationItemDefinition> GetPropertyInfoDefinitions()
		{
			var definitions = new List<SerializationItemDefinition>();

			var targetType = self;

			while (targetType is not null)
			{
				definitions.AddRange(targetType.GetMembers().OfType<IFieldSymbol>()
					.Where(field => field.IsStatic && field.DeclaredAccessibility == Accessibility.Public && field.IsPropertyInfo())
					.Select(field =>
					{
						var fieldContainingType = field.ContainingType;
						var fieldType = (INamedTypeSymbol)field.Type;
						var propertyInfoType = fieldType.TypeArguments[0]!;
						return new SerializationItemDefinition(field.Name, fieldContainingType, propertyInfoType);
					}));

				// We also need to look for [CslaImplementProperties]
				// and include any public partial properties that 
				// are not attributed with [CslaIgnoreProperty]
				if (targetType.GetAttributes().Any(attribute =>
					attribute.AttributeClass?.Name == "CslaImplementPropertiesAttribute" &&
					attribute.AttributeClass?.GetNamespace() == "Csla"))
				{
					definitions.AddRange(targetType.GetMembers().OfType<IPropertySymbol>()
						.Where(property => property.IsPartialDefinition &&
							property.DeclaredAccessibility == Accessibility.Public &&
							!property.GetAttributes().Any(attribute =>
								attribute.AttributeClass?.Name == "CslaIgnorePropertyAttribute" &&
								attribute.AttributeClass?.GetNamespace() == "Csla"))
						.Select(property =>
							new SerializationItemDefinition($"{property.Name}Property", property.ContainingType, property.Type)));
				}

				targetType = targetType.BaseType;
			}

			return definitions;
		}
	}
}