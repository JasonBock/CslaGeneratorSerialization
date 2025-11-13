using Csla.Configuration;
using CslaGeneratorSerialization.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace CslaGeneratorSerialization.Tests.Extensions;

public sealed class IServiceCollectionExtensionsTests
{
	[Test]
	public async Task AddCslaGeneratorSerializationWithNoCustomizationsAsync()
	{
		var services = new ServiceCollection();
		services.AddCslaGeneratorSerialization();

		using (Assert.Multiple())
		{
			await Assert.That(services).HasCount(2);

			var customSerializationResolverRegistration =
				services.Single(_ => _.ServiceType == typeof(CustomSerializationResolver));
			await Assert.That(customSerializationResolverRegistration.Lifetime).EqualTo(ServiceLifetime.Singleton);

			var mobileFormatterOptionsRegistration =
				services.Single(_ => _.ServiceType == typeof(MobileFormatterOptions));
			await Assert.That(mobileFormatterOptionsRegistration.Lifetime).EqualTo(ServiceLifetime.Singleton);
		}
	}

	[Test]
	public async Task AddCslaGeneratorSerializationWithCustomizationsAsync()
	{
		var services = new ServiceCollection();
		services.AddCslaGeneratorSerialization(
			new CustomSerialization<string>(
				(data, writer) => { },
				(reader) => "a")!);

		using (Assert.Multiple())
		{
			await Assert.That(services).HasCount(3);

			var customSerializationResolverRegistration =
				services.Single(_ => _.ServiceType == typeof(CustomSerializationResolver));
			await Assert.That(customSerializationResolverRegistration.Lifetime).EqualTo(ServiceLifetime.Singleton);

			var mobileFormatterOptionsRegistration =
				services.Single(_ => _.ServiceType == typeof(MobileFormatterOptions));
			await Assert.That(mobileFormatterOptionsRegistration.Lifetime).EqualTo(ServiceLifetime.Singleton);
			var options = (MobileFormatterOptions)mobileFormatterOptionsRegistration.ImplementationInstance!;
			await Assert.That(options.CustomSerializers).HasCount(1);
			await Assert.That(options.CustomSerializers[0].GetType()).EqualTo(typeof(MobileTypeMap<string>));

			var customSerializationOfStringRegistration =
				services.Single(_ => _.ServiceType == typeof(CustomSerialization<string>));
			await Assert.That(customSerializationOfStringRegistration.Lifetime).EqualTo(ServiceLifetime.Singleton);
		}
	}
}