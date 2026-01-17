using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.ValueTypes.TimeSpanTestsDomain;

[GeneratorSerializable]
public sealed partial class TimeSpanData
	: BusinessBase<TimeSpanData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<TimeSpan> ContentsProperty =
		RegisterProperty<TimeSpan>(nameof(TimeSpanData.Contents));

	public TimeSpan Contents
	{
		get => this.GetProperty(TimeSpanData.ContentsProperty);
		set => this.SetProperty(TimeSpanData.ContentsProperty, value);
	}
}

[GeneratorSerializable]
public sealed partial class TimeSpanNullableData
	: BusinessBase<TimeSpanNullableData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<TimeSpan?> ContentsProperty =
		RegisterProperty<TimeSpan?>(nameof(TimeSpanNullableData.Contents));
	public TimeSpan? Contents
	{
		get => this.GetProperty(TimeSpanNullableData.ContentsProperty);
		set => this.SetProperty(TimeSpanNullableData.ContentsProperty, value);
	}
}


internal static class TimeSpanTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<TimeSpanData>>();
		var data = await portal.CreateAsync();

		data.Contents = TimeSpan.FromHours(1);

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (TimeSpanData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.EqualTo(TimeSpan.FromHours(1)));
	}

	[Test]
	public static async Task RoundtripWithNullableAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<TimeSpanNullableData>>();
		var data = await portal.CreateAsync();

		data.Contents = TimeSpan.FromHours(2);
		data.Contents = null;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (TimeSpanNullableData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.Null);
	}
}