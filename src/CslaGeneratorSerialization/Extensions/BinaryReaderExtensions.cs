using System.Numerics;

namespace CslaGeneratorSerialization.Extensions;

public static class BinaryReaderExtensions
{
	extension(BinaryReader self)
	{
		public SerializationState ReadStateValue() =>
			(SerializationState)self.ReadByte();

		public BigInteger ReadBigInteger() => 
			new BigInteger(self.ReadByteArray());

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
	}
}