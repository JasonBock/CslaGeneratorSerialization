using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.Graphs.Unions.UnionWithMultipleCasesTestsDomain;

#pragma warning disable CA1815 // Override equals and operator equals on value types
#pragma warning disable IDE0250 // Make struct 'readonly'
public union Identifier(string, int, Guid);

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

internal static class UnionWithMultipleCasesTests
{
	[Test]
	public static async Task RoundtripStringAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<Customer>>();
		var customer = await portal.CreateAsync();
		customer.Identifier = "hello";

		using var stream = new MemoryStream();
		formatter.Serialize(stream, customer);
		stream.Position = 0;
		var newCustomer = (Customer)formatter.Deserialize(stream)!;

		Assert.That(newCustomer.Identifier.Value, Is.EqualTo("hello"));
	}

	[Test]
	public static async Task RoundtripInt32Async()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<Customer>>();
		var customer = await portal.CreateAsync();
		customer.Identifier = 42;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, customer);
		stream.Position = 0;
		var newCustomer = (Customer)formatter.Deserialize(stream)!;

		Assert.That(newCustomer.Identifier.Value, Is.EqualTo(42));
	}

	[Test]
	public static async Task RoundtripGuidAsync()
	{
		var identifier = Guid.NewGuid();
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<Customer>>();
		var customer = await portal.CreateAsync();
		customer.Identifier = identifier;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, customer);
		stream.Position = 0;
		var newCustomer = (Customer)formatter.Deserialize(stream)!;

		Assert.That(newCustomer.Identifier.Value, Is.EqualTo(identifier));
	}
}