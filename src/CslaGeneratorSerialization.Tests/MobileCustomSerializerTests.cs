using Csla.Serialization.Mobile;
using NUnit.Framework;

namespace CslaGeneratorSerialization.Tests.MobileCustomSerializerTestsDomain;

public static class MobileCustomSerializerTests
{
	[Test]
	public static void Roundtrip()
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

		Assert.That(newData.Id, Is.EqualTo(data.Id));
	}

	[Test]
	public static void CreateWhenCustomSerializationIsNull() =>
		Assert.That(() => new MobileCustomSerializer<CustomData>(null!), Throws.TypeOf<ArgumentNullException>());
}

public sealed class CustomData
{
	public int Id { get; set; }
}