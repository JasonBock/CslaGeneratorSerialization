using Csla.Serialization.Mobile;

namespace CslaGeneratorSerialization;

public sealed class MobileCustomSerializer<T>
	: IMobileSerializer
{
	private readonly CustomSerialization<T> customSerialization;

	public MobileCustomSerializer(CustomSerialization<T> customSerialization)
	{
		if (customSerialization is null) { throw new ArgumentNullException(nameof(customSerialization)); }

		this.customSerialization = customSerialization;
	}

	public object Deserialize(SerializationInfo info)
	{
		var value = info.GetValue<byte[]>("m");
		using var stream = new MemoryStream(value);
		using var reader = new BinaryReader(stream);
		return this.customSerialization.Read(reader)!;
	}

	public void Serialize(object obj, SerializationInfo info)
	{
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		this.customSerialization.Write((T)obj, writer);
		stream.Position = 0;
		info.AddValue("m", stream.ToArray());
	}
}