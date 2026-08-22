using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.Graphs.Unions.RecursiveUnionInPropertiesTestsDomain;

#pragma warning disable CA1815 // Override equals and operator equals on value types
#pragma warning disable IDE0250 // Make struct 'readonly'
public union DataOne(int, string, DataTwo);
public union DataTwo(int, string, DataOne);

[GeneratorSerializable]
public partial class Customer
	: BusinessBase<Customer>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<DataOne> IdentifierProperty =
		Customer.RegisterProperty<DataOne>(nameof(Customer.Identifier));
	public DataOne Identifier
	{
		get => this.GetProperty(Customer.IdentifierProperty);
		set => this.SetProperty(Customer.IdentifierProperty, value);
	}
}

internal static class RecursiveUnionInPropertiesTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var customerPortal = provider.GetRequiredService<IDataPortal<Customer>>();
		var customer = await customerPortal.CreateAsync();
		customer.Identifier = new DataTwo(42);

		using var stream = new MemoryStream();
		formatter.Serialize(stream, customer);
		stream.Position = 0;
		var newCustomer = (Customer)formatter.Deserialize(stream)!;

		Assert.That(((DataTwo)newCustomer.Identifier.Value).Value, Is.EqualTo(42));
	}
}