using CslaGeneratorSerialization.Analysis.Models;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace CslaGeneratorSerialization.Analysis.Extensions;

internal static class INamedTypeSymbolExtensions
{
	extension(INamedTypeSymbol self)
	{
		// Hopefully at some point, I can remove this in favor of a UnionCaseTypes property:
		// 
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

		internal List<IFieldSymbol> GetPropertyInfoFields()
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
}