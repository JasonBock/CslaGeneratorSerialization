using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.ValueTypes.IntTestsDomain;

[GeneratorSerializable]
public sealed partial class IntData
	: BusinessBase<IntData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<int> ContentsProperty =
		RegisterProperty<int>(nameof(IntData.Contents));

	public int Contents
	{
		get => this.GetProperty(IntData.ContentsProperty);
		set => this.SetProperty(IntData.ContentsProperty, value);
	}
}

[GeneratorSerializable]
public sealed partial class IntNullableData
	: BusinessBase<IntNullableData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<int?> ContentsProperty =
		RegisterProperty<int?>(nameof(IntNullableData.Contents));
	public int? Contents
	{
		get => this.GetProperty(IntNullableData.ContentsProperty);
		set => this.SetProperty(IntNullableData.ContentsProperty, value);
	}
}


internal static class IntTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<IntData>>();
		var data = await portal.CreateAsync();

		data.Contents = 2;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (IntData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.EqualTo(2));
	}

	[Test]
	public static async Task RoundtripWithNullableAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<IntNullableData>>();
		var data = await portal.CreateAsync();

		data.Contents = 3;
		data.Contents = null;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (IntNullableData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.Null);
	}
}