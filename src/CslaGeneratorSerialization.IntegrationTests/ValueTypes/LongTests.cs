using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.ValueTypes.LongTestsDomain;

[GeneratorSerializable]
public sealed partial class LongData
	: BusinessBase<LongData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<long> ContentsProperty =
		RegisterProperty<long>(nameof(LongData.Contents));
	public long Contents
	{
		get => this.GetProperty(LongData.ContentsProperty);
		set => this.SetProperty(LongData.ContentsProperty, value);
	}
}

[GeneratorSerializable]
public sealed partial class LongNullableData
	: BusinessBase<LongNullableData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<long?> ContentsProperty =
		RegisterProperty<long?>(nameof(LongNullableData.Contents));
	public long? Contents
	{
		get => this.GetProperty(LongNullableData.ContentsProperty);
		set => this.SetProperty(LongNullableData.ContentsProperty, value);
	}
}

internal static class LongTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<LongData>>();
		var data = await portal.CreateAsync();

		data.Contents = 3;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (LongData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.EqualTo(3));
	}

	[Test]
	public static async Task RoundtripWithNullableAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<LongNullableData>>();
		var data = await portal.CreateAsync();

		data.Contents = 3;
		data.Contents = null;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (LongNullableData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.Null);
	}
}