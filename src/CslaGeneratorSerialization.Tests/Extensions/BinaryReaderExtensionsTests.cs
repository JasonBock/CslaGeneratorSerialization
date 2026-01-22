using CslaGeneratorSerialization.Extensions;
using NUnit.Framework;
using System.Globalization;
using System.Numerics;

namespace CslaGeneratorSerialization.Tests.Extensions;

internal static class BinaryReaderExtensionsTests
{
	[Test]
	public static async Task ReadStateValueAsync()
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
	public static async Task ReadBigIntegerAsync()
	{
		var value = BigInteger.Parse("473107483917948931749814", CultureInfo.CurrentCulture);

		var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write(value);

		stream.Position = 0;

		using var reader = new BinaryReader(stream);
		Assert.That(reader.ReadBigInteger(), Is.EqualTo(value));
	}

	[Test]
	public static async Task ReadByteArrayAsync()
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
	public static async Task ReadCharArrayAsync()
	{
		char[] value = ['a', 'b', 'c'];
		var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write((value.Length, value));

		stream.Position = 0;

		using var reader = new BinaryReader(stream);
		Assert.That(reader.ReadCharArray(), Is.EquivalentTo(value));
	}
}