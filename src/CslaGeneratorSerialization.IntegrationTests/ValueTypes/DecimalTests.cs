using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;

namespace CslaGeneratorSerialization.IntegrationTests.ValueTypes.DecimalTestsDomain;

[GeneratorSerializable]
public sealed partial class DecimalData
	: BusinessBase<DecimalData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<decimal> ContentsProperty =
		RegisterProperty<decimal>(nameof(DecimalData.Contents));

	public decimal Contents
	{
		get => this.GetProperty(DecimalData.ContentsProperty);
		set => this.SetProperty(DecimalData.ContentsProperty, value);
	}
}

[GeneratorSerializable]
public sealed partial class DecimalNullableData
	: BusinessBase<DecimalNullableData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<decimal?> ContentsProperty =
		RegisterProperty<decimal?>(nameof(DecimalNullableData.Contents));
	public decimal? Contents
	{
		get => this.GetProperty(DecimalNullableData.ContentsProperty);
		set => this.SetProperty(DecimalNullableData.ContentsProperty, value);
	}
}


internal static class DecimalTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<DecimalData>>();
		var data = await portal.CreateAsync();

		data.Contents = 1.0m;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (DecimalData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.EqualTo(1.0m));
	}

	[Test]
	public static async Task RoundtripWithNullableAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<DecimalNullableData>>();
		var data = await portal.CreateAsync();

		data.Contents = 1.0m;
		data.Contents = null;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (DecimalNullableData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.Null);
	}
}