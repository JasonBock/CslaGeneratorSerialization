using NUnit.Framework;

namespace CslaGeneratorSerialization.Tests;

public static class MobileTypeMapTests
{
	[Test]
	public static void Create()
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