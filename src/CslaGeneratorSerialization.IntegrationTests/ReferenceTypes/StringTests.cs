using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.ReferenceTypes;

[Serializable]
public sealed partial class StringData
	: BusinessBase<StringData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<string> ContentsProperty =
		RegisterProperty<string>(_ => _.Contents);
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
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>());
		var portal = provider.GetRequiredService<IDataPortal<StringData>>();
		var data = portal.Create();

		data.Contents = "ABC";

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (StringData)formatter.Deserialize(stream);

		Assert.Multiple(() =>
		{
			Assert.That(newData.Contents, Is.EqualTo(data.Contents));
			Assert.That(newData.IsNew, Is.EqualTo(data.IsNew));
			Assert.That(newData.IsDeleted, Is.EqualTo(data.IsDeleted));
			Assert.That(newData.IsDirty, Is.EqualTo(data.IsDirty));
			Assert.That(newData.IsChild, Is.EqualTo(data.IsChild));
		});
	}

	[Test]
	public static void RoundtripWithNullable()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>());
		var portal = provider.GetRequiredService<IDataPortal<StringData>>();
		var data = portal.Create();

		data.Contents = "ABC";
		data.Contents = null!;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (StringData)formatter.Deserialize(stream);

		Assert.Multiple(() =>
		{
			Assert.That(newData.Contents, Is.EqualTo(string.Empty));
			Assert.That(newData.IsNew, Is.EqualTo(data.IsNew));
			Assert.That(newData.IsDeleted, Is.EqualTo(data.IsDeleted));
			Assert.That(newData.IsDirty, Is.EqualTo(data.IsDirty));
			Assert.That(newData.IsChild, Is.EqualTo(data.IsChild));
		});
	}
}