namespace CslaGeneratorSerialization.Extensions;

public static class BinaryReaderExtensions
{
	extension(BinaryReader self)
	{
		public SerializationState ReadStateValue() =>
			(SerializationState)self.ReadByte();

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

		public T? Read<T>(Func<T> reader)
			where T : class
		{
			var state = (SerializationState)self.ReadByte();

			if (state == SerializationState.Value)
			{
				return reader();
			}
			else
			{
				return null;
			}
		}
	}
}