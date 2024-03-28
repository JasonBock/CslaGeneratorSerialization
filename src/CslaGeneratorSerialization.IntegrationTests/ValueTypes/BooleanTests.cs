using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.ValueTypes;

[Serializable]
public sealed partial class BooleanData
	: BusinessBase<BooleanData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<bool> ContentsProperty =
		RegisterProperty<bool>(_ => _.Contents);
	public bool Contents
	{
		get => this.GetProperty(BooleanData.ContentsProperty);
		set => this.SetProperty(BooleanData.ContentsProperty, value);
	}
}

[Serializable]
public sealed partial class BooleanNullableData
	: BusinessBase<BooleanNullableData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<bool?> ContentsProperty =
		RegisterProperty<bool?>(_ => _.Contents);
	public bool? Contents
	{
		get => this.GetProperty(BooleanNullableData.ContentsProperty);
		set => this.SetProperty(BooleanNullableData.ContentsProperty, value);
	}
}

public static class BooleanTests
{
	[Test]
	public static void Roundtrip()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>());
		var portal = provider.GetRequiredService<IDataPortal<BooleanData>>();
		var data = portal.Create();

		data.Contents = true;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (BooleanData)formatter.Deserialize(stream);

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
		var portal = provider.GetRequiredService<IDataPortal<BooleanNullableData>>();
		var data = portal.Create();

		data.Contents = true;
		data.Contents = null;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (BooleanNullableData)formatter.Deserialize(stream);

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