using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Globalization;

namespace CslaGeneratorSerialization.IntegrationTests.ValueTypes.HalfTestsDomain;

[GeneratorSerializable]
public sealed partial class HalfData
	: BusinessBase<HalfData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<Half> ContentsProperty =
		RegisterProperty<Half>(nameof(HalfData.Contents));
	public Half Contents
	{
		get => this.GetProperty(HalfData.ContentsProperty);
		set => this.SetProperty(HalfData.ContentsProperty, value);
	}
}

[GeneratorSerializable]
public sealed partial class HalfNullableData
	: BusinessBase<HalfNullableData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<Half?> ContentsProperty =
		RegisterProperty<Half?>(nameof(HalfNullableData.Contents));
	public Half? Contents
	{
		get => this.GetProperty(HalfNullableData.ContentsProperty);
		set => this.SetProperty(HalfNullableData.ContentsProperty, value);
	}
}

internal static class HalfTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<HalfData>>();
		var data = await portal.CreateAsync();

		data.Contents = Half.Parse("3.14", CultureInfo.CurrentCulture);

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (HalfData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.EqualTo(data.Contents));
	}

	[Test]
	public static async Task RoundtripWithNullableAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<HalfNullableData>>();
		var data = await portal.CreateAsync();

		data.Contents = Half.Parse("3.14", CultureInfo.CurrentCulture);
		data.Contents = null;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (HalfNullableData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.Null);
	}
}