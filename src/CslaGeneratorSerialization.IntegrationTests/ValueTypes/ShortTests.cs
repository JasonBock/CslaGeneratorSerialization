using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.ValueTypes.ShortTestsDomain;

[GeneratorSerializable]
public sealed partial class ShortData
	: BusinessBase<ShortData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<short> ContentsProperty =
		RegisterProperty<short>(nameof(ShortData.Contents));
	public short Contents
	{
		get => this.GetProperty(ShortData.ContentsProperty);
		set => this.SetProperty(ShortData.ContentsProperty, value);
	}
}

[GeneratorSerializable]
public sealed partial class ShortNullableData
	: BusinessBase<ShortNullableData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<short?> ContentsProperty =
		RegisterProperty<short?>(nameof(ShortNullableData.Contents));
	public short? Contents
	{
		get => this.GetProperty(ShortNullableData.ContentsProperty);
		set => this.SetProperty(ShortNullableData.ContentsProperty, value);
	}
}

internal static class ShortTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<ShortData>>();
		var data = await portal.CreateAsync();

		data.Contents = 3;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (ShortData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.EqualTo(3));
	}

	[Test]
	public static async Task RoundtripWithNullableAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<ShortNullableData>>();
		var data = await portal.CreateAsync();

		data.Contents = 3;
		data.Contents = null;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (ShortNullableData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.Null);
	}
}