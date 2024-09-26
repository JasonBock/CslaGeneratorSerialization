using BenchmarkDotNet.Attributes;
using Csla;
using Csla.Configuration;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Microsoft.Extensions.DependencyInjection;

namespace CslaGeneratorSerialization.Performance;

[MemoryDiagnoser]
public class BusinessListBaseSerialization
{
	private readonly ISerializationFormatter generatorFormatter;
	private readonly ISerializationFormatter mobileFormatter;
	private readonly People generatorPeople;
	private readonly People mobilePeople;

	public BusinessListBaseSerialization()
	{
		var generatorService = new ServiceCollection();
		generatorService.AddCsla(o =>
			o.Serialization(so => so.UseSerializationFormatter<GeneratorFormatter>()));
		var generatorProvider = generatorService.BuildServiceProvider();

		var generatorApplicationContext = generatorProvider.GetService<ApplicationContext>()!;
		var generatorPeoplePortal = generatorProvider.GetRequiredService<IDataPortal<People>>();
		var generatorPeople = generatorPeoplePortal.Create();

		var generatorPersonPortal = generatorProvider.GetRequiredService<IChildDataPortal<Person>>();

		for (var i = 0; i < 100; i++)
		{
			var generatorPerson = generatorPersonPortal.CreateChild();
			generatorPerson.Name = "Jason";
			generatorPerson.Age = 53;
			generatorPeople.Add(generatorPerson);
		}

		(this.generatorPeople, this.generatorFormatter) = (generatorPeople, new GeneratorFormatter(generatorApplicationContext, new(generatorProvider)));

		var mobileService = new ServiceCollection();
		mobileService.AddCsla(o =>
			o.Serialization(so => so.UseSerializationFormatter<MobileFormatter>()));
		var mobileProvider = mobileService.BuildServiceProvider();

		var mobileApplicationContext = mobileProvider.GetService<ApplicationContext>()!;
		var mobilePeoplePortal = mobileProvider.GetRequiredService<IDataPortal<People>>();
		var mobilePeople = mobilePeoplePortal.Create();

		var mobilePersonPortal = mobileProvider.GetRequiredService<IChildDataPortal<Person>>();

		for (var i = 0; i < 100; i++)
		{
			var mobilePerson = mobilePersonPortal.CreateChild();
			mobilePerson.Name = "Jason";
			mobilePerson.Age = 53;
			mobilePeople.Add(mobilePerson);
		}

		(this.mobilePeople, this.mobileFormatter) = (mobilePeople, new MobileFormatter(mobileApplicationContext));
	}

	[Benchmark]
	public People RoundtripWithGenerator()
	{
		using var stream = new MemoryStream();
		this.generatorFormatter.Serialize(stream, this.generatorPeople);
		stream.Position = 0;
		return (People)this.generatorFormatter.Deserialize(stream);
	}

	[Benchmark(Baseline = true)]
	public People RoundtripWithMobile()
	{
		using var stream = new MemoryStream();
		this.mobileFormatter.Serialize(stream, this.mobilePeople);
		stream.Position = 0;
		return (People)this.mobileFormatter.Deserialize(stream);
	}
}