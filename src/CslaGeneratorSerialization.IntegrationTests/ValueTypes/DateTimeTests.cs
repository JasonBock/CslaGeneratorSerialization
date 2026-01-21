using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.ValueTypes.DateTimeTestsDomain;

[GeneratorSerializable]
public sealed partial class DateTimeData
	: BusinessBase<DateTimeData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<DateTime> ContentsProperty =
		RegisterProperty<DateTime>(nameof(DateTimeData.Contents));

	public DateTime Contents
	{
		get => this.GetProperty(DateTimeData.ContentsProperty);
		set => this.SetProperty(DateTimeData.ContentsProperty, value);
	}
}

[GeneratorSerializable]
public sealed partial class DateTimeNullableData
	: BusinessBase<DateTimeNullableData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<DateTime?> ContentsProperty =
		RegisterProperty<DateTime?>(nameof(DateTimeNullableData.Contents));
	public DateTime? Contents
	{
		get => this.GetProperty(DateTimeNullableData.ContentsProperty);
		set => this.SetProperty(DateTimeNullableData.ContentsProperty, value);
	}
}


internal static class DateTimeTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<DateTimeData>>();
		var data = await portal.CreateAsync();

		data.Contents = DateTime.UtcNow;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (DateTimeData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.EqualTo(data.Contents));
	}

	[Test]
	public static async Task RoundtripWithNullableAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<DateTimeNullableData>>();
		var data = await portal.CreateAsync();

		data.Contents = DateTime.UtcNow;
		data.Contents = null;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (DateTimeNullableData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.Null);
	}
}