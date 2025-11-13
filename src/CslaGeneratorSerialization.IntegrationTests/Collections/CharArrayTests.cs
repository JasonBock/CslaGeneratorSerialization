using Csla;
using Microsoft.Extensions.DependencyInjection;

namespace CslaGeneratorSerialization.IntegrationTests.Collections.CharArrayTestsDomain;

[GeneratorSerializable]
public sealed partial class CharArrayData
	: BusinessBase<CharArrayData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<char[]> ContentsProperty =
		RegisterProperty<char[]>(nameof(CharArrayData.Contents));
#pragma warning disable CA1819 // Properties should not return arrays
	public char[] Contents
#pragma warning restore CA1819 // Properties should not return arrays
	{
		get => this.GetProperty(CharArrayData.ContentsProperty)!;
		set => this.SetProperty(CharArrayData.ContentsProperty, value);
	}
}

public sealed class CharArrayTests
{
	[Test]
	public async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<CharArrayData>>();
		var data = await portal.CreateAsync();

		data.Contents = ['a', 'b', 'c'];

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (CharArrayData)formatter.Deserialize(stream)!;

		await Assert.That(newData.Contents).IsEquivalentTo(new[] { 'a', 'b', 'c' }));
	}

	[Test]
	public async Task RoundtripWithNullableAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<CharArrayData>>();
		var data = await portal.CreateAsync();

		data.Contents = ['a', 'b', 'c'];
		data.Contents = null!;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (CharArrayData)formatter.Deserialize(stream)!;

		await Assert.That(newData.Contents).IsNull();
	}
}