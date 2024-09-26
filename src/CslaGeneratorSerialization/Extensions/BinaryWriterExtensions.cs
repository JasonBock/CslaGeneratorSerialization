namespace CslaGeneratorSerialization.Extensions;

public static class BinaryWriterExtensions
{
	// Before I forget...
	// the reason I have these two extensions is that
	// there are already methods to handle a byte[] or char[],
	// but I need to put the length in first.
	// So I need a different method signature.
	public static void Write(this BinaryWriter writer, (int length, byte[] buffer) value)
	{
		writer.Write(value.length);
		writer.Write(value.buffer);
	}

	public static void Write(this BinaryWriter writer, (int length, char[] buffer) value)
	{
		writer.Write(value.length);
		writer.Write(value.buffer);
	}

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