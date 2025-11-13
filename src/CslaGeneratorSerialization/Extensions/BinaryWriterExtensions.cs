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

		public void Write(DateTime value) =>
			self.Write(value.Ticks);

		public void Write(DateTimeOffset value)
		{
			self.Write(value.Ticks);
			self.Write(value.Offset.Ticks);
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

		public void Write<T>(T? value) where T : class
		{
			if (value is not null)
			{
				self.Write((byte)SerializationState.Value);
				self.Write(value);
			}
			else
			{
				self.Write((byte)SerializationState.Null);
			}
		}
	}
}