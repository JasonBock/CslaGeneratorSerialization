using Csla;
using Microsoft.Extensions.DependencyInjection;

namespace CslaGeneratorSerialization.IntegrationTests.Graphs.DuplicateTestsDomain;

[GeneratorSerializable]
public sealed partial class Node
	: BusinessBase<Node>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<Node?> LeftProperty =
		RegisterProperty<Node?>(nameof(Node.Left));
	public Node? Left
	{
		get => this.GetProperty(Node.LeftProperty);
		set => this.SetProperty(Node.LeftProperty, value);
	}

	public static readonly PropertyInfo<Node?> RightProperty =
		RegisterProperty<Node?>(nameof(Node.Right));
	public Node? Right
	{
		get => this.GetProperty(Node.RightProperty);
		set => this.SetProperty(Node.RightProperty, value);
	}

	public static readonly PropertyInfo<string> NameProperty =
		RegisterProperty<string>(nameof(Node.Name));
	public string Name
	{
		get => this.GetProperty(Node.NameProperty)!;
		set => this.SetProperty(Node.NameProperty, value);
	}
}

public sealed class DuplicateTests
{
	[Test]
	public async Task RoundtripWhenDifferentAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<Node>>();
		var childPortal = provider.GetRequiredService<IChildDataPortal<Node>>();
		var data = await portal.CreateAsync();

		data.Left = await childPortal.CreateChildAsync();
		data.Left.Name = "Left";
		data.Right = await childPortal.CreateChildAsync();
		data.Right.Name = "Right";

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (Node)formatter.Deserialize(stream)!;

		using (Assert.Multiple())
		{
			await Assert.That(newData.Left!.Name).IsEqualTo("Left");
			await Assert.That(newData.Right!.Name).IsEqualTo("Right");
		}
	}

	[Test]
	public async Task RoundtripWhenRightIsSameAsLeftAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<Node>>();
		var childPortal = provider.GetRequiredService<IChildDataPortal<Node>>();
		var data = await portal.CreateAsync();

		data.Left = await childPortal.CreateChildAsync();
		data.Left.Name = "Left";
		data.Right = data.Left;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (Node)formatter.Deserialize(stream)!;

		using (Assert.Multiple())
		{
			await Assert.That(newData.Left!.Name).IsEqualTo("Left");
			await Assert.That(newData.Right!.Name).IsEqualTo("Left");
			await Assert.That(newData.Left).IsSameReferenceAs(newData.Right);
		}
	}

	[Test]
	public async Task RoundtripWhenLeftIsSameAsRightAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<Node>>();
		var childPortal = provider.GetRequiredService<IChildDataPortal<Node>>();
		var data = await portal.CreateAsync();

		data.Right = await childPortal.CreateChildAsync();
		data.Right.Name = "Left";
		data.Left = data.Right;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (Node)formatter.Deserialize(stream)!;

		using (Assert.Multiple())
		{
			await Assert.That(newData.Left!.Name).IsEqualTo("Left");
			await Assert.That(newData.Right!.Name).IsEqualTo("Left");
			await Assert.That(newData.Left).IsSameReferenceAs(newData.Right);
		}
	}
}