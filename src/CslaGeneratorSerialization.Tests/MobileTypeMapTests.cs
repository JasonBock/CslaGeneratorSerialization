using NUnit.Framework;

namespace CslaGeneratorSerialization.Tests;

internal static class MobileTypeMapTests
{
	[Test]
	public static async Task CreateAsync()
	{
		var typeMap = new MobileTypeMap<string>();

		using (Assert.EnterMultipleScope())
		{
			Assert.That(typeMap.SerializerType, Is.EqualTo(typeof(MobileCustomSerializer<string>)));
			Assert.That(typeMap.CanSerialize(typeof(string)), Is.True);
			Assert.That(typeMap.CanSerialize(typeof(Guid)), Is.False);
		}
	}
}