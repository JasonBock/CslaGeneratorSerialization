using BenchmarkDotNet.Attributes;
using Csla;
using Csla.Configuration;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Microsoft.Extensions.DependencyInjection;

namespace CslaGeneratorSerialization.Performance;

[MemoryDiagnoser]
public class RoundtripSerialization
{
	private readonly ISerializationFormatter formatter;
	private readonly Person person;

	public RoundtripSerialization()
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

		(this.person, this.formatter) = (person, new GeneratorFormatter(applicationContext));
	}

	[Benchmark]
	public object Roundtrip()
	{
		using var stream = new MemoryStream();
		this.formatter.Serialize(stream, this.person);
		stream.Position = 0;
		return (Person)this.formatter.Deserialize(stream);
	}
}