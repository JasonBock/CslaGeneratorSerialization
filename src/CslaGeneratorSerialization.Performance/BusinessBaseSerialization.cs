using BenchmarkDotNet.Attributes;
using Csla;
using Csla.Configuration;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Microsoft.Extensions.DependencyInjection;

namespace CslaGeneratorSerialization.Performance;

[MemoryDiagnoser]
public class BusinessBaseSerialization
{
	private readonly ISerializationFormatter generatorFormatter;
	private readonly ISerializationFormatter mobileFormatter;
	private readonly Person generatorPerson;
	private readonly Person mobilePerson;

	public BusinessBaseSerialization()
	{
		var generatorService = new ServiceCollection();
		generatorService.AddCsla(o =>
			o.Serialization(so => so.SerializationFormatter(typeof(GeneratorFormatter))));
		var generatorProvider = generatorService.BuildServiceProvider();
		var generatorApplicationContext = generatorProvider.GetService<ApplicationContext>()!;
		var generatorPortal = generatorProvider.GetRequiredService<IDataPortal<Person>>();
		var generatorPerson = generatorPortal.Create();

		generatorPerson.Name = "Jason";
		generatorPerson.Age = 53;

		(this.generatorPerson, this.generatorFormatter) = (generatorPerson, new GeneratorFormatter(generatorApplicationContext, new(generatorProvider)));

		var mobileService = new ServiceCollection();
		mobileService.AddCsla(o =>
			o.Serialization(so => so.SerializationFormatter(typeof(MobileFormatter))));
		var mobileProvider = mobileService.BuildServiceProvider();

		var mobileApplicationContext = mobileProvider.GetService<ApplicationContext>()!;
		var mobilePortal = mobileProvider.GetRequiredService<IDataPortal<Person>>();
		var mobilePerson = mobilePortal.Create();

		mobilePerson.Name = "Jason";
		mobilePerson.Age = 53;

		(this.mobilePerson, this.mobileFormatter) = (mobilePerson, new MobileFormatter(mobileApplicationContext));
	}

	[Benchmark]
	public Person RoundtripWithGenerator()
	{
		using var stream = new MemoryStream();
		this.generatorFormatter.Serialize(stream, this.generatorPerson);
		stream.Position = 0;
		return (Person)this.generatorFormatter.Deserialize(stream);
	}

	[Benchmark(Baseline = true)]
	public Person RoundtripWithMobile()
	{
		using var stream = new MemoryStream();
		this.mobileFormatter.Serialize(stream, this.mobilePerson);
		stream.Position = 0;
		return (Person)this.mobileFormatter.Deserialize(stream);
	}
}