using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.Graphs;

[GeneratorSerializable]
public sealed partial class Customization
	: BusinessBase<Customization>, IGeneratorSerializableCustomization
{
	[Create]
	private void Create() { }

	[Fetch]
	private void Fetch() { }

	public void GetCustomState(BinaryReader reader)
	{
		ArgumentNullException.ThrowIfNull(reader);
		this.Custom = reader.ReadInt32();
	}

	public void SetCustomState(BinaryWriter writer)
	{
		ArgumentNullException.ThrowIfNull(writer);
		writer.Write(this.Custom);
	}

	public static readonly PropertyInfo<string> ContentsProperty =
		RegisterProperty<string>(_ => _.Contents);
	public string Contents
	{
		get => this.GetProperty(Customization.ContentsProperty);
		set => this.SetProperty(Customization.ContentsProperty, value);
	}

	public int Custom { get; set; }
}


public static class CustomizationTests
{
	[Test]
	public static void ChangeStateOnCreate()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<Customization>>();
		var data = portal.Create();

		data.Custom = 33;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (Customization)formatter.Deserialize(stream);

		Assert.That(newData.Custom, Is.EqualTo(33));
	}
}
