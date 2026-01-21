using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.ValueTypes.CharTestsDomain;

[GeneratorSerializable]
public sealed partial class CharData
	: BusinessBase<CharData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<char> ContentsProperty =
		RegisterProperty<char>(nameof(CharData.Contents));
	public char Contents
	{
		get => this.GetProperty(CharData.ContentsProperty);
		set => this.SetProperty(CharData.ContentsProperty, value);
	}
}

[GeneratorSerializable]
public sealed partial class CharNullableData
	: BusinessBase<CharNullableData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<char?> ContentsProperty =
		RegisterProperty<char?>(nameof(CharNullableData.Contents));
	public char? Contents
	{
		get => this.GetProperty(CharNullableData.ContentsProperty);
		set => this.SetProperty(CharNullableData.ContentsProperty, value);
	}
}

internal static class CharTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<CharData>>();
		var data = await portal.CreateAsync();

		data.Contents = '3';

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (CharData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.EqualTo('3'));
	}

	[Test]
	public static async Task RoundtripWithNullableAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<CharNullableData>>();
		var data = await portal.CreateAsync();

		data.Contents = '3';
		data.Contents = null;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (CharNullableData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.Null);
	}
}