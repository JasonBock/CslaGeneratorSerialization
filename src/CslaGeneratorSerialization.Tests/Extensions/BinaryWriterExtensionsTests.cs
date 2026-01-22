using CslaGeneratorSerialization.Extensions;
using NUnit.Framework;
using System.Globalization;
using System.Numerics;

namespace CslaGeneratorSerialization.Tests.Extensions;

internal static class BinaryWriterExtensionsTests
{
	[Test]
	public static async Task WriteBigIntegerAsync()
	{
		var number = BigInteger.Parse("473107483917948931749814", CultureInfo.CurrentCulture);

		var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write(number);

		stream.Position = 0;

		var lengthBuffer = new byte[4];
		await stream.ReadAsync(lengthBuffer.AsMemory(0, 4));
		var length = BitConverter.ToInt32(lengthBuffer);

		var content = new byte[length];
		await stream.ReadAsync(content.AsMemory(0, length));

		Assert.That(new BigInteger(content), Is.EqualTo(number));
	}

	[Test]
	public static async Task WriteByteArrayAsync()
	{
		byte[] buffer = [22, 33, 44];

		var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write((buffer.Length, buffer));

		stream.Position = 0;

		using (Assert.EnterMultipleScope())
		{
			var lengthBuffer = new byte[4];
			await stream.ReadAsync(lengthBuffer.AsMemory(0, 4));
			var length = BitConverter.ToInt32(lengthBuffer);

			Assert.That(length, Is.EqualTo(3));

			var readBuffer = new byte[3];
			await stream.ReadAsync(readBuffer.AsMemory(0, 3));
			Assert.That(readBuffer, Is.EquivalentTo(buffer));
		}
	}

	[Test]
	public static async Task WriteCharArrayAsync()
	{
		char[] buffer = ['a', 'b', 'c'];

		var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write((buffer.Length, buffer));

		stream.Position = 0;

		using (Assert.EnterMultipleScope())
		{
			var lengthBuffer = new byte[4];
			await stream.ReadAsync(lengthBuffer.AsMemory(0, 4));
			var length = BitConverter.ToInt32(lengthBuffer);

			Assert.That(length, Is.EqualTo(3));

			var readBuffer = new byte[3];
			await stream.ReadAsync(readBuffer.AsMemory(0, 3));
			Assert.That(readBuffer, Has.Length.EqualTo(3));
			Assert.That((char)readBuffer[0], Is.EqualTo(buffer[0]));
			Assert.That((char)readBuffer[1], Is.EqualTo(buffer[1]));
			Assert.That((char)readBuffer[2], Is.EqualTo(buffer[2]));
		}
	}

	[Test]
	public static async Task WriteDateTimeAsync()
	{
		var value = DateTime.UtcNow;

		var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write(value);

		stream.Position = 0;

		var lengthBuffer = new byte[8];
		await stream.ReadAsync(lengthBuffer.AsMemory(0, 8));
		var ticks = BitConverter.ToInt64(lengthBuffer);

		Assert.That(ticks, Is.EqualTo(value.Ticks));
	}

	[Test]
	public static async Task WriteDateTimeOffsetAsync()
	{
		var value = DateTimeOffset.UtcNow;

		var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write(value);

		stream.Position = 0;

		using (Assert.EnterMultipleScope())
		{
			var ticksBuffer = new byte[8];
			await stream.ReadAsync(ticksBuffer.AsMemory(0, 8));
			var ticks = BitConverter.ToInt64(ticksBuffer);

			Assert.That(ticks, Is.EqualTo(value.Ticks));

			var offsetTicksBuffer = new byte[8];
			await stream.ReadAsync(offsetTicksBuffer.AsMemory(0, 8));
			var offsetTicks = BitConverter.ToInt64(offsetTicksBuffer);

			Assert.That(offsetTicks, Is.EqualTo(value.Offset.Ticks));
		}
	}

	[Test]
	public static async Task WriteDecimalAsync()
	{
		var value = 3.4M;
		var valueBits = decimal.GetBits(value);

		var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write(value);

		stream.Position = 0;

		using (Assert.EnterMultipleScope())
		{
			var buffer = new byte[valueBits.Length * 4];
			await stream.ReadAsync(buffer.AsMemory(0, buffer.Length));

			for (var i = 0; i < valueBits.Length; i++)
			{
				Assert.That(BitConverter.ToInt32(buffer.AsSpan(i * 4, 4)), Is.EqualTo(valueBits[i]));
			}
		}
	}

	[Test]
	public static async Task WriteGuidAsync()
	{
		var value = Guid.NewGuid();
		var valueBuffer = value.ToByteArray();

		var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write(value);

		stream.Position = 0;

		var buffer = new byte[valueBuffer.Length];
		await stream.ReadAsync(buffer.AsMemory(0, buffer.Length));

		Assert.That(buffer, Is.EquivalentTo(valueBuffer));
	}

	[Test]
	public static async Task WriteTimeSpanAsync()
	{
		var value = TimeSpan.FromTicks(DateTimeOffset.UtcNow.Ticks);

		var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write(value);

		stream.Position = 0;

		var ticksBuffer = new byte[8];
		await stream.ReadAsync(ticksBuffer.AsMemory(0, 8));
		var ticks = BitConverter.ToInt64(ticksBuffer);

		Assert.That(ticks, Is.EqualTo(value.Ticks));
	}
}