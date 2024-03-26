using Csla;
using Csla.Configuration;
using Csla.Serialization.Mobile;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.Tests;

public static class ExperimentsTests
{
	[Test]
	public static void RoundTripUsingMobileFormatter()
	{
		var services = new ServiceCollection();
		services.AddCsla();
		var provider = services.BuildServiceProvider();
		var applicationContext = provider.GetService<ApplicationContext>();
		var portal = provider.GetRequiredService<IDataPortal<Person>>();
		var person = portal.Create();

		person.Name = "Jason";
		person.Age = 53;

		using var stream = new MemoryStream();
		var formatter = new MobileFormatter(applicationContext);
		formatter.Serialize(stream, person);
		stream.Position = 0;
		var deserializedPerson = (Person)formatter.Deserialize(stream);

		Assert.Multiple(() =>
		{
			Assert.That(deserializedPerson.Age, Is.EqualTo(person.Age));
			Assert.That(deserializedPerson.Id, Is.EqualTo(person.Id));
			Assert.That(deserializedPerson.Name, Is.EqualTo(person.Name));
			Assert.That(deserializedPerson.IsDirty, Is.EqualTo(person.IsDirty));
		});
	}

	[Test]
	public static void RoundTripUsingGeneratorFormatter()
	{
		var services = new ServiceCollection();
		services.AddCsla(o => 
			o.Serialization(so => so.SerializationFormatter(typeof(GeneratorFormatter))));
		var provider = services.BuildServiceProvider();
		var applicationContext = provider.GetService<ApplicationContext>()!;
		var portal = provider.GetRequiredService<IDataPortal<Person>>();
		var person = portal.Create();

		person.Name = "Jason";
		person.Age = 53;

		using var stream = new MemoryStream();
		var formatter = new GeneratorFormatter(applicationContext);
		formatter.Serialize(stream, person);
		stream.Position = 0;
		var deserializedPerson = (Person)formatter.Deserialize(stream);

		Assert.Multiple(() =>
		{
			Assert.That(deserializedPerson.Age, Is.EqualTo(person.Age));
			Assert.That(deserializedPerson.Id, Is.EqualTo(person.Id));
			Assert.That(deserializedPerson.Name, Is.EqualTo(person.Name));
			Assert.That(deserializedPerson.IsDirty, Is.EqualTo(person.IsDirty));
		});
	}
}