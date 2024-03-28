using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.ReferenceTypes;

[Serializable]
public sealed partial class ParentData
	: BusinessBase<ParentData>
{
	[Create]
	private void Create() =>
		this.Contents = this.ApplicationContext.GetRequiredService<IChildDataPortal<ChildData>>().CreateChild();

	public static readonly PropertyInfo<ChildData> ContentsProperty =
		RegisterProperty<ChildData>(_ => _.Contents);
	public ChildData Contents
	{
		get => this.GetProperty(ParentData.ContentsProperty);
		set => this.SetProperty(ParentData.ContentsProperty, value);
	}
}

[Serializable]
public sealed partial class ChildData
	: BusinessBase<ChildData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<string> ChildContentsProperty =
		RegisterProperty<string>(_ => _.ChildContents);
	public string ChildContents
	{
		get => this.GetProperty(ChildData.ChildContentsProperty);
		set => this.SetProperty(ChildData.ChildContentsProperty, value);
	}
}

public static class ChildBusinessObjectTests
{
	[Test]
	public static void Roundtrip()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>());
		var portal = provider.GetRequiredService<IDataPortal<ParentData>>();
		var data = portal.Create();

		data.Contents.ChildContents = "ABC";

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (ParentData)formatter.Deserialize(stream);

		Assert.Multiple(() =>
		{
			Assert.That(newData.Contents.ChildContents, Is.EqualTo(data.Contents.ChildContents));
			Assert.That(newData.IsNew, Is.EqualTo(data.IsNew));
			Assert.That(newData.IsDeleted, Is.EqualTo(data.IsDeleted));
			Assert.That(newData.IsDirty, Is.EqualTo(data.IsDirty));
			Assert.That(newData.IsChild, Is.EqualTo(data.IsChild));
			Assert.That(newData.Contents.IsNew, Is.EqualTo(data.Contents.IsNew));
			Assert.That(newData.Contents.IsDeleted, Is.EqualTo(data.Contents.IsDeleted));
			Assert.That(newData.Contents.IsDirty, Is.EqualTo(data.Contents.IsDirty));
			Assert.That(newData.Contents.IsChild, Is.EqualTo(data.Contents.IsChild));
		});
	}

	[Test]
	public static void RoundtripWithNullable()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>());
		var portal = provider.GetRequiredService<IDataPortal<ParentData>>();
		var data = portal.Create();

		data.Contents.ChildContents = "ABC";
		data.Contents = null!;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (ParentData)formatter.Deserialize(stream);

		Assert.That(newData.Contents, Is.Null);

		Assert.Multiple(() =>
		{
			Assert.That(newData.Contents, Is.Null);
			Assert.That(newData.IsNew, Is.EqualTo(data.IsNew));
			Assert.That(newData.IsDeleted, Is.EqualTo(data.IsDeleted));
			Assert.That(newData.IsDirty, Is.EqualTo(data.IsDirty));
			Assert.That(newData.IsChild, Is.EqualTo(data.IsChild));
		});
	}
}