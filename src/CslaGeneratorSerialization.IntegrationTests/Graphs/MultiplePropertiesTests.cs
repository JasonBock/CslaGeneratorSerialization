using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.Graphs.MultiplePropertiesTestsDomain;

[Serializable]
public sealed partial class ParentPropertiesData
	: BusinessBase<ParentPropertiesData>
{
	[Create]
	private void Create() =>
		this.ChildContents = this.ApplicationContext.GetRequiredService<IChildDataPortal<ChildPropertiesData>>().CreateChild();

	public static readonly PropertyInfo<string> StringContentsProperty =
		RegisterProperty<string>(_ => _.StringContents);
	public string StringContents
   {
	  get => this.GetProperty(ParentPropertiesData.StringContentsProperty);
		set => this.SetProperty(ParentPropertiesData.StringContentsProperty, value);
	}

	public static readonly PropertyInfo<ChildPropertiesData> ChildContentsProperty =
		RegisterProperty<ChildPropertiesData>(_ => _.ChildContents);
	public ChildPropertiesData ChildContents
   {
		get => this.GetProperty(ParentPropertiesData.ChildContentsProperty);
		set => this.SetProperty(ParentPropertiesData.ChildContentsProperty, value);
	}

	public static readonly PropertyInfo<int> Int32ContentsProperty =
		RegisterProperty<int>(_ => _.Int32Contents);
	public int Int32Contents
   {
		get => this.GetProperty(ParentPropertiesData.Int32ContentsProperty);
		set => this.SetProperty(ParentPropertiesData.Int32ContentsProperty, value);
	}
}

[Serializable]
public sealed partial class ChildPropertiesData
	: BusinessBase<ChildPropertiesData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<string> ValueProperty =
		RegisterProperty<string>(_ => _.Value);
	public string Value
	{
		get => this.GetProperty(ChildPropertiesData.ValueProperty);
		set => this.SetProperty(ChildPropertiesData.ValueProperty, value);
	}
}

public static class MultiplePropertiesTests
{
	[Test]
	public static void Roundtrip()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>());
		var portal = provider.GetRequiredService<IDataPortal<ParentPropertiesData>>();
		var data = portal.Create();

		data.Int32Contents = 3;
		data.StringContents = "4";
		data.ChildContents.Value = "ABC";

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (ParentPropertiesData)formatter.Deserialize(stream);

		Assert.Multiple(() =>
		{
			Assert.That(newData.Int32Contents, Is.EqualTo(3));
			Assert.That(newData.StringContents, Is.EqualTo("4"));
			Assert.That(newData.ChildContents.Value, Is.EqualTo("ABC"));
		});
	}

	[Test]
	public static void RoundtripWithNullable()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>());
		var portal = provider.GetRequiredService<IDataPortal<ParentPropertiesData>>();
		var data = portal.Create();

		data.StringContents = "4";
		data.ChildContents.Value = "ABC";
		data.StringContents = null!;
		data.ChildContents = null!;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (ParentPropertiesData)formatter.Deserialize(stream);

		Assert.Multiple(() => 
		{
			Assert.That(newData.StringContents, Is.EqualTo(string.Empty));
			Assert.That(newData.ChildContents, Is.Null);
		});
	}
}