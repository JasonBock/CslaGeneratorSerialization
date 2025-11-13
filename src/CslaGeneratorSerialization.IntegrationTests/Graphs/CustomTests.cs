using Csla;
using Csla.Configuration;
using Csla.Core;
using CslaGeneratorSerialization.Extensions;
using Microsoft.Extensions.DependencyInjection;

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
		RegisterProperty<CustomClassData>(nameof(Data.CustomClass));
	public CustomClassData CustomClass
	{
		get => this.GetProperty(Data.CustomClassProperty)!;
		set => this.SetProperty(Data.CustomClassProperty, value);
	}

	public static readonly PropertyInfo<CustomStructData> CustomStructProperty =
		RegisterProperty<CustomStructData>(nameof(Data.CustomStruct));
	public CustomStructData CustomStruct
	{
		get => this.GetProperty(Data.CustomStructProperty);
		set => this.SetProperty(Data.CustomStructProperty, value);
	}
}

public sealed class CustomTests
{
	[Test]
	public async Task RoundtripAsync()
	{
		var services = new ServiceCollection();
		services.AddCsla(o =>
			o.Serialization(so => so.UseSerializationFormatter<GeneratorFormatter>()));
		services.AddCslaGeneratorSerialization(
			new CustomSerialization<CustomClassData>(
				(data, writer) =>
				{
					writer.Write(data.Id);
					writer.Write(data.Name);
				},
				(reader) => new() { Id = reader.ReadInt32(), Name = reader.ReadString() }),
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
		var data = await portal.CreateAsync();

		data.CustomClass.Name = "Custom Class";
		data.CustomClass.Id = 1;
		data.CustomStruct = new CustomStructData { Id = 2, Name = "Custom Struct" };

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (Data)formatter.Deserialize(stream)!;

		using (Assert.Multiple())
		{
			await Assert.That(newData.CustomClass.Name).IsEqualTo("Custom Class");
			await Assert.That(newData.CustomClass.Id).IsEqualTo(1);
			await Assert.That(newData.CustomStruct.Name).IsEqualTo("Custom Struct");
			await Assert.That(newData.CustomStruct.Id).IsEqualTo(2);
		}
	}

	[Test]
	public async Task CloneAsync()
	{
		var services = new ServiceCollection();
		services.AddCsla(o =>
			o.Serialization(so => so.UseSerializationFormatter<GeneratorFormatter>()));
		services.AddCslaGeneratorSerialization(
			new CustomSerialization<CustomClassData>(
				(data, writer) =>
				{
					writer.Write(data.Id);
					writer.Write(data.Name);
				},
				(reader) => new() { Id = reader.ReadInt32(), Name = reader.ReadString() }),
			new CustomSerialization<CustomStructData>(
				(data, writer) =>
				{
					writer.Write(data.Id);
					writer.Write(data.Name);
				},
				(reader) => new() { Id = reader.ReadInt32(), Name = reader.ReadString() }));

		var provider = services.BuildServiceProvider();
		var context = provider.GetRequiredService<ApplicationContext>();
		var formatter = new GeneratorFormatter(context, new(provider));
		var portal = provider.GetRequiredService<IDataPortal<Data>>();
		var data = await portal.CreateAsync();

		data.CustomClass.Name = "Custom Class";
		data.CustomClass.Id = 1;
		data.CustomStruct = new CustomStructData { Id = 2, Name = "Custom Struct" };

		var cloner = new ObjectCloner(context);
		var newData = (Data)cloner.Clone(data);

		using (Assert.Multiple())
		{
			await Assert.That(newData.CustomClass.Name).IsEqualTo("Custom Class");
			await Assert.That(newData.CustomClass.Id).IsEqualTo(1);
			await Assert.That(newData.CustomStruct.Name).IsEqualTo("Custom Struct");
			await Assert.That(newData.CustomStruct.Id).IsEqualTo(2);
		}
	}

	[Test]
	public async Task NLevelUndoAsync()
	{
		var services = new ServiceCollection();
		services.AddCsla(o =>
			o.Serialization(so => so.UseSerializationFormatter<GeneratorFormatter>()));
		services.AddCslaGeneratorSerialization(
			new CustomSerialization<CustomClassData>(
				(data, writer) =>
				{
					writer.Write(data.Id);
					writer.Write(data.Name);
				},
				(reader) => new() { Id = reader.ReadInt32(), Name = reader.ReadString() }),
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
		var data = await portal.CreateAsync();

		data.CustomClass.Name = "Custom Class";
		data.CustomClass.Id = 1;
		data.CustomStruct = new CustomStructData { Id = 2, Name = "Custom Struct" };

		data.BeginEdit();
		data.CustomClass.Name = "Custom Class 2";
		data.CustomClass.Id = 2;
		data.CustomStruct = new CustomStructData { Id = 3, Name = "Custom Struct 2" };
		data.CancelEdit();

		using (Assert.Multiple())
		{
			await Assert.That(data.CustomClass.Name).IsEqualTo("Custom Class");
			await Assert.That(data.CustomClass.Id).IsEqualTo(1);
			await Assert.That(data.CustomStruct.Name).IsEqualTo("Custom Struct");
			await Assert.That(data.CustomStruct.Id).IsEqualTo(2);
		}
	}

	[Test]
	public async Task RoundtripWhenCustomizationIsNotConfiguredAsync()
	{
		var provider = Shared.ServiceProvider;

		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<Data>>();
		var data = await portal.CreateAsync();

		data.CustomClass.Name = "Custom Class";
		data.CustomClass.Id = 1;
		data.CustomStruct = new CustomStructData { Id = 2, Name = "Custom Struct" };

		using var stream = new MemoryStream();

		await Assert.That(() => formatter.Serialize(stream, data)).Throws<NotSupportedException>();
	}
}