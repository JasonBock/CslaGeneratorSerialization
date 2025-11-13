using Csla;
using Microsoft.Extensions.DependencyInjection;

namespace CslaGeneratorSerialization.IntegrationTests.ReferenceTypes.StringTestsDomain;

[GeneratorSerializable]
public sealed partial class StringData
	: BusinessBase<StringData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<string> ContentsProperty =
		RegisterProperty<string>(nameof(StringData.Contents));
	public string Contents
	{
		get => this.GetProperty(StringData.ContentsProperty)!;
		set => this.SetProperty(StringData.ContentsProperty, value);
	}
}

public sealed class StringTests
{
	[Test]
	public async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<StringData>>();
		var data = await portal.CreateAsync();

		data.Contents = "ABC";

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (StringData)formatter.Deserialize(stream)!;

		await Assert.That(newData.Contents).IsEqualTo("ABC");
	}

	[Test]
	public async Task RoundtripWithNullableAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<StringData>>();
		var data = await portal.CreateAsync();

		data.Contents = "ABC";
		data.Contents = null!;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (StringData)formatter.Deserialize(stream)!;

		await Assert.That(newData.Contents).IsEqualTo(string.Empty);
	}
}