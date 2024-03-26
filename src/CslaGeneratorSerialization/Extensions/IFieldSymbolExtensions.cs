using Microsoft.CodeAnalysis;

namespace CslaGeneratorSerialization.Extensions;

internal static class IFieldSymbolExtensions
{
   internal static bool IsPropertyInfo(this IFieldSymbol self) => 
		self.Type.Name == "PropertyInfo" && self.Type.ContainingAssembly.Name == "Csla" &&
			self.Type is INamedTypeSymbol fieldType &&
			fieldType.TypeParameters.Length == 1;
}