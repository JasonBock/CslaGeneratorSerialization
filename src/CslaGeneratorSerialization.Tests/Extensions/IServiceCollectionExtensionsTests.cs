using Csla.Configuration;
using CslaGeneratorSerialization.Extensions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.Tests.Extensions;

public static class IServiceCollectionExtensionsTests
{
	[Test]
	public static void AddCslaGeneratorSerializationWithNoCustomizations()
	{
		var services = new ServiceCollection();
		services.AddCslaGeneratorSerialization();

		Assert.Multiple(() =>
		{
			Assert.That(services, Has.Count.EqualTo(2));

			var customSerializationResolverRegistration =
				services.Single(_ => _.ServiceType == typeof(CustomSerializationResolver));
			Assert.That(customSerializationResolverRegistration.Lifetime, Is.EqualTo(ServiceLifetime.Singleton));

			var mobileFormatterOptionsRegistration =
				services.Single(_ => _.ServiceType == typeof(MobileFormatterOptions));
			Assert.That(mobileFormatterOptionsRegistration.Lifetime, Is.EqualTo(ServiceLifetime.Singleton));
		});
	}

	[Test]
	public static void AddCslaGeneratorSerializationWithCustomizations()
	{
		var services = new ServiceCollection();
		services.AddCslaGeneratorSerialization(
			new CustomSerialization<string>(
				(data, writer) => { },
				(reader) => "a"));

		Assert.Multiple(() =>
		{
			Assert.That(services, Has.Count.EqualTo(3));

			var customSerializationResolverRegistration =
				services.Single(_ => _.ServiceType == typeof(CustomSerializationResolver));
			Assert.That(customSerializationResolverRegistration.Lifetime, Is.EqualTo(ServiceLifetime.Singleton));

			var mobileFormatterOptionsRegistration =
				services.Single(_ => _.ServiceType == typeof(MobileFormatterOptions));
			Assert.That(mobileFormatterOptionsRegistration.Lifetime, Is.EqualTo(ServiceLifetime.Singleton));
			var options = (MobileFormatterOptions)mobileFormatterOptionsRegistration.ImplementationInstance!;
			Assert.That(options.CustomSerializers, Has.Count.EqualTo(1));
			Assert.That(options.CustomSerializers[0].GetType(), Is.EqualTo(typeof(MobileTypeMap<string>)));

			var customSerializationOfStringRegistration =
				services.Single(_ => _.ServiceType == typeof(CustomSerialization<string>));
			Assert.That(customSerializationOfStringRegistration.Lifetime, Is.EqualTo(ServiceLifetime.Singleton));
		});
	}
}