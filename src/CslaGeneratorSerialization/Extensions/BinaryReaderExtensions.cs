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

	public static byte[][] ReadByteArrayArray(this BinaryReader reader)
	{
		var outerCount = reader.ReadInt32();
		var result = new byte[outerCount][];

		for (var i = 0; i < outerCount; i++)
		{
			result[i] = reader.ReadByteArray();
		}

		return result;
	}

	public static char[] ReadCharArray(this BinaryReader reader)
	{
		var length = reader.ReadInt32();
		var buffer = reader.ReadChars(length);
		return buffer;
	}

	public static List<int> ReadListOfInt32(this BinaryReader reader)
	{
		var count = reader.ReadInt32();
		var buffer = new int[count];

		for (var i = 0; i < count; i++)
		{
			buffer[i] = reader.ReadInt32();
		}

		return new List<int>(buffer);
	}
}