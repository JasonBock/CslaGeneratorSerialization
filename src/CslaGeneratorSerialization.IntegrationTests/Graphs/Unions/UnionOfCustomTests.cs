using Csla;
using Csla.Configuration;
using CslaGeneratorSerialization.Extensions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.Graphs.Unions.UnionOfCustomTestsDomain;

#pragma warning disable CA1815 // Override equals and operator equals on value types
#pragma warning disable IDE0250 // Make struct 'readonly'
public sealed class CustomData
{
	public int Id { get; set; }
	public required string Name { get; set; }
}

public union Identifier(CustomData);

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

internal static class UnionOfCustomTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var services = new ServiceCollection();
		_ = services.AddCsla(o =>
			o.Serialization(so => so.UseSerializationFormatter<GeneratorFormatter>()));
		_ = services.AddCslaGeneratorSerialization(
			new CustomSerialization<CustomData>(
				(data, writer) =>
				{
					writer.Write(data.Id);
					writer.Write(data.Name);
				},
				(reader) => new() { Id = reader.ReadInt32(), Name = reader.ReadString() })!,
			new CustomSerialization<CustomData>(
				(data, writer) =>
				{
					writer.Write(data.Id);
					writer.Write(data.Name);
				},
				(reader) => new() { Id = reader.ReadInt32(), Name = reader.ReadString() })!);

		var provider = services.BuildServiceProvider();
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<Customer>>();
		var customer = await portal.CreateAsync();
		customer.Identifier = new CustomData { Id = 3, Name = "Jane" };

		using var stream = new MemoryStream();
		formatter.Serialize(stream, customer);
		stream.Position = 0;
		var newCustomer = (Customer)formatter.Deserialize(stream)!;

		using (Assert.EnterMultipleScope())
		{
			var value = (CustomData)newCustomer.Identifier.Value;
			Assert.That(value.Id, Is.EqualTo(3));
			Assert.That(value.Name, Is.EqualTo("Jane"));
		}
	}
}