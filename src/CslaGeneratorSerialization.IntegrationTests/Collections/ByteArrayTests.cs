using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.Collections.ByteArrayTestsDomain;

[GeneratorSerializable]
public sealed partial class ByteArrayData
	: BusinessBase<ByteArrayData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<byte[]> ContentsProperty =
		RegisterProperty<byte[]>(nameof(ByteArrayData.Contents));
#pragma warning disable CA1819 // Properties should not return arrays
	public byte[] Contents
#pragma warning restore CA1819 // Properties should not return arrays
	{
		get => this.GetProperty(ByteArrayData.ContentsProperty);
		set => this.SetProperty(ByteArrayData.ContentsProperty, value);
	}
}

public static class ByteArrayTests
{
	[Test]
	public static void Roundtrip()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<ByteArrayData>>();
		var data = portal.Create();

		data.Contents = [22, 33, 44];

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (ByteArrayData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.EquivalentTo(new byte[] { 22, 33, 44 }));
	}

	[Test]
	public static void RoundtripWithNullable()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<ByteArrayData>>();
		var data = portal.Create();

		data.Contents = [22, 33, 44];
		data.Contents = null!;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (ByteArrayData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.Null);
	}
}