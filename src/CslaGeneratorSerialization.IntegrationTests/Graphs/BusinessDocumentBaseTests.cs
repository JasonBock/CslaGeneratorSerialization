using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.Graphs.BusinessDocumentBaseTestsDomain;

[GeneratorSerializable]
public partial class Customer
	: BusinessDocumentBase<Customer, Address>
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

[GeneratorSerializable]
public partial class Address
	: BusinessBase<Address>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<string> CityProperty =
		Address.RegisterProperty<string>(nameof(Address.City));
	public string City
	{
		get => this.GetProperty(Address.CityProperty)!;
		set => this.SetProperty(Address.CityProperty, value);
	}
}

internal static class BusinessDocumentBaseTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<Customer>>();
		var data = await portal.CreateAsync();

		data.Name = "John";
		data.Age = 44;
		var address1 = await data.AddNewAsync();
		address1.City = "City1";
		var address2 = await data.AddNewAsync();
		address2.City = "City2";

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (Customer)formatter.Deserialize(stream)!;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(newData.Name, Is.EqualTo(data.Name));
			Assert.That(newData.Age, Is.EqualTo(data.Age));
			Assert.That(newData, Has.Count.EqualTo(2));
			Assert.That(newData.Any(address => address.City == "City1"), Is.True);
			Assert.That(newData.Any(address => address.City == "City2"), Is.True);
		}
	}
}