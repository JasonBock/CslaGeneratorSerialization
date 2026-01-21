using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.ValueTypes.DateTimeOffsetTestsDomain;

[GeneratorSerializable]
public sealed partial class DateTimeOffsetData
	: BusinessBase<DateTimeOffsetData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<DateTimeOffset> ContentsProperty =
		RegisterProperty<DateTimeOffset>(nameof(DateTimeOffsetData.Contents));

	public DateTimeOffset Contents
	{
		get => this.GetProperty(DateTimeOffsetData.ContentsProperty);
		set => this.SetProperty(DateTimeOffsetData.ContentsProperty, value);
	}
}

[GeneratorSerializable]
public sealed partial class DateTimeOffsetNullableData
	: BusinessBase<DateTimeOffsetNullableData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<DateTimeOffset?> ContentsProperty =
		RegisterProperty<DateTimeOffset?>(nameof(DateTimeOffsetNullableData.Contents));
	public DateTimeOffset? Contents
	{
		get => this.GetProperty(DateTimeOffsetNullableData.ContentsProperty);
		set => this.SetProperty(DateTimeOffsetNullableData.ContentsProperty, value);
	}
}


internal static class DateTimeOffsetTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<DateTimeOffsetData>>();
		var data = await portal.CreateAsync();

		data.Contents = DateTimeOffset.UtcNow;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (DateTimeOffsetData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.EqualTo(data.Contents));
	}

	[Test]
	public static async Task RoundtripWithNullableAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<DateTimeOffsetNullableData>>();
		var data = await portal.CreateAsync();

		data.Contents = DateTimeOffset.UtcNow;
		data.Contents = null;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (DateTimeOffsetNullableData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.Null);
	}
}