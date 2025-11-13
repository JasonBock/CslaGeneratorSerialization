using Csla;
using Microsoft.Extensions.DependencyInjection;

namespace CslaGeneratorSerialization.IntegrationTests.StateTestsDomain;

[GeneratorSerializable]
public sealed partial class StateData
	: BusinessBase<StateData>
{
	[Create]
	private void Create() { }

	[Fetch]
	private void Fetch() { }

	public static readonly PropertyInfo<string> ContentsProperty =
		RegisterProperty<string>(nameof(StateData.Contents));
	public string Contents
	{
		get => this.GetProperty(StateData.ContentsProperty)!;
		set => this.SetProperty(StateData.ContentsProperty, value);
	}
}

public sealed class StateTests
{
	[Test]
	public async Task CreateAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<StateData>>();
		var data = await portal.CreateAsync();

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (StateData)formatter.Deserialize(stream)!;

		using (Assert.Multiple())
		{
			await Assert.That(newData.IsNew).IsTrue();
			await Assert.That(newData.IsDeleted).IsFalse();
			await Assert.That(newData.IsDirty).IsTrue();
			await Assert.That(newData.IsChild).IsFalse();
		}
	}

	[Test]
	public async Task FetchAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<StateData>>();
		var data = await portal.FetchAsync();

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (StateData)formatter.Deserialize(stream)!;

		using (Assert.Multiple())
		{
			await Assert.That(newData.IsNew).IsFalse();
			await Assert.That(newData.IsDeleted).IsFalse();
			await Assert.That(newData.IsDirty).IsFalse();
			await Assert.That(newData.IsChild).IsFalse();
		}
	}

	[Test]
	public async Task ChangeStateOnCreateAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<StateData>>();
		var data = await portal.CreateAsync();

		data.Contents = "ABC";

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (StateData)formatter.Deserialize(stream)!;

		using (Assert.Multiple())
		{
			await Assert.That(newData.IsNew).IsTrue();
			await Assert.That(newData.IsDeleted).IsFalse();
			await Assert.That(newData.IsDirty).IsTrue();
			await Assert.That(newData.IsChild).IsFalse();
		}
	}

	[Test]
	public async Task ChangeStateOnFetchAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<StateData>>();
		var data = await portal.FetchAsync();

		data.Contents = "ABC";

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (StateData)formatter.Deserialize(stream)!;

		using (Assert.Multiple())
		{
			await Assert.That(newData.IsNew).IsFalse();
			await Assert.That(newData.IsDeleted).IsFalse();
			await Assert.That(newData.IsDirty).IsTrue();
			await Assert.That(newData.IsChild).IsFalse();
		}
	}

	[Test]
	public async Task DeleteAfterCreateAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<StateData>>();
		var data = await portal.CreateAsync();

		data.Contents = "ABC";
		data.Delete();

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (StateData)formatter.Deserialize(stream)!;

		using (Assert.Multiple())
		{
			await Assert.That(newData.IsNew).IsTrue();
			await Assert.That(newData.IsDeleted).IsTrue();
			await Assert.That(newData.IsDirty).IsTrue();
			await Assert.That(newData.IsChild).IsFalse();
		}
	}

	[Test]
	public async Task DeleteAfterFetchAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<StateData>>();
		var data = await portal.FetchAsync();

		data.Contents = "ABC";
		data.Delete();

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (StateData)formatter.Deserialize(stream)!;

		using (Assert.Multiple())
		{
			await Assert.That(newData.IsNew).IsFalse();
			await Assert.That(newData.IsDeleted).IsTrue();
			await Assert.That(newData.IsDirty).IsTrue();
			await Assert.That(newData.IsChild).IsFalse();
		}
	}
}