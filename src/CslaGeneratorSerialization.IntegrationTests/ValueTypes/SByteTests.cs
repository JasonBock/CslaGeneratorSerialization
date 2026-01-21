using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.ValueTypes.SByteTestsDomain;

[GeneratorSerializable]
public sealed partial class SByteData
	: BusinessBase<SByteData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<sbyte> ContentsProperty =
		RegisterProperty<sbyte>(nameof(SByteData.Contents));
	public sbyte Contents
	{
		get => this.GetProperty(SByteData.ContentsProperty);
		set => this.SetProperty(SByteData.ContentsProperty, value);
	}
}

[GeneratorSerializable]
public sealed partial class SByteNullableData
	: BusinessBase<SByteNullableData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<sbyte?> ContentsProperty =
		RegisterProperty<sbyte?>(nameof(SByteNullableData.Contents));
	public sbyte? Contents
	{
		get => this.GetProperty(SByteNullableData.ContentsProperty);
		set => this.SetProperty(SByteNullableData.ContentsProperty, value);
	}
}

internal static class SByteTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<SByteData>>();
		var data = await portal.CreateAsync();

		data.Contents = 3;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (SByteData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.EqualTo(3));
	}

	[Test]
	public static async Task RoundtripWithNullableAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<SByteNullableData>>();
		var data = await portal.CreateAsync();

		data.Contents = 3;
		data.Contents = null;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (SByteNullableData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.Null);
	}
}