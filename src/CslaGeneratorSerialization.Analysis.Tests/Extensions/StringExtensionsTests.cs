using CslaGeneratorSerialization.Analysis.Extensions;
using NUnit.Framework;

namespace CslaGeneratorSerialization.Analysis.Tests.Extensions;

internal static class StringExtensionsTests
{
	[Test]
	public static void GenerateFileName() =>
		Assert.That("global:::<File>, ?".GenerateFileName(), Is.EqualTo("_File__null_"));
}