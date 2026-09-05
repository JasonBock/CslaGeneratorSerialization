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

	[CslaIgnoreProperty]
	public partial uint Value { get; set; }
}

[GeneratorSerializable]
public sealed partial class NoImplementStateData
	: BusinessBase<NoImplementStateData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<string> ContentsProperty =
		RegisterProperty<string>(nameof(NoImplementStateData.Contents));
	public string Contents
	{
		get => this.GetProperty(NoImplementStateData.ContentsProperty)!;
		set => this.SetProperty(NoImplementStateData.ContentsProperty, value);
	}

	public partial Guid Id { get; set; }
}

public sealed partial class StateData
	: BusinessBase<StateData>
{
#pragma warning disable IDE0032 // Use auto property
	private uint value;
#pragma warning restore IDE0032 // Use auto property
	public partial uint Value { get => this.value; set => this.value = value; }
}

public sealed partial class NoImplementStateData
	: BusinessBase<NoImplementStateData>
{
#pragma warning disable IDE0032 // Use auto property
	private Guid id;
#pragma warning restore IDE0032 // Use auto property
	public partial Guid Id { get => this.id; set => this.id = value; }
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
		data.Value = 42;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (StateData)formatter.Deserialize(stream)!;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(newData.Contents, Is.EqualTo(data.Contents));
			Assert.That(newData.Value, Is.Zero);
			Assert.That(newData.Id, Is.EqualTo(data.Id));
		}
	}

	[Test]
	public static async Task RoundtripNoImplementAsync()
	{
		var id = Guid.NewGuid();
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<NoImplementStateData>>();
		var data = await portal.CreateAsync();

		data.Contents = "Stuff";
		data.Id = id;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (NoImplementStateData)formatter.Deserialize(stream)!;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(newData.Contents, Is.EqualTo(data.Contents));
			Assert.That(newData.Id, Is.EqualTo(Guid.Empty));
		}
	}
}