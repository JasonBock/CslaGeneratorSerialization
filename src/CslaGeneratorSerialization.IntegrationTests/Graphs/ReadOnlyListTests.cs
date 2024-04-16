using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.Graphs.ReadOnlyListTestsDomain;

[GeneratorSerializable]
public partial class Experiments
	: ReadOnlyBase<Experiments>
{
	[Create]
	private void Create() =>
		this.Values = this.ApplicationContext.GetRequiredService<IChildDataPortal<Datum>>().CreateChild();

	public static readonly PropertyInfo<Datum> ValuesProperty =
		Experiments.RegisterProperty<Datum>(_ => _.Values);
	public Datum Values
	{
		get => this.ReadProperty(Experiments.ValuesProperty);
		private set => this.LoadProperty(Experiments.ValuesProperty, value);
	}
}

[GeneratorSerializable]
public partial class Datum
	: ReadOnlyListBase<Datum, Data>
{
	[CreateChild]
	private void CreateChild()
	{
		using (this.LoadListMode)
		{
			this.Add(this.ApplicationContext.GetRequiredService<IChildDataPortal<Data>>().CreateChild());
		}
	}
}

[GeneratorSerializable]
public partial class Data
	: ReadOnlyBase<Data>
{
	[CreateChild]
	private void CreateChild() =>
		this.Value = "123";

	public static readonly PropertyInfo<string> ValueProperty =
		Data.RegisterProperty<string>(_ => _.Value);
	public string Value
	{
		get => this.ReadProperty(Data.ValueProperty);
		private set => this.LoadProperty(Data.ValueProperty, value);
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
		var experiments = portal.Create();

		using var stream = new MemoryStream();
		formatter.Serialize(stream, experiments);
		stream.Position = 0;
		var newData = (Experiments)formatter.Deserialize(stream);

		Assert.Multiple(() =>
		{
			Assert.That(newData.Values, Has.Count.EqualTo(1));
			Assert.That(newData.Values[0].Value, Is.EqualTo("123"));
		});
	}
}