using Csla;
using Csla.Configuration;
using CslaGeneratorSerialization.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace CslaGeneratorSerialization.IntegrationTests.Collections.CustomTestsDomain;

[GeneratorSerializable]
public sealed partial class Data
	: BusinessBase<Data>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<int[]> ValueProperty =
		RegisterProperty<int[]>(nameof(Data.Value));
#pragma warning disable CA1819 // Properties should not return arrays
	public int[] Value
#pragma warning restore CA1819 // Properties should not return arrays
	{
		get => this.GetProperty(Data.ValueProperty)!;
		set => this.SetProperty(Data.ValueProperty, value);
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
		var data = await portal.CreateAsync();

		data.Value = [3, 7, 4, 2];

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (Data)formatter.Deserialize(stream)!;

		await Assert.That(newData.Value).IsEquivalentTo([3, 7, 4, 2]);
	}

	[Test]
	public async Task RoundtripWhenCustomizationIsNotConfiguredAsync()
	{
		var provider = Shared.ServiceProvider;

		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<Data>>();
		var data = await portal.CreateAsync();

		data.Value = [3, 7, 4, 2];

		using var stream = new MemoryStream();

		await Assert.That(() => formatter.Serialize(stream, data)).Throws<NotSupportedException>();
	}
}