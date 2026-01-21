using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.ValueTypes.ULongTestsDomain;

[GeneratorSerializable]
public sealed partial class ULongData
	: BusinessBase<ULongData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<ulong> ContentsProperty =
		RegisterProperty<ulong>(nameof(ULongData.Contents));
	public ulong Contents
	{
		get => this.GetProperty(ULongData.ContentsProperty);
		set => this.SetProperty(ULongData.ContentsProperty, value);
	}
}

[GeneratorSerializable]
public sealed partial class ULongNullableData
	: BusinessBase<ULongNullableData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<ulong?> ContentsProperty =
		RegisterProperty<ulong?>(nameof(ULongNullableData.Contents));
	public ulong? Contents
	{
		get => this.GetProperty(ULongNullableData.ContentsProperty);
		set => this.SetProperty(ULongNullableData.ContentsProperty, value);
	}
}

internal static class ULongTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<ULongData>>();
		var data = await portal.CreateAsync();

		data.Contents = 3;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (ULongData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.EqualTo(3));
	}

	[Test]
	public static async Task RoundtripWithNullableAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<ULongNullableData>>();
		var data = await portal.CreateAsync();

		data.Contents = 3;
		data.Contents = null;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (ULongNullableData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.Null);
	}
}