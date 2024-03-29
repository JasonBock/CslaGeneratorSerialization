using Microsoft.CodeAnalysis;

namespace CslaGeneratorSerialization.Extensions;

internal static class INamedTypeSymbolExtensions
{
	internal static List<IFieldSymbol> GetPropertyInfoFields(this INamedTypeSymbol self)
	{
		var fields = new List<IFieldSymbol>();

		var targetType = self;

		while (targetType is not null)
		{
			fields.AddRange(targetType.GetMembers().OfType<IFieldSymbol>()
				.Where(_ => _.IsStatic && _.DeclaredAccessibility == Accessibility.Public && _.IsPropertyInfo()));

			targetType = targetType.BaseType;
		}

		return fields;
	}
}