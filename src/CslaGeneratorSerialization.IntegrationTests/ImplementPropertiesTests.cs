using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.ImplementPropertiesDomain;

[GeneratorSerializable]
[CslaImplementProperties]
public sealed partial class StateData
	: BusinessBase<StateData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<string> ContentsProperty =
		RegisterProperty<string>(nameof(StateData.Contents));
	public string Contents
	{
		get => this.GetProperty(StateData.ContentsProperty)!;
		set => this.SetProperty(StateData.ContentsProperty, value);
	}

	public partial Guid Id { get; set; }
}

internal static class ImplementPropertiesTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var id = Guid.NewGuid();
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<StateData>>();
		var data = await portal.CreateAsync();

		data.Contents = "Stuff";
		data.Id = id;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (StateData)formatter.Deserialize(stream)!;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(newData.Contents, Is.EqualTo(data.Contents));
			Assert.That(newData.Id, Is.EqualTo(data.Id));
		}
	}
}