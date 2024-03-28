using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.Collections;

[Serializable]
public sealed partial class ByteArrayArrayData
	: BusinessBase<ByteArrayArrayData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<byte[][]> ContentsProperty =
		RegisterProperty<byte[][]>(_ => _.Contents);
#pragma warning disable CA1819 // Properties should not return arrays
	public byte[][] Contents
#pragma warning restore CA1819 // Properties should not return arrays
	{
		get => this.GetProperty(ByteArrayArrayData.ContentsProperty);
		set => this.SetProperty(ByteArrayArrayData.ContentsProperty, value);
	}
}

public static class ByteArrayArrayTests
{
	[Test]
	public static void Roundtrip()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>());
		var portal = provider.GetRequiredService<IDataPortal<ByteArrayArrayData>>();
		var data = portal.Create();

		data.Contents =
			[
				[ 22, 33, 44 ],
				[ 55, 66, 77 ],
			];

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (ByteArrayArrayData)formatter.Deserialize(stream);

		CollectionAssert.AreEqual(newData.Contents, 
			new byte[][]
			{
				[ 22, 33, 44 ],
				[ 55, 66, 77 ],
			});
	}

	[Test]
	public static void RoundtripWithNullable()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>());
		var portal = provider.GetRequiredService<IDataPortal<ByteArrayArrayData>>();
		var data = portal.Create();

		data.Contents =
			[
				[ 22, 33, 44 ],
				[ 55, 66, 77 ],
			];
		data.Contents = null!;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (ByteArrayArrayData)formatter.Deserialize(stream);

		Assert.That(newData.Contents, Is.Null);
	}
}