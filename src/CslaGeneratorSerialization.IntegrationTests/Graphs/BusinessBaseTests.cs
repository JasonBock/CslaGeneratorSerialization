using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.Graphs.BusinessBaseTestsDomain;

[GeneratorSerializable]
public partial class Customer
	: BusinessBase<Customer>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<uint> AgeProperty =
		Customer.RegisterProperty<uint>(_ => _.Age);
	public uint Age
	{
		get => this.GetProperty(Customer.AgeProperty);
		set => this.SetProperty(Customer.AgeProperty, value);
	}

	public static readonly PropertyInfo<string> NameProperty =
		Customer.RegisterProperty<string>(_ => _.Name);
	public string Name
	{
		get => this.GetProperty(Customer.NameProperty);
		set => this.SetProperty(Customer.NameProperty, value);
	}
}

public static class BusinessBaseTests
{
	[Test]
	public static void Roundtrip()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<Customer>>();
		var data = portal.Create();

		data.Name = "John";
		data.Age = 44;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (Customer)formatter.Deserialize(stream);

		Assert.Multiple(() =>
		{
			Assert.That(newData.Name, Is.EqualTo(data.Name));
			Assert.That(newData.Age, Is.EqualTo(data.Age));
		});
	}
}