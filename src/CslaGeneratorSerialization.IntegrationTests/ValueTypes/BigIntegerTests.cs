using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Globalization;
using System.Numerics;

namespace CslaGeneratorSerialization.IntegrationTests.ValueTypes.BigIntegerTestsDomain;

[GeneratorSerializable]
public sealed partial class BigIntegerData
	: BusinessBase<BigIntegerData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<BigInteger> ContentsProperty =
		RegisterProperty<BigInteger>(nameof(BigIntegerData.Contents));
	public BigInteger Contents
	{
		get => this.GetProperty(BigIntegerData.ContentsProperty);
		set => this.SetProperty(BigIntegerData.ContentsProperty, value);
	}
}

[GeneratorSerializable]
public sealed partial class BigIntegerNullableData
	: BusinessBase<BigIntegerNullableData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<BigInteger?> ContentsProperty =
		RegisterProperty<BigInteger?>(nameof(BigIntegerNullableData.Contents));
	public BigInteger? Contents
	{
		get => this.GetProperty(BigIntegerNullableData.ContentsProperty);
		set => this.SetProperty(BigIntegerNullableData.ContentsProperty, value);
	}
}

internal static class BigIntegerTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<BigIntegerData>>();
		var data = await portal.CreateAsync();

		data.Contents = BigInteger.Parse("750389174809371089431", CultureInfo.CurrentCulture);

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (BigIntegerData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.EqualTo(data.Contents));
	}

	[Test]
	public static async Task RoundtripWithNullableAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<BigIntegerNullableData>>();
		var data = await portal.CreateAsync();

		data.Contents = BigInteger.Parse("750389174809371089431", CultureInfo.CurrentCulture);
		data.Contents = null;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (BigIntegerNullableData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.Null);
	}
}