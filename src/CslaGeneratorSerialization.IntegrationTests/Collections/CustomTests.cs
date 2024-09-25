using Csla;
using Csla.Configuration;
using CslaGeneratorSerialization.Extensions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.Collections.CustomTestsDomain;

[GeneratorSerializable]
public sealed partial class Data
	: BusinessBase<Data>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<int[]> ValueProperty =
		RegisterProperty<int[]>(_ => _.Value);
#pragma warning disable CA1819 // Properties should not return arrays
	public int[] Value
#pragma warning restore CA1819 // Properties should not return arrays
	{
		get => this.GetProperty(Data.ValueProperty);
		set => this.SetProperty(Data.ValueProperty, value);
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
			new CustomSerialization<int[]>(
				(data, writer) =>
				{
					writer.Write(data.Length);

					foreach (var item in data)
					{
						writer.Write(item);
					}
				},
				(reader) =>
				{
					var data = new int[reader.ReadInt32()];

					for (var i = 0; i < data.Length; i++)
					{
						data[i] = reader.ReadInt32();
					}

					return data;
				}));

		var provider = services.BuildServiceProvider();

		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<Data>>();
		var data = portal.Create();

		data.Value = [3, 7, 4, 2];

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (Data)formatter.Deserialize(stream);

		Assert.That(newData.Value, Is.EquivalentTo(new[] { 3, 7, 4, 2 }));
	}

	[Test]
	public static void RoundtripWhenCustomizationIsNotConfigured()
	{
		var provider = Shared.ServiceProvider;

		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<Data>>();
		var data = portal.Create();

		data.Value = [3, 7, 4, 2];

		using var stream = new MemoryStream();

		Assert.That(() => formatter.Serialize(stream, data), Throws.TypeOf<NotSupportedException>());
	}
}