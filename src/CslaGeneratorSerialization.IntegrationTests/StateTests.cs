using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

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

internal static class StateTests
{
	[Test]
	public static async Task CreateAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<StateData>>();
		var data = await portal.CreateAsync();

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (StateData)formatter.Deserialize(stream)!;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(newData.IsNew, Is.True);
			Assert.That(newData.IsDeleted, Is.False);
			Assert.That(newData.IsDirty, Is.True);
			Assert.That(newData.IsChild, Is.False);
		}
	}

	[Test]
	public static async Task FetchAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<StateData>>();
		var data = await portal.FetchAsync();

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (StateData)formatter.Deserialize(stream)!;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(newData.IsNew, Is.False);
			Assert.That(newData.IsDeleted, Is.False);
			Assert.That(newData.IsDirty, Is.False);
			Assert.That(newData.IsChild, Is.False);
		}
	}

	[Test]
	public static async Task ChangeStateOnCreateAsync()
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

		using (Assert.EnterMultipleScope())
		{
			Assert.That(newData.IsNew, Is.True);
			Assert.That(newData.IsDeleted, Is.False);
			Assert.That(newData.IsDirty, Is.True);
			Assert.That(newData.IsChild, Is.False);
		}
	}

	[Test]
	public static async Task ChangeStateOnFetchAsync()
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

		using (Assert.EnterMultipleScope())
		{
			Assert.That(newData.IsNew, Is.False);
			Assert.That(newData.IsDeleted, Is.False);
			Assert.That(newData.IsDirty, Is.True);
			Assert.That(newData.IsChild, Is.False);
		}
	}

	[Test]
	public static async Task DeleteAfterCreateAsync()
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

		using (Assert.EnterMultipleScope())
		{
			Assert.That(newData.IsNew, Is.True);
			Assert.That(newData.IsDeleted, Is.True);
			Assert.That(newData.IsDirty, Is.True);
			Assert.That(newData.IsChild, Is.False);
		}
	}

	[Test]
	public static async Task DeleteAfterFetchAsync()
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

		using (Assert.EnterMultipleScope())
		{
			Assert.That(newData.IsNew, Is.False);
			Assert.That(newData.IsDeleted, Is.True);
			Assert.That(newData.IsDirty, Is.True);
			Assert.That(newData.IsChild, Is.False);
		}
	}
}