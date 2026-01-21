using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.ValueTypes.GuidTestsDomain;

[GeneratorSerializable]
public sealed partial class GuidData
	: BusinessBase<GuidData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<Guid> ContentsProperty =
		RegisterProperty<Guid>(nameof(GuidData.Contents));

	public Guid Contents
	{
		get => this.GetProperty(GuidData.ContentsProperty);
		set => this.SetProperty(GuidData.ContentsProperty, value);
	}
}

[GeneratorSerializable]
public sealed partial class GuidNullableData
	: BusinessBase<GuidNullableData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<Guid?> ContentsProperty =
		RegisterProperty<Guid?>(nameof(GuidNullableData.Contents));
	public Guid? Contents
	{
		get => this.GetProperty(GuidNullableData.ContentsProperty);
		set => this.SetProperty(GuidNullableData.ContentsProperty, value);
	}
}


internal static class GuidTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<GuidData>>();
		var data = await portal.CreateAsync();

		data.Contents = Guid.NewGuid();

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (GuidData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.EqualTo(data.Contents));
	}

	[Test]
	public static async Task RoundtripWithNullableAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<GuidNullableData>>();
		var data = await portal.CreateAsync();

		data.Contents = Guid.NewGuid();
		data.Contents = null;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (GuidNullableData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.Null);
	}
}