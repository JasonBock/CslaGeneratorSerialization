namespace CslaGeneratorSerialization.Extensions;

public static class BinaryWriterExtensions
{
   public static void Write(this BinaryWriter writer, DateTime value) => 
		writer.Write(value.Ticks);

   public static void Write(this BinaryWriter writer, DateTimeOffset value)
	{
		writer.Write(value.Ticks);
		writer.Write(value.Offset.Ticks);
	}

	public static void Write(this BinaryWriter writer, decimal value)
	{
		var bits = decimal.GetBits(value);

		foreach (var bit in bits)
		{
			writer.Write(bit);
		}
	}

	public static void Write(this BinaryWriter writer, Guid value) =>
		writer.Write(value.ToByteArray());

	public static void Write(this BinaryWriter writer, TimeSpan value) =>
		writer.Write(value.Ticks);
}