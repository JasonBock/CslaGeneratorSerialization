using CslaGeneratorSerialization.Extensions;
using NUnit.Framework;
using System.Globalization;
using System.Numerics;

namespace CslaGeneratorSerialization.Tests.Extensions;

internal static class BinaryReaderExtensionsTests
{
	[Test]
	public static void ReadStateValue()
	{
		var value = SerializationState.Duplicate;
		var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write((byte)value);

		stream.Position = 0;

		using var reader = new BinaryReader(stream);
		Assert.That(reader.ReadStateValue(), Is.EqualTo(value));
	}

	[Test]
	public static void ReadBigInteger()
	{
		var value = BigInteger.Parse("473107483917948931749814", CultureInfo.InvariantCulture);

		var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write(value);

		stream.Position = 0;

		using var reader = new BinaryReader(stream);
		Assert.That(reader.ReadBigInteger(), Is.EqualTo(value));
	}

	[Test]
	public static void ReadByteArray()
	{
		byte[] value = [22, 33, 44];
		var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write((value.Length, value));

		stream.Position = 0;

		using var reader = new BinaryReader(stream);
		Assert.That(reader.ReadByteArray(), Is.EquivalentTo(value));
	}

	[Test]
	public static void ReadCharArray()
	{
		char[] value = ['a', 'b', 'c'];
		var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write((value.Length, value));

		stream.Position = 0;

		using var reader = new BinaryReader(stream);
		Assert.That(reader.ReadCharArray(), Is.EquivalentTo(value));
	}

	[Test]
	public static void ReadInt32Array()
	{
		int[] value = [22, 33, 44];
		var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write((value.Length, value));

		stream.Position = 0;

		using var reader = new BinaryReader(stream);
		Assert.That(reader.ReadInt32Array(), Is.EquivalentTo(value));
	}

	[Test]
	public static void ReadUInt32Array()
	{
		uint[] value = [22, 33, 44];
		var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write((value.Length, value));

		stream.Position = 0;

		using var reader = new BinaryReader(stream);
		Assert.That(reader.ReadUInt32Array(), Is.EquivalentTo(value));
	}

	[Test]
	public static void ReadInt64Array()
	{
		long[] value = [22, 33, 44];
		var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write((value.Length, value));

		stream.Position = 0;

		using var reader = new BinaryReader(stream);
		Assert.That(reader.ReadInt64Array(), Is.EquivalentTo(value));
	}

	[Test]
	public static void ReadUInt64Array()
	{
		ulong[] value = [22, 33, 44];
		var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write((value.Length, value));

		stream.Position = 0;

		using var reader = new BinaryReader(stream);
		Assert.That(reader.ReadUInt64Array(), Is.EquivalentTo(value));
	}
}