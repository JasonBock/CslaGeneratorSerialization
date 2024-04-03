using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.Graphs.ReadOnlyTestsDomain;

[Serializable]
public sealed partial class Data
	: ReadOnlyBase<Data>
{
	[Create]
	private void Create()
	{
		this.StringContents = "123";
		this.Int32Contents = 123;
		this.ChildContents = this.ApplicationContext.GetRequiredService<IChildDataPortal<ChildData>>().CreateChild();
	}

	public static readonly PropertyInfo<string> StringContentsProperty =
		RegisterProperty<string>(_ => _.StringContents);
	public string StringContents
	{
		get => this.ReadProperty(Data.StringContentsProperty);
		private set => this.LoadProperty(Data.StringContentsProperty, value);
	}

	public static readonly PropertyInfo<ChildData> ChildContentsProperty =
		RegisterProperty<ChildData>(_ => _.ChildContents);
	public ChildData ChildContents
	{
		get => this.ReadProperty(Data.ChildContentsProperty);
		private set => this.LoadProperty(Data.ChildContentsProperty, value);
	}

	public static readonly PropertyInfo<int> Int32ContentsProperty =
		RegisterProperty<int>(_ => _.Int32Contents);
	public int Int32Contents
	{
		get => this.ReadProperty(Data.Int32ContentsProperty);
		private set => this.LoadProperty(Data.Int32ContentsProperty, value);
	}
}

[Serializable]
public sealed partial class ChildData
	: ReadOnlyBase<ChildData>
{
	[CreateChild]
	private void CreateChild() =>
		this.Value = "Child 123";

	public static readonly PropertyInfo<string> ValueProperty =
		RegisterProperty<string>(_ => _.Value);
	public string Value
	{
		get => this.ReadProperty(ChildData.ValueProperty);
		private set => this.LoadProperty(ChildData.ValueProperty, value);
	}
}

public static class ReadOnlyTests
{
	[Test]
	public static void Roundtrip()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>());
		var portal = provider.GetRequiredService<IDataPortal<Data>>();
		var data = portal.Create();

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (Data)formatter.Deserialize(stream);

		Assert.Multiple(() =>
		{
			Assert.That(newData.StringContents, Is.EqualTo("123"));
			Assert.That(newData.Int32Contents, Is.EqualTo(123));
			Assert.That(newData.ChildContents.Value, Is.EqualTo("Child 123"));
		});
	}
}