using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.Collections.CharArrayTestsDomain;

[Serializable]
public sealed partial class CharArrayData
	: BusinessBase<CharArrayData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<char[]> ContentsProperty =
		RegisterProperty<char[]>(_ => _.Contents);
#pragma warning disable CA1819 // Properties should not return arrays
	public char[] Contents
#pragma warning restore CA1819 // Properties should not return arrays
	{
		get => this.GetProperty(CharArrayData.ContentsProperty);
		set => this.SetProperty(CharArrayData.ContentsProperty, value);
	}
}

public static class CharArrayTests
{
	[Test]
	public static void Roundtrip()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<CharArrayData>>();
		var data = portal.Create();

		data.Contents = ['a', 'b', 'c'];

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (CharArrayData)formatter.Deserialize(stream);

		CollectionAssert.AreEqual(newData.Contents, new char[] { 'a', 'b', 'c' });
	}

	[Test]
	public static void RoundtripWithNullable()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<CharArrayData>>();
		var data = portal.Create();

		data.Contents = ['a', 'b', 'c'];
		data.Contents = null!;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (CharArrayData)formatter.Deserialize(stream);

		Assert.That(newData.Contents, Is.Null);
	}
}