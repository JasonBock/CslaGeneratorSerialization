using Csla.Serialization.Mobile;
using Microsoft.CodeAnalysis;

namespace CslaGeneratorSerialization.Extensions;

internal static class INamedTypeSymbolExtensions
{
   internal static bool IsMobileObject(this INamedTypeSymbol self) => 
		(self.Name == nameof(IMobileObject) && self.ContainingAssembly.Name == "Csla") || 
			self.AllInterfaces.Any(_ => _.IsMobileObject());

	internal static bool IsGeneratorSerializable(this INamedTypeSymbol self) =>
		(self.Name == nameof(IGeneratorSerializable) && self.ContainingAssembly.Name == "CslaGeneratorSerializable") || 
			self.AllInterfaces.Any(_ => _.IsGeneratorSerializable());
}