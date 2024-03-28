using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.Collections;

[Serializable]
public sealed partial class ListOfInt32Data
	: BusinessBase<ListOfInt32Data>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<List<int>> ContentsProperty =
		RegisterProperty<List<int>>(_ => _.Contents);
#pragma warning disable CA2227 // Collection properties should be read only
   public List<int> Contents
#pragma warning restore CA2227 // Collection properties should be read only
	{
		get => this.GetProperty(ListOfInt32Data.ContentsProperty);
		set => this.SetProperty(ListOfInt32Data.ContentsProperty, value);
	}
}

public static class ListOfInt32Tests
{
	[Test]
	public static void Roundtrip()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>());
		var portal = provider.GetRequiredService<IDataPortal<ListOfInt32Data>>();
		var data = portal.Create();

		data.Contents = [1, 2, 3];

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (ListOfInt32Data)formatter.Deserialize(stream);

		CollectionAssert.AreEqual(newData.Contents, new int[] { 1, 2, 3 });
	}

	[Test]
	public static void RoundtripWithNullable()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>());
		var portal = provider.GetRequiredService<IDataPortal<ListOfInt32Data>>();
		var data = portal.Create();

		data.Contents = [1, 2, 3];
		data.Contents = null!;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (ListOfInt32Data)formatter.Deserialize(stream);

		Assert.That(newData.Contents, Is.Null);
	}
}