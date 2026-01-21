using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.ValueTypes.UShortTestsDomain;

[GeneratorSerializable]
public sealed partial class UShortData
	: BusinessBase<UShortData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<ushort> ContentsProperty =
		RegisterProperty<ushort>(nameof(UShortData.Contents));
	public ushort Contents
	{
		get => this.GetProperty(UShortData.ContentsProperty);
		set => this.SetProperty(UShortData.ContentsProperty, value);
	}
}

[GeneratorSerializable]
public sealed partial class UShortNullableData
	: BusinessBase<UShortNullableData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<ushort?> ContentsProperty =
		RegisterProperty<ushort?>(nameof(UShortNullableData.Contents));
	public ushort? Contents
	{
		get => this.GetProperty(UShortNullableData.ContentsProperty);
		set => this.SetProperty(UShortNullableData.ContentsProperty, value);
	}
}

internal static class UShortTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<UShortData>>();
		var data = await portal.CreateAsync();

		data.Contents = 3;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (UShortData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.EqualTo(3));
	}

	[Test]
	public static async Task RoundtripWithNullableAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<UShortNullableData>>();
		var data = await portal.CreateAsync();

		data.Contents = 3;
		data.Contents = null;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (UShortNullableData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.Null);
	}
}