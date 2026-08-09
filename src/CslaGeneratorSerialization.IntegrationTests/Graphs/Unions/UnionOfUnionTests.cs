using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.Graphs.Unions.UnionOfUnionTestsDomain;

#pragma warning disable CA1815 // Override equals and operator equals on value types
#pragma warning disable IDE0250 // Make struct 'readonly'
public union Identifier(ChildIdentifier);

public union ChildIdentifier(int, string);

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

internal static class UnionOfUnionTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<Customer>>();
		var customer = await portal.CreateAsync();
		customer.Identifier = (ChildIdentifier)"hello";

		using var stream = new MemoryStream();
		formatter.Serialize(stream, customer);
		stream.Position = 0;
		var newCustomer = (Customer)formatter.Deserialize(stream)!;

		Assert.That(((ChildIdentifier)newCustomer.Identifier.Value).Value, Is.EqualTo("hello"));
	}
}