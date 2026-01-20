using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.Graphs;

enum TaskEnum
{
	First,
	Second,
	Third
}

[GeneratorSerializable]
public partial class CustomClassName<T> : BusinessBase<CustomClassName<T>>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<string> ContentProperty =
		RegisterProperty<string>(nameof(Content));
	public string Content
	{
		get => this.GetProperty(ContentProperty)!;
		set => this.SetProperty(ContentProperty, value);
	}
}

internal static class TypeClassNameTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<CustomClassName<TaskEnum>>>();
		var data = await portal.CreateAsync();

		data.Content = "Dummy";

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (CustomClassName<TaskEnum>)formatter.Deserialize(stream)!;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(newData.Content, Is.EqualTo("Dummy"));
		}
	}
}