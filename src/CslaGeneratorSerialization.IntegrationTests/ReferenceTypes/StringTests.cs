using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

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
		get => this.GetProperty(StringData.ContentsProperty);
		set => this.SetProperty(StringData.ContentsProperty, value);
	}
}

public static class StringTests
{
	[Test]
	public static void Roundtrip()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<StringData>>();
		var data = portal.Create();

		data.Contents = "ABC";

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (StringData)formatter.Deserialize(stream);

		Assert.That(newData.Contents, Is.EqualTo("ABC"));
	}

	[Test]
	public static void RoundtripWithNullable()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<StringData>>();
		var data = portal.Create();

		data.Contents = "ABC";
		data.Contents = null!;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (StringData)formatter.Deserialize(stream);

		Assert.That(newData.Contents, Is.EqualTo(string.Empty));
	}
}