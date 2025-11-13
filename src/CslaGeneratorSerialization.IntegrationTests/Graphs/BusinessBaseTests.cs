using Csla;
using Microsoft.Extensions.DependencyInjection;

namespace CslaGeneratorSerialization.IntegrationTests.Graphs.BusinessBaseTestsDomain;

[GeneratorSerializable]
public partial class Customer
	: BusinessBase<Customer>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<uint> AgeProperty =
		Customer.RegisterProperty<uint>(nameof(Customer.Age));
	public uint Age
	{
		get => this.GetProperty(Customer.AgeProperty);
		set => this.SetProperty(Customer.AgeProperty, value);
	}

	public static readonly PropertyInfo<string> NameProperty =
		Customer.RegisterProperty<string>(nameof(Customer.Name));
	public string Name
	{
		get => this.GetProperty(Customer.NameProperty)!;
		set => this.SetProperty(Customer.NameProperty, value);
	}
}

public sealed class BusinessBaseTests
{
	[Test]
	public async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<Customer>>();
		var data = await portal.CreateAsync();

		data.Name = "John";
		data.Age = 44;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (Customer)formatter.Deserialize(stream)!;

		using (Assert.Multiple())
		{
			await Assert.That(newData.Name).IsEqualTo(data.Name);
			await Assert.That(newData.Age).IsEqualTo(data.Age);
		}
	}
}