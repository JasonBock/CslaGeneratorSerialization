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
}