using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.Graphs.Unions.MultipleUnionsInPropertiesTestsDomain;

#pragma warning disable CA1815 // Override equals and operator equals on value types
#pragma warning disable IDE0250 // Make struct 'readonly'
public union Identifier(int);
public union Description(string);

[GeneratorSerializable]
public partial class Customer
	: BusinessBase<Customer>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<Identifier> IdentifierProperty =
		Customer.RegisterProperty<Identifier>(nameof(Customer.Identifier));
	public Identifier Identifier
	{
		get => this.GetProperty(Customer.IdentifierProperty);
		set => this.SetProperty(Customer.IdentifierProperty, value);
	}

	public static readonly PropertyInfo<Description> DescriptionProperty =
		Customer.RegisterProperty<Description>(nameof(Customer.Description));
	public Description Description
	{
		get => this.GetProperty(Customer.DescriptionProperty);
		set => this.SetProperty(Customer.DescriptionProperty, value);
	}
}

internal static class MultipleUnionsInPropertiesTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var customerPortal = provider.GetRequiredService<IDataPortal<Customer>>();
		var customer = await customerPortal.CreateAsync();
		customer.Identifier = 42;
		customer.Description = "Joe";

		using var stream = new MemoryStream();
		formatter.Serialize(stream, customer);
		stream.Position = 0;
		var newCustomer = (Customer)formatter.Deserialize(stream)!;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(newCustomer.Identifier.Value, Is.EqualTo(42));
			Assert.That(newCustomer.Description.Value, Is.EqualTo("Joe"));
		}
	}
}