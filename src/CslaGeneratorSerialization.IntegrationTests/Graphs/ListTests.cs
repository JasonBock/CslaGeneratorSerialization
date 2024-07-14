using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.Graphs.ListTestsDomain;

[GeneratorSerializable]
public partial class Experiments
	: BusinessBase<Experiments>
{
	[Create]
	private void Create() =>
		this.Values = this.ApplicationContext.GetRequiredService<IChildDataPortal<Datum>>().CreateChild();

	public static readonly PropertyInfo<Datum> ValuesProperty =
		Experiments.RegisterProperty<Datum>(_ => _.Values);
	public Datum Values
	{
		get => this.GetProperty(Experiments.ValuesProperty);
		private set => this.SetProperty(Experiments.ValuesProperty, value);
	}
}

[GeneratorSerializable]
public partial class Datum
	: BusinessListBase<Datum, Data>
{
	[CreateChild]
	private void CreateChild() { }
}

[GeneratorSerializable]
public partial class Data
	: BusinessBase<Data>
{
	[CreateChild]
	private void CreateChild() { }

	public static readonly PropertyInfo<string> ValueProperty =
		Data.RegisterProperty<string>(_ => _.Value);
	public string Value
	{
		get => this.GetProperty(Data.ValueProperty);
		set => this.SetProperty(Data.ValueProperty, value);
	}
}

public static class ListTests
{
	[Test]
	public static void Roundtrip()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<Experiments>>();
		var dataPortal = provider.GetRequiredService<IChildDataPortal<Data>>();
		var experiments = portal.Create();

		for (var i = 0; i < 3; i++)
		{
			var data = dataPortal.CreateChild();
			data.Value = $"ABC_{i}";
			experiments.Values.Add(data);
		}

		using var stream = new MemoryStream();
		formatter.Serialize(stream, experiments);
		stream.Position = 0;
		var newData = (Experiments)formatter.Deserialize(stream);

		Assert.That(newData.Values, Has.Count.EqualTo(3));
	}
}