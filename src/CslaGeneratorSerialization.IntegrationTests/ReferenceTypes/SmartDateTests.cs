using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.ReferenceTypes.SmartDateTestsDomain;

[GeneratorSerializable]
public sealed partial class SmartDateData
	: BusinessBase<SmartDateData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<SmartDate> ContentsProperty =
		RegisterProperty<SmartDate>(nameof(SmartDateData.Contents));
	public SmartDate Contents
	{
		get => this.GetProperty(SmartDateData.ContentsProperty);
		set => this.SetProperty(SmartDateData.ContentsProperty, value);
	}
}

[GeneratorSerializable]
public sealed partial class SmartDateNullableData
	: BusinessBase<SmartDateNullableData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<SmartDate?> ContentsProperty =
		RegisterProperty<SmartDate?>(nameof(SmartDateNullableData.Contents));
	public SmartDate? Contents
	{
		get => this.GetProperty(SmartDateNullableData.ContentsProperty);
		set => this.SetProperty(SmartDateNullableData.ContentsProperty, value);
	}
}

internal static class SmartDateTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<SmartDateData>>();
		var data = await portal.CreateAsync();
		
		data.Contents = new SmartDate(DateTime.Now.Date);

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (SmartDateData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.EqualTo(new SmartDate(DateTime.Now.Date)));
		Assert.That(newData.Contents, Has.Property("IsEmpty").False);
	}

	[Test]
	public static async Task Roundtrip_Empty_Async()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<SmartDateData>>();
		var data = await portal.CreateAsync();

		data.Contents = new SmartDate(true);

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (SmartDateData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Has.Property("IsEmpty").True);
	}

	[Test]
	public static async Task RoundtripWithNullableAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<SmartDateNullableData>>();
		var data = await portal.CreateAsync();

		data.Contents = new SmartDate(DateTime.Now.Date);
		data.Contents = null!;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (SmartDateNullableData)formatter.Deserialize(stream)!;

		Assert.That(newData.Contents, Is.Null);
	}
}