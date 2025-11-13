using Csla;
using Microsoft.Extensions.DependencyInjection;

namespace CslaGeneratorSerialization.IntegrationTests.ValueTypes.BooleanTestsDomain;

[GeneratorSerializable]
public sealed partial class BooleanData
	: BusinessBase<BooleanData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<bool> ContentsProperty =
		RegisterProperty<bool>(nameof(BooleanData.Contents));
	public bool Contents
	{
		get => this.GetProperty(BooleanData.ContentsProperty);
		set => this.SetProperty(BooleanData.ContentsProperty, value);
	}
}

[GeneratorSerializable]
public sealed partial class BooleanNullableData
	: BusinessBase<BooleanNullableData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<bool?> ContentsProperty =
		RegisterProperty<bool?>(nameof(BooleanNullableData.Contents));
	public bool? Contents
	{
		get => this.GetProperty(BooleanNullableData.ContentsProperty);
		set => this.SetProperty(BooleanNullableData.ContentsProperty, value);
	}
}

public sealed class BooleanTests
{
	[Test]
	public async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<BooleanData>>();
		var data = await portal.CreateAsync();

		data.Contents = true;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (BooleanData)formatter.Deserialize(stream)!;

		await Assert.That(newData.Contents).IsTrue();
	}

	[Test]
	public async Task RoundtripWithNullableAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<BooleanNullableData>>();
		var data = await portal.CreateAsync();

		data.Contents = true;
		data.Contents = null;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (BooleanNullableData)formatter.Deserialize(stream)!;

		await Assert.That(newData.Contents).IsNull();
	}
}