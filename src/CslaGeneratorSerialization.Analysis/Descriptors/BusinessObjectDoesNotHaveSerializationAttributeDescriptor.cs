using CslaGeneratorSerialization.Analysis.Diagnostics;
using Microsoft.CodeAnalysis;

namespace CslaGeneratorSerialization.Analysis.Descriptors;

internal static class BusinessObjectDoesNotHaveSerializationAttributeDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(Id, Title,
			Message,
			DiagnosticConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				Id, Title));

	internal const string Id = "CGSA1";
	internal const string Message = "The type {0} is not marked with GeneratorSerializableAttribute";
	internal const string Title = "Business Object Does Not Have GeneratorSerializableAttribute";
}