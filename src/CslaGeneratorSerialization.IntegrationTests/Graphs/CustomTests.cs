using Csla;
using Csla.Configuration;
using CslaGeneratorSerialization.Extensions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.Graphs.CustomTestsDomain;

public sealed class CustomClassData
{
	public int Id { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public string Name { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

#pragma warning disable CA1815 // Override equals and operator equals on value types
public struct CustomStructData
#pragma warning restore CA1815 // Override equals and operator equals on value types
{
	public int Id { get; set; }
	public string Name { get; set; }
}

[GeneratorSerializable]
public sealed partial class Data
	: BusinessBase<Data>
{
	[Create]
	private void Create() =>
		this.CustomClass = new();

	public static readonly PropertyInfo<CustomClassData> CustomClassProperty =
		RegisterProperty<CustomClassData>(_ => _.CustomClass);
	public CustomClassData CustomClass
	{
		get => this.GetProperty(Data.CustomClassProperty);
		set => this.SetProperty(Data.CustomClassProperty, value);
	}

	public static readonly PropertyInfo<CustomStructData> CustomStructProperty =
		RegisterProperty<CustomStructData>(_ => _.CustomStruct);
	public CustomStructData CustomStruct
	{
		get => this.GetProperty(Data.CustomStructProperty);
		set => this.SetProperty(Data.CustomStructProperty, value);
	}
}

public static class CustomTests
{
	[Test]
	public static void Roundtrip()
	{
		var services = new ServiceCollection();
		services.AddCsla(o =>
			o.Serialization(so => so.SerializationFormatter(typeof(GeneratorFormatter))));
		services.AddCslaGeneratorSerialization();
		services.AddSingleton(
			new CustomSerialization<CustomClassData>(
				(data, writer) =>
				{
					writer.Write(data.Id);
					writer.Write(data.Name);
				},
				(reader) => new() { Id = reader.ReadInt32(), Name = reader.ReadString() }));
		services.AddSingleton(
			new CustomSerialization<CustomStructData>(
				(data, writer) =>
				{
					writer.Write(data.Id);
					writer.Write(data.Name);
				},
				(reader) => new() { Id = reader.ReadInt32(), Name = reader.ReadString() }));

		var provider = services.BuildServiceProvider();

		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<Data>>();
		var data = portal.Create();

		data.CustomClass.Name = "Custom Class";
		data.CustomClass.Id = 1;
		data.CustomStruct = new CustomStructData { Id = 2, Name = "Custom Struct" };

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (Data)formatter.Deserialize(stream);

		Assert.Multiple(() =>
		{
			Assert.That(newData.CustomClass.Name, Is.EqualTo("Custom Class"));
			Assert.That(newData.CustomClass.Id, Is.EqualTo(1));
			Assert.That(newData.CustomStruct.Name, Is.EqualTo("Custom Struct"));
			Assert.That(newData.CustomStruct.Id, Is.EqualTo(2));
		});
	}

	[Test]
	public static void RoundtripWhenCustomizationIsNotConfigured()
	{
		var provider = Shared.ServiceProvider;

		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<Data>>();
		var data = portal.Create();

		data.CustomClass.Name = "Custom Class";
		data.CustomClass.Id = 1;
		data.CustomStruct = new CustomStructData { Id = 2, Name = "Custom Struct" };

		using var stream = new MemoryStream();

		Assert.That(() => formatter.Serialize(stream, data), Throws.TypeOf<NotSupportedException>());
	}
}