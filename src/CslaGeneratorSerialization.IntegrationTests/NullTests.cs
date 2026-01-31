using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests;

internal static class NullTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));

		using var stream = new MemoryStream();
		formatter.Serialize(stream, null);
		stream.Position = 0;
		var newData = formatter.Deserialize(stream)!;

		Assert.That(newData, Is.Null);
	}
}