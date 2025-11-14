using CslaGeneratorSerialization.Extensions;

namespace CslaGeneratorSerialization.Tests.Extensions;

public sealed class BinaryReaderExtensionsTests
{
	[Test]
	public async Task ReadStateValueAsync()
	{
		var value = SerializationState.Duplicate;
		var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write((byte)value);

		stream.Position = 0;

		using var reader = new BinaryReader(stream);
		await Assert.That(reader.ReadStateValue()).IsEqualTo(value);
	}

	[Test]
	public async Task ReadByteArrayAsync()
	{
		byte[] value = [22, 33, 44];
		var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write((value.Length, value));

		stream.Position = 0;

		using var reader = new BinaryReader(stream);
		await Assert.That(reader.ReadByteArray()).IsEquivalentTo(value);
	}

	[Test]
	public async Task ReadCharArrayAsync()
	{
		char[] value = ['a', 'b', 'c'];
		var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write((value.Length, value));

		stream.Position = 0;

		using var reader = new BinaryReader(stream);
		await Assert.That(reader.ReadCharArray()).IsEquivalentTo(value);
	}

	[Test]
	public async Task ReadNullableWhenNotNullAsync()
	{
		var value = "data";

		var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.WriteNullable(value, writer.Write);

		stream.Position = 0;

		using var reader = new BinaryReader(stream);
		await Assert.That(reader.ReadNullable(reader.ReadString)).IsEqualTo(value);
	}

	[Test]
	public async Task ReadNullableWhenNullAsync()
	{
		string? value = null;

		var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.WriteNullable(value, writer.Write);

		stream.Position = 0;

		using var reader = new BinaryReader(stream);
		await Assert.That(reader.ReadNullable(reader.ReadString)).IsNull();
	}
}