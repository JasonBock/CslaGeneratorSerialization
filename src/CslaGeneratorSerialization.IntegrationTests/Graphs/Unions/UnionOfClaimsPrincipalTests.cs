using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Security.Claims;

namespace CslaGeneratorSerialization.IntegrationTests.Graphs.Unions.UnionOfClaimsPrincipalTestsDomain;

#pragma warning disable CA1815 // Override equals and operator equals on value types
#pragma warning disable IDE0250 // Make struct 'readonly'
public union Identifier(ClaimsPrincipal);

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

internal static class UnionOfClaimsPrincipalTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<Customer>>();
		var customer = await portal.CreateAsync();
		customer.Identifier = new ClaimsPrincipal(
			new ClaimsIdentity(
			[
				new Claim(ClaimTypes.Role, "admin")
			], "fake auth"));

		using var stream = new MemoryStream();
		formatter.Serialize(stream, customer);
		stream.Position = 0;
		var newCustomer = (Customer)formatter.Deserialize(stream)!;

		using (Assert.EnterMultipleScope())
		{
			var identity = ((ClaimsPrincipal)newCustomer.Identifier.Value).Identities.Single();
			Assert.That(identity.AuthenticationType, Is.EqualTo("fake auth"));
			var claim = identity.Claims.Single();
			Assert.That(claim.Type, Is.EqualTo("http://schemas.microsoft.com/ws/2008/06/identity/claims/role"));
			Assert.That(claim.Value, Is.EqualTo("admin"));
		}
	}

	[Test]
	public static async Task RoundtripNullAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<Customer>>();
		var customer = await portal.CreateAsync();
		customer.Identifier = null!;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, customer);
		stream.Position = 0;
		var newCustomer = (Customer)formatter.Deserialize(stream)!;

		Assert.That(newCustomer.Identifier.Value, Is.Null);
	}
}