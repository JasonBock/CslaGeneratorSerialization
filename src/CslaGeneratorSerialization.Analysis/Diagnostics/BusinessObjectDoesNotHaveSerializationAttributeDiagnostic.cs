using CslaGeneratorSerialization.Analysis.Descriptors;
using Microsoft.CodeAnalysis;

namespace CslaGeneratorSerialization.Analysis.Diagnostics;

internal static class BusinessObjectDoesNotHaveSerializationAttributeDiagnostic
{
	internal static Diagnostic Create(SyntaxNode node, ITypeSymbol type) =>
		Diagnostic.Create(BusinessObjectDoesNotHaveSerializationAttributeDescriptor.Create(),
			node.GetLocation(), type.Name);
}