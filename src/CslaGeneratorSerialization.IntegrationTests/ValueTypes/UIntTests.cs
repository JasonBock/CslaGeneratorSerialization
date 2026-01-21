using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.ValueTypes.UIntTestsDomain;

[GeneratorSerializable]
public sealed partial class UIntData
	: BusinessBase<UIntData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<uint> ContentsProperty =
		RegisterProperty<uint>(nameof(UIntData.Contents));

	public uint Contents
	{
		get => this.GetProperty(UIntData.ContentsProperty);
		set => this.SetProperty(UIntData.ContentsProperty, value);
	}
}

[GeneratorSerializable]
public sealed partial class UIntNullableData
	: BusinessBase<UIntNullableData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<uint?> ContentsProperty =
		RegisterProperty<uint?>(nameof(UIntNullableData.Contents));
	public uint? Contents
	{
		get => this.GetProperty(UIntNullableData.ContentsProperty);
		set => this.SetProperty(UIntNullableData.ContentsProperty, value);
	}
}


internal static class UIntTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<UIntData>>();
		var data = await portal.CreateAsync();

		data.Contents = 2;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (UIntData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.EqualTo(2));
	}

	[Test]
	public static async Task RoundtripWithNullableAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<UIntNullableData>>();
		var data = await portal.CreateAsync();

		data.Contents = 3;
		data.Contents = null;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (UIntNullableData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.Null);
	}
}