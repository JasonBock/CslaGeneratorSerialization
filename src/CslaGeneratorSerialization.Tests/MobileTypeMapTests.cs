namespace CslaGeneratorSerialization.Tests;

public sealed class MobileTypeMapTests
{
	[Test]
	public async Task CreateAsync()
	{
		var typeMap = new MobileTypeMap<string>();

		using (Assert.Multiple())
		{
			await Assert.That(typeMap.SerializerType).EqualTo(typeof(MobileCustomSerializer<string>));
			await Assert.That(typeMap.CanSerialize(typeof(string))).IsTrue();
			await Assert.That(typeMap.CanSerialize(typeof(Guid))).IsFalse();
		}
	}
}