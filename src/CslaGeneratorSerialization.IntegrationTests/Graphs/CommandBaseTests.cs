using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.Graphs.CommandBaseTestsDomain;

[GeneratorSerializable]
public sealed partial class Operation
	: CommandBase<Operation>
{
	[Create]
	private void Create() { }

	[Execute]
	private void Execute() { }

	public static readonly PropertyInfo<string> StringContentsProperty =
		RegisterProperty<string>(nameof(Operation.StringContents));
	public string StringContents
	{
		get => this.ReadProperty(Operation.StringContentsProperty);
		set => this.LoadProperty(Operation.StringContentsProperty, value);
	}

	public static readonly PropertyInfo<int> Int32ContentsProperty =
		RegisterProperty<int>(nameof(Operation.Int32Contents));
	public int Int32Contents
	{
		get => this.ReadProperty(Operation.Int32ContentsProperty);
		set => this.LoadProperty(Operation.Int32ContentsProperty, value);
	}
}

public static class CommandBaseTestsTests
{
	[Test]
	public static void Roundtrip()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<Operation>>();
		var data = portal.Create();

		data.Int32Contents = 3;
		data.StringContents = "4";

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (Operation)formatter.Deserialize(stream)!;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(newData.Int32Contents, Is.EqualTo(3));
			Assert.That(newData.StringContents, Is.EqualTo("4"));
		}
	}
}