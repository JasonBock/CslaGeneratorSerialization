using Csla;
using Microsoft.Extensions.DependencyInjection;

namespace CslaGeneratorSerialization.IntegrationTests.Graphs.ReadOnlyListTestsDomain;

[GeneratorSerializable]
public partial class Experiments
	: ReadOnlyBase<Experiments>
{
	[Create]
	private void Create() =>
		this.Values = this.ApplicationContext.GetRequiredService<IChildDataPortal<Datum>>().CreateChild();

	public static readonly PropertyInfo<Datum> ValuesProperty =
		Experiments.RegisterProperty<Datum>(nameof(Experiments.Values));
	public Datum Values
	{
		get => this.ReadProperty(Experiments.ValuesProperty)!;
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
		Data.RegisterProperty<string>(nameof(Data.Value));
	public string Value
	{
		get => this.ReadProperty(Data.ValueProperty)!;
		private set => this.LoadProperty(Data.ValueProperty, value);
	}
}

public sealed class ListTests
{
	[Test]
	public async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<Experiments>>();
		var experiments = await portal.CreateAsync();

		using var stream = new MemoryStream();
		formatter.Serialize(stream, experiments);
		stream.Position = 0;
		var newData = (Experiments)formatter.Deserialize(stream)!;

		using (Assert.Multiple())
		{
			await Assert.That(newData.Values).HasCount(1);
			await Assert.That(newData.Values[0].Value).IsEqualTo("123");
		}
	}
}