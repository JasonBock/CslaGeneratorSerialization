namespace CslaGeneratorSerialization.Extensions;

public static class BinaryReaderExtensions
{
   public static SerializationState ReadStateValue(this BinaryReader reader) => 
		(SerializationState)reader.ReadByte();
}