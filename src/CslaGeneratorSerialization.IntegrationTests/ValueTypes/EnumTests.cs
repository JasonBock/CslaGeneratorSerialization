using Csla;
using Microsoft.Extensions.DependencyInjection;

namespace CslaGeneratorSerialization.IntegrationTests.ValueTypes.EnumTestsDomain;

public enum States { First, Second, Third }

[GeneratorSerializable]
public sealed partial class EnumData
	: BusinessBase<EnumData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<States> ContentsProperty =
		RegisterProperty<States>(nameof(EnumData.Contents));
	public States Contents
	{
		get => this.GetProperty(EnumData.ContentsProperty);
		set => this.SetProperty(EnumData.ContentsProperty, value);
	}
}

[GeneratorSerializable]
public sealed partial class EnumNullableData
	: BusinessBase<EnumNullableData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<States?> ContentsProperty =
		RegisterProperty<States?>(nameof(EnumNullableData.Contents));
	public States? Contents
	{
		get => this.GetProperty(EnumNullableData.ContentsProperty);
		set => this.SetProperty(EnumNullableData.ContentsProperty, value);
	}
}

public sealed class EnumTests
{
	[Test]
	public async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<EnumData>>();
		var data = await portal.CreateAsync();

		data.Contents = States.Second;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (EnumData)formatter.Deserialize(stream)!;

		await Assert.That(newData.Contents).IsEqualTo(States.Second);
	}

	[Test]
	public async Task RoundtripWithNullableAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<EnumNullableData>>();
		var data = await portal.CreateAsync();

		data.Contents = States.Second;
		data.Contents = null!;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (EnumNullableData)formatter.Deserialize(stream)!;

		await Assert.That(newData.Contents).IsNull();
	}
}