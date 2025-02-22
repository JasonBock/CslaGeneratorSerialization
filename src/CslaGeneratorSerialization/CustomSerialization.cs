using Csla.Serialization.Mobile;

namespace CslaGeneratorSerialization;

public abstract class CustomSerialization
{
	protected CustomSerialization()
		: base() { }

	public abstract ITypeMap TypeMap { get; }
}

public sealed class CustomSerialization<TType>
	: CustomSerialization
{
	private readonly Action<TType, BinaryWriter> serialize;
	private readonly Func<BinaryReader, TType> deserialize;

   public override ITypeMap TypeMap => new MobileTypeMap<TType>();

   public CustomSerialization(Action<TType, BinaryWriter> serialize, Func<BinaryReader, TType> deserialize) =>
		(this.serialize, this.deserialize) = (serialize, deserialize);

	public void Write(TType value, BinaryWriter writer) => this.serialize(value, writer);

	public TType Read(BinaryReader reader) => this.deserialize(reader);
}