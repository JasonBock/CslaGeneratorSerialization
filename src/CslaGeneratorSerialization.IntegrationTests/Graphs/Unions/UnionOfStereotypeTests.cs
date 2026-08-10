using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.Graphs.Unions.UnionOfStereotypeTestsDomain;

#pragma warning disable CA1815 // Override equals and operator equals on value types
#pragma warning disable IDE0250 // Make struct 'readonly'
public union Identifier(Data);

[GeneratorSerializable]
public sealed partial class Data
	: BusinessBase<Data>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<string> ContentsProperty =
		RegisterProperty<string>(_ => _.Contents);
	public string Contents
	{
		get => this.GetProperty(Data.ContentsProperty)!;
		set => this.SetProperty(Data.ContentsProperty, value);
	}
}

[GeneratorSerializable]
public partial class Customer
	: BusinessBase<Customer>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<Identifier> IdentifierProperty =
		Customer.RegisterProperty<Identifier>(_ => _.Identifier);
	public Identifier Identifier
	{
		get => this.GetProperty(Customer.IdentifierProperty);
		set => this.SetProperty(Customer.IdentifierProperty, value);
	}
}

internal static class UnionOfStereotypeTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var customerPortal = provider.GetRequiredService<IDataPortal<Customer>>();
		var customer = await customerPortal.CreateAsync();
		var dataPortal = provider.GetRequiredService<IChildDataPortal<Data>>();
		var data = await dataPortal.CreateChildAsync();
		data.Contents = "hello";
		customer.Identifier = data;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, customer);
		stream.Position = 0;
		var newCustomer = (Customer)formatter.Deserialize(stream)!;

		Assert.That(((Data)newCustomer.Identifier.Value).Contents, Is.EqualTo("hello"));
	}

	[Test]
	public static async Task RoundtripNullAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var customerPortal = provider.GetRequiredService<IDataPortal<Customer>>();
		var customer = await customerPortal.CreateAsync();
		customer.Identifier = null!;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, customer);
		stream.Position = 0;
		var newCustomer = (Customer)formatter.Deserialize(stream)!;

		Assert.That(newCustomer.Identifier.Value, Is.Null);
	}
}