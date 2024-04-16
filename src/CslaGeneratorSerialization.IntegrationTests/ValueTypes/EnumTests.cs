using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.ValueTypes.EnumTestsDomain;

public enum States { First, Second, Third }

[GeneratorSerializable]
public sealed partial class EnumData
	: BusinessBase<EnumData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<States> ContentsProperty =
		RegisterProperty<States>(_ => _.Contents);
	public States Contents
	{
		get => this.GetProperty(EnumData.ContentsProperty);
		set => this.SetProperty(EnumData.ContentsProperty, value);
	}
}

[GeneratorSerializable]
public sealed partial class EnumNullableData
	: BusinessBase<EnumNullableData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<States?> ContentsProperty =
		RegisterProperty<States?>(_ => _.Contents);
	public States? Contents
	{
		get => this.GetProperty(EnumNullableData.ContentsProperty);
		set => this.SetProperty(EnumNullableData.ContentsProperty, value);
	}
}

public static class EnumTests
{
	[Test]
	public static void Roundtrip()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<EnumData>>();
		var data = portal.Create();

		data.Contents = States.Second;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (EnumData)formatter.Deserialize(stream);

		Assert.That(newData.Contents, Is.EqualTo(States.Second));
	}

	[Test]
	public static void RoundtripWithNullable()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<EnumNullableData>>();
		var data = portal.Create();

		data.Contents = States.Second;
		data.Contents = null!;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (EnumNullableData)formatter.Deserialize(stream);

		Assert.That(newData.Contents, Is.Null);
	}
}