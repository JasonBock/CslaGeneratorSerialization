using Csla.Serialization.Mobile;

namespace CslaGeneratorSerialization.Tests.MobileCustomSerializerTestsDomain;

public sealed class MobileCustomSerializerTests
{
	[Test]
	public async Task RoundtripAsync()
	{
		var customSerialization = new CustomSerialization<CustomData>(
			(data, writer) =>
			{
				writer.Write(data.Id);
			},
			(reader) => new() { Id = reader.ReadInt32() });

		var mobileCustomSerialization = 
			new MobileCustomSerializer<CustomData>(customSerialization);

		var data = new CustomData { Id = 3 };
		var info = new SerializationInfo(1, typeof(CustomData).AssemblyQualifiedName!);
		mobileCustomSerialization.Serialize(data, info);
		var newData = (CustomData)mobileCustomSerialization.Deserialize(info);

		await Assert.That(newData.Id).EqualTo(data.Id);
	}

	[Test]
	public async Task CreateWhenCustomSerializationIsNullAsync() =>
		await Assert.That(() => new MobileCustomSerializer<CustomData>(null!)).Throws<ArgumentNullException>();
}

public sealed class CustomData
{
	public int Id { get; set; }
}