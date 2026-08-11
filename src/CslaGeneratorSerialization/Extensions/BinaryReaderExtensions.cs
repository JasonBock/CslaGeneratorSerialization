using System.Numerics;

namespace CslaGeneratorSerialization.Extensions;

public static class BinaryReaderExtensions
{
	extension(BinaryReader self)
	{
		public SerializationState ReadStateValue() =>
			(SerializationState)self.ReadByte();

		public BigInteger ReadBigInteger() =>
			new(self.ReadByteArray());

		public byte[] ReadByteArray()
		{
			var length = self.ReadInt32();
			return self.ReadBytes(length);
		}

		public char[] ReadCharArray()
		{
			var length = self.ReadInt32();
			return self.ReadChars(length);
		}

		public short[] ReadInt16Array()
		{
			var length = self.ReadInt32();
			var buffer = new short[length];

			for (var i = 0; i < length; i++)
			{
				buffer[i] = self.ReadInt16();
			}

			return buffer;
		}

		public ushort[] ReadUInt16Array()
		{
			var length = self.ReadInt32();
			var buffer = new ushort[length];

			for (var i = 0; i < length; i++)
			{
				buffer[i] = self.ReadUInt16();	
			}

			return buffer;
		}

		public int[] ReadInt32Array()
		{
			var length = self.ReadInt32();
			var buffer = new int[length];

			for (var i = 0; i < length; i++)
			{
				buffer[i] = self.ReadInt32();
			}

			return buffer;
		}

		public uint[] ReadUInt32Array()
		{
			var length = self.ReadInt32();
			var buffer = new uint[length];

			for (var i = 0; i < length; i++)
			{
				buffer[i] = self.ReadUInt32();
			}

			return buffer;
		}

		public long[] ReadInt64Array()
		{
			var length = self.ReadInt32();
			var buffer = new long[length];

			for (var i = 0; i < length; i++)
			{
				buffer[i] = self.ReadInt64();
			}

			return buffer;
		}

		public ulong[] ReadUInt64Array()
		{
			var length = self.ReadInt32();
			var buffer = new ulong[length];

			for (var i = 0; i < length; i++)
			{
				buffer[i] = self.ReadUInt64();
			}

			return buffer;
		}
	}
}