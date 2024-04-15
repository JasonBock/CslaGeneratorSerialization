using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.Graphs.DuplicateTestsDomain;

[Serializable]
public sealed partial class Node
	: BusinessBase<Node>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<Node?> LeftProperty =
		RegisterProperty<Node?>(_ => _.Left);
	public Node? Left
	{
		get => this.GetProperty(Node.LeftProperty);
		set => this.SetProperty(Node.LeftProperty, value);
	}

	public static readonly PropertyInfo<Node?> RightProperty =
		RegisterProperty<Node?>(_ => _.Right);
	public Node? Right
	{
		get => this.GetProperty(Node.RightProperty);
		set => this.SetProperty(Node.RightProperty, value);
	}

	public static readonly PropertyInfo<string> NameProperty =
		RegisterProperty<string>(_ => _.Name);
	public string Name
	{
		get => this.GetProperty(Node.NameProperty);
		set => this.SetProperty(Node.NameProperty, value);
	}
}

public static class DuplicateTests
{
	[Test]
	public static void RoundtripWhenDifferent()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<Node>>();
		var childPortal = provider.GetRequiredService<IChildDataPortal<Node>>();
		var data = portal.Create();

		data.Left = childPortal.CreateChild();
		data.Left.Name = "Left";
		data.Right = childPortal.CreateChild();
		data.Right.Name = "Right";

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (Node)formatter.Deserialize(stream);

		Assert.Multiple(() =>
		{
			Assert.That(newData.Left!.Name, Is.EqualTo("Left"));
			Assert.That(newData.Right!.Name, Is.EqualTo("Right"));
		});
	}

	[Test]
	public static void RoundtripWhenRightIsSameAsLeft()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<Node>>();
		var childPortal = provider.GetRequiredService<IChildDataPortal<Node>>();
		var data = portal.Create();

		data.Left = childPortal.CreateChild();
		data.Left.Name = "Left";
		data.Right = data.Left;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (Node)formatter.Deserialize(stream);

		Assert.Multiple(() =>
		{
			Assert.That(newData.Left!.Name, Is.EqualTo("Left"));
			Assert.That(newData.Right!.Name, Is.EqualTo("Left"));
			Assert.That(newData.Left, Is.SameAs(newData.Right));
		});
	}

	[Test]
	public static void RoundtripWhenLeftIsSameAsRight()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<Node>>();
		var childPortal = provider.GetRequiredService<IChildDataPortal<Node>>();
		var data = portal.Create();

		data.Right = childPortal.CreateChild();
		data.Right.Name = "Left";
		data.Left = data.Right;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (Node)formatter.Deserialize(stream);

		Assert.Multiple(() =>
		{
			Assert.That(newData.Left!.Name, Is.EqualTo("Left"));
			Assert.That(newData.Right!.Name, Is.EqualTo("Left"));
			Assert.That(newData.Left, Is.SameAs(newData.Right));
		});
	}
}