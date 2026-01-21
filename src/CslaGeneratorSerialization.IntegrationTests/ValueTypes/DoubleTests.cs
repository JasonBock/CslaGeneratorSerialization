using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.ValueTypes.DoubleTestsDomain;

[GeneratorSerializable]
public sealed partial class DoubleData
	: BusinessBase<DoubleData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<double> ContentsProperty =
		RegisterProperty<double>(nameof(DoubleData.Contents));
	public double Contents
	{
		get => this.GetProperty(DoubleData.ContentsProperty);
		set => this.SetProperty(DoubleData.ContentsProperty, value);
	}
}

[GeneratorSerializable]
public sealed partial class DoubleNullableData
	: BusinessBase<DoubleNullableData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<double?> ContentsProperty =
		RegisterProperty<double?>(nameof(DoubleNullableData.Contents));
	public double? Contents
	{
		get => this.GetProperty(DoubleNullableData.ContentsProperty);
		set => this.SetProperty(DoubleNullableData.ContentsProperty, value);
	}
}

internal static class DoubleTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<DoubleData>>();
		var data = await portal.CreateAsync();

		data.Contents = 3.14d;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (DoubleData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.EqualTo(3.14d));
	}

	[Test]
	public static async Task RoundtripWithNullableAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<DoubleNullableData>>();
		var data = await portal.CreateAsync();

		data.Contents = 3.14d;
		data.Contents = null;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (DoubleNullableData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.Null);
	}
}