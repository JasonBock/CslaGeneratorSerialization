using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.ValueTypes.SingleTestsDomain;

[GeneratorSerializable]
public sealed partial class SingleData
	: BusinessBase<SingleData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<float> ContentsProperty =
		RegisterProperty<float>(nameof(SingleData.Contents));
	public float Contents
	{
		get => this.GetProperty(SingleData.ContentsProperty);
		set => this.SetProperty(SingleData.ContentsProperty, value);
	}
}

[GeneratorSerializable]
public sealed partial class SingleNullableData
	: BusinessBase<SingleNullableData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<float?> ContentsProperty =
		RegisterProperty<float?>(nameof(SingleNullableData.Contents));
	public float? Contents
	{
		get => this.GetProperty(SingleNullableData.ContentsProperty);
		set => this.SetProperty(SingleNullableData.ContentsProperty, value);
	}
}

internal static class SingleTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<SingleData>>();
		var data = await portal.CreateAsync();

		data.Contents = 3.14f;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (SingleData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.EqualTo(3.14f));
	}

	[Test]
	public static async Task RoundtripWithNullableAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<SingleNullableData>>();
		var data = await portal.CreateAsync();

		data.Contents = 3.14f;
		data.Contents = null;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (SingleNullableData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.Null);
	}
}