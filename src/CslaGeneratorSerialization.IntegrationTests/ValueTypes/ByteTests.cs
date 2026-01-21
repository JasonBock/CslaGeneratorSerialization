using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.ValueTypes.ByteTestsDomain;

[GeneratorSerializable]
public sealed partial class ByteData
	: BusinessBase<ByteData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<byte> ContentsProperty =
		RegisterProperty<byte>(nameof(ByteData.Contents));
	public byte Contents
	{
		get => this.GetProperty(ByteData.ContentsProperty);
		set => this.SetProperty(ByteData.ContentsProperty, value);
	}
}

[GeneratorSerializable]
public sealed partial class ByteNullableData
	: BusinessBase<ByteNullableData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<byte?> ContentsProperty =
		RegisterProperty<byte?>(nameof(ByteNullableData.Contents));
	public byte? Contents
	{
		get => this.GetProperty(ByteNullableData.ContentsProperty);
		set => this.SetProperty(ByteNullableData.ContentsProperty, value);
	}
}

internal static class ByteTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<ByteData>>();
		var data = await portal.CreateAsync();

		data.Contents = 3;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (ByteData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.EqualTo(3));
	}

	[Test]
	public static async Task RoundtripWithNullableAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<ByteNullableData>>();
		var data = await portal.CreateAsync();

		data.Contents = 3;
		data.Contents = null;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (ByteNullableData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.Null);
	}
}