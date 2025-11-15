using CslaGeneratorSerialization.Extensions;
using System.Text;

namespace CslaGeneratorSerialization.Tests.Extensions;

public sealed class BinaryWriterExtensionsTests
{
	[Test]
	public async Task WriteByteArrayAsync()
	{
		byte[] buffer = [22, 33, 44];

		var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write((buffer.Length, buffer));

		stream.Position = 0;

		using (Assert.Multiple())
		{
			var lengthBuffer = new byte[4];
			await stream.ReadAsync(lengthBuffer.AsMemory(0, 4));
			var length = BitConverter.ToInt32(lengthBuffer);

			await Assert.That(length).IsEqualTo(3);

			var readBuffer = new byte[3];
			await stream.ReadAsync(readBuffer.AsMemory(0, 3));
			await Assert.That(readBuffer).IsEquivalentTo(buffer);
		}
	}

	[Test]
	public async Task WriteCharArrayAsync()
	{
		char[] buffer = ['a', 'b', 'c'];

		var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write((buffer.Length, buffer));

		stream.Position = 0;

		using (Assert.Multiple())
		{
			var lengthBuffer = new byte[4];
			await stream.ReadAsync(lengthBuffer.AsMemory(0, 4));
			var length = BitConverter.ToInt32(lengthBuffer);

			await Assert.That(length).IsEqualTo(3);

			var readBuffer = new byte[3];
			await stream.ReadAsync(readBuffer.AsMemory(0, 3));
			await Assert.That(readBuffer).HasCount(3);
			await Assert.That((char)readBuffer[0]).IsEqualTo(buffer[0]);
			await Assert.That((char)readBuffer[1]).IsEqualTo(buffer[1]);
			await Assert.That((char)readBuffer[2]).IsEqualTo(buffer[2]);
		}
	}

	[Test]
	public async Task WriteDateTimeAsync()
	{
		var value = DateTime.UtcNow;

		var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write(value);

		stream.Position = 0;

		var lengthBuffer = new byte[8];
		await stream.ReadAsync(lengthBuffer.AsMemory(0, 8));
		var ticks = BitConverter.ToInt64(lengthBuffer);

		await Assert.That(ticks).IsEqualTo(value.Ticks);
	}

	[Test]
	public async Task WriteDateTimeOffsetAsync()
	{
		var value = DateTimeOffset.UtcNow;

		var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write(value);

		stream.Position = 0;

		using (Assert.Multiple())
		{
			var ticksBuffer = new byte[8];
			await stream.ReadAsync(ticksBuffer.AsMemory(0, 8));
			var ticks = BitConverter.ToInt64(ticksBuffer);

			await Assert.That(ticks).IsEqualTo(value.Ticks);

			var offsetTicksBuffer = new byte[8];
			await stream.ReadAsync(offsetTicksBuffer.AsMemory(0, 8));
			var offsetTicks = BitConverter.ToInt64(offsetTicksBuffer);

			await Assert.That(offsetTicks).IsEqualTo(value.Offset.Ticks);
		}
	}

	[Test]
	public async Task WriteDecimalAsync()
	{
		var value = 3.4M;
		var valueBits = decimal.GetBits(value);

		var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write(value);

		stream.Position = 0;

		using (Assert.Multiple())
		{
			var buffer = new byte[valueBits.Length * 4];
			await stream.ReadAsync(buffer.AsMemory(0, buffer.Length));

			for (var i = 0; i < valueBits.Length; i++)
			{
				await Assert.That(BitConverter.ToInt32(buffer.AsSpan(i * 4, 4))).IsEqualTo(valueBits[i]);
			}
		}
	}

	[Test]
	public async Task WriteGuidAsync()
	{
		var value = Guid.NewGuid();
		var valueBuffer = value.ToByteArray();

		var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write(value);

		stream.Position = 0;

		var buffer = new byte[valueBuffer.Length];
		await stream.ReadAsync(buffer.AsMemory(0, buffer.Length));

		await Assert.That(buffer).IsEquivalentTo(valueBuffer);
	}

	[Test]
	public async Task WriteTimeSpanAsync()
	{
		var value = TimeSpan.FromTicks(DateTimeOffset.UtcNow.Ticks);

		var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write(value);

		stream.Position = 0;

		var ticksBuffer = new byte[8];
		await stream.ReadAsync(ticksBuffer.AsMemory(0, 8));
		var ticks = BitConverter.ToInt64(ticksBuffer);

		await Assert.That(ticks).IsEqualTo(value.Ticks);
	}
}