namespace CslaGeneratorSerialization.Extensions;

public static class BinaryReaderExtensions
{
	public static SerializationState ReadStateValue(this BinaryReader reader) =>
		(SerializationState)reader.ReadByte();

	public static byte[] ReadByteArray(this BinaryReader reader)
	{
		var length = reader.ReadInt32();
		var buffer = reader.ReadBytes(length);
		return buffer;
	}

	public static char[] ReadCharArray(this BinaryReader reader)
	{
		var length = reader.ReadInt32();
		var buffer = reader.ReadChars(length);
		return buffer;
	}
}