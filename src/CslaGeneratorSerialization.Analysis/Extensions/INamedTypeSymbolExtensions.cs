using Microsoft.CodeAnalysis;

namespace CslaGeneratorSerialization.Analysis.Extensions;

internal static class INamedTypeSymbolExtensions
{
	extension(INamedTypeSymbol self)
	{
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