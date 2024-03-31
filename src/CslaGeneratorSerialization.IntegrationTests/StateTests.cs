using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.StateTestsDomain;

[Serializable]
public sealed partial class StateData
	: BusinessBase<StateData>
{
	[Create]
	private void Create() { }

	[Fetch]
	private void Fetch() { }

	public static readonly PropertyInfo<string> ContentsProperty =
		RegisterProperty<string>(_ => _.Contents);
	public string Contents
	{
		get => this.GetProperty(StateData.ContentsProperty);
		set => this.SetProperty(StateData.ContentsProperty, value);
	}
}

public static class StateTests
{
	[Test]
	public static void Create()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>());
		var portal = provider.GetRequiredService<IDataPortal<StateData>>();
		var data = portal.Create();

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (StateData)formatter.Deserialize(stream);

		Assert.Multiple(() =>
		{
			Assert.That(newData.IsNew, Is.True, nameof(newData.IsNew));
			Assert.That(newData.IsDeleted, Is.False, nameof(newData.IsDeleted));
			Assert.That(newData.IsDirty, Is.True, nameof(newData.IsDirty));
			Assert.That(newData.IsChild, Is.False, nameof(newData.IsChild));
		});
	}

	[Test]
	public static void Fetch()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>());
		var portal = provider.GetRequiredService<IDataPortal<StateData>>();
		var data = portal.Fetch();

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (StateData)formatter.Deserialize(stream);

		Assert.Multiple(() =>
		{
			Assert.That(newData.IsNew, Is.False);
			Assert.That(newData.IsDeleted, Is.False);
			Assert.That(newData.IsDirty, Is.False);
			Assert.That(newData.IsChild, Is.False);
		});
	}

	[Test]
	public static void ChangeStateOnCreate()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>());
		var portal = provider.GetRequiredService<IDataPortal<StateData>>();
		var data = portal.Create();

		data.Contents = "ABC";

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (StateData)formatter.Deserialize(stream);

		Assert.Multiple(() =>
		{
			Assert.That(newData.IsNew, Is.True);
			Assert.That(newData.IsDeleted, Is.False);
			Assert.That(newData.IsDirty, Is.True);
			Assert.That(newData.IsChild, Is.False);
		});
	}

	[Test]
	public static void ChangeStateOnFetch()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>());
		var portal = provider.GetRequiredService<IDataPortal<StateData>>();
		var data = portal.Fetch();

		data.Contents = "ABC";

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (StateData)formatter.Deserialize(stream);

		Assert.Multiple(() =>
		{
			Assert.That(newData.IsNew, Is.False);
			Assert.That(newData.IsDeleted, Is.False);
			Assert.That(newData.IsDirty, Is.True);
			Assert.That(newData.IsChild, Is.False);
		});
	}

	[Test]
	public static void DeleteAfterCreate()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>());
		var portal = provider.GetRequiredService<IDataPortal<StateData>>();
		var data = portal.Create();

		data.Contents = "ABC";
		data.Delete();

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (StateData)formatter.Deserialize(stream);

		Assert.Multiple(() =>
		{
			Assert.That(newData.IsNew, Is.True);
			Assert.That(newData.IsDeleted, Is.True);
			Assert.That(newData.IsDirty, Is.True);
			Assert.That(newData.IsChild, Is.False);
		});
	}

	[Test]
	public static void DeleteAfterFetch()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>());
		var portal = provider.GetRequiredService<IDataPortal<StateData>>();
		var data = portal.Fetch();

		data.Contents = "ABC";
		data.Delete();

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (StateData)formatter.Deserialize(stream);

		Assert.Multiple(() =>
		{
			Assert.That(newData.IsNew, Is.False);
			Assert.That(newData.IsDeleted, Is.True);
			Assert.That(newData.IsDirty, Is.True);
			Assert.That(newData.IsChild, Is.False);
		});
	}
}