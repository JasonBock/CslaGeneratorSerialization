using Csla.Configuration;
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
			return services.BuildServiceProvider();
		}, true);

	public static ServiceProvider ServiceProvider => lazyProvider.Value;
}