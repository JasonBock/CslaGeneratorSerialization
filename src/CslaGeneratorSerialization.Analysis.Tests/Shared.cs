using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace CslaGeneratorSerialization.Analysis.Tests;

internal static class Shared
{
	internal static Lazy<ImmutableArray<PortableExecutableReference>> References { get; } =
		new Lazy<ImmutableArray<PortableExecutableReference>>(() =>
			[.. AppDomain.CurrentDomain.GetAssemblies()
				.Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
				.Select(_ => MetadataReference.CreateFromFile(_.Location))]);
}