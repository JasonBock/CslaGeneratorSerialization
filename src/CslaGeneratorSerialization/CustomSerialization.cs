namespace CslaGeneratorSerialization;

public sealed class CustomSerialization<TType>
{
	private readonly Action<TType, BinaryWriter> serialize;
	private readonly Func<BinaryReader, TType> deserialize;

	public CustomSerialization(Action<TType, BinaryWriter> serialize, Func<BinaryReader, TType> deserialize) =>
		(this.serialize, this.deserialize) = (serialize, deserialize);

	public void Write(TType value, BinaryWriter writer) => this.serialize(value, writer);

	public TType Read(BinaryReader reader) => this.deserialize(reader);
}