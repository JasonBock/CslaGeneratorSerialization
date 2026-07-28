using CslaGeneratorSerialization.Analysis.Models;
using NUnit.Framework;
using System.Collections.Immutable;

namespace CslaGeneratorSerialization.Analysis.Tests.Models;

internal static class EquatableArrayTests
{
	[Test]
	public static void IsEmptyOnEmptyCollectionExpression()
	{
		ImmutableArray<string> immutable = [];
		EquatableArray<string> equatable = [];

		Assert.That(immutable.IsEmpty, Is.True);
		Assert.That(equatable.IsEmpty, Is.True);
	}

	[Test]
	public static void LengthOnEmptyCollectionExpression()
	{
		ImmutableArray<string> immutable = [];
		EquatableArray<string> equatable = [];

		Assert.That(immutable.Length, Is.Zero);
		Assert.That(equatable.Length, Is.Zero);
	}

	[Test]
	public static void CanContainExpressionElementsInCollectionExpression()
	{
		ImmutableArray<string> immutable = ["first", "second"];
		EquatableArray<string> equatable = ["first", "second"];

		Assert.That(immutable.AsEquatableArray(), Is.EqualTo(equatable));
	}
}