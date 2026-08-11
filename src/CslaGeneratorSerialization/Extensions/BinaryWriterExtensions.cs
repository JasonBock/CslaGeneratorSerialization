using System.Numerics;

namespace CslaGeneratorSerialization.Extensions;

public static class BinaryWriterExtensions
{
	extension(BinaryWriter self)
	{
		// Before I forget...
		// the reason I have these two extensions is that
		// there are already methods to handle a byte[] or char[],
		// but I need to put the length in first.
		// So I need a different method signature.
		public void Write((int length, byte[] buffer) value)
		{
			self.Write(value.length);
			self.Write(value.buffer);
		}

		public void Write((int length, char[] buffer) value)
		{
			self.Write(value.length);
			self.Write(value.buffer);
		}

		public void Write((int length, short[] buffer) value)
		{
			self.Write(value.length);

			foreach (var item in value.buffer)
			{
				self.Write(item);
			}
		}

		public void Write((int length, ushort[] buffer) value)
		{
			self.Write(value.length);

			foreach (var item in value.buffer)
			{
				self.Write(item);
			}
		}

		public void Write((int length, int[] buffer) value)
		{
			self.Write(value.length);

			foreach (var item in value.buffer)
			{
				self.Write(item);
			}
		}

		public void Write((int length, uint[] buffer) value)
		{
			self.Write(value.length);

			foreach (var item in value.buffer)
			{
				self.Write(item);
			}
		}

		public void Write((int length, long[] buffer) value)
		{
			self.Write(value.length);

			foreach (var item in value.buffer)
			{
				self.Write(item);
			}
		}

		public void Write((int length, ulong[] buffer) value)
		{
			self.Write(value.length);

			foreach (var item in value.buffer)
			{
				self.Write(item);
			}
		}

		public void Write(DateTime value) =>
			self.Write(value.Ticks);

		public void Write(DateTimeOffset value)
		{
			self.Write(value.Ticks);
			self.Write(value.Offset.Ticks);
		}

		public void Write(BigInteger value)
		{
			var bytes = value.ToByteArray();
			self.Write((bytes.Length, bytes));
		}

		public void Write(decimal value)
		{
			var bits = decimal.GetBits(value);

			foreach (var bit in bits)
			{
				self.Write(bit);
			}
		}

		public void Write(Guid value) =>
			self.Write(value.ToByteArray());

		public void Write(TimeSpan value) =>
			self.Write(value.Ticks);
	}
}