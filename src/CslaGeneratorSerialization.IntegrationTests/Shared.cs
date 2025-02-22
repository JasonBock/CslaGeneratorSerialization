using Csla.Configuration;
using CslaGeneratorSerialization.Extensions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

[assembly: Parallelizable(ParallelScope.Children)]

namespace CslaGeneratorSerialization.IntegrationTests;

#pragma warning disable CA1716 // Identifiers should not match keywords
public static class Shared
#pragma warning restore CA1716 // Identifiers should not match keywords
{
	private static readonly Lazy<ServiceProvider> lazyProvider =
		new(() =>
		{
			var services = new ServiceCollection();
			services.AddCsla(o =>
				o.Serialization(so => so.UseSerializationFormatter<GeneratorFormatter>()));
			services.AddCslaGeneratorSerialization();
			return services.BuildServiceProvider();
		}, true);

	// This service provider is the "default" with no customization.
	// This should reduce the amount of times where an IoC container is built.
	// If a test needs custom serialization, like the CustomTests.cs
	// file has, it should build a provider separately.
	public static ServiceProvider ServiceProvider => lazyProvider.Value;
}