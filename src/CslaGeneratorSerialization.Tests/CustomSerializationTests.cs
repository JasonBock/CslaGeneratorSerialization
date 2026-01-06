using NUnit.Framework;

namespace CslaGeneratorSerialization.Tests.CustomSerializationTestsDomain;

internal static class CustomSerializationTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var customSerialization = new CustomSerialization<CustomData>(
			(data, writer) =>
			{
				writer.Write(data.Id);
			},
			(reader) => new() { Id = reader.ReadInt32() });

		var data = new CustomData { Id = 3 };

		var writerStream = new MemoryStream();
		using var writer = new BinaryWriter(writerStream);
		customSerialization.Write(data, writer);

		writerStream.Position = 0;
		var readerStream = new MemoryStream(writerStream.ToArray());
		using var reader = new BinaryReader(readerStream);
		var newData = customSerialization.Read(reader);

		Assert.That(newData.Id, Is.EqualTo(data.Id));
	}
}

public sealed class CustomData
{
	public int Id { get; set; }
}