using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.Graphs.SealedTestsDomain;

[GeneratorSerializable]
public partial class BaseData
	: BusinessBase<BaseData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<string> CoreProperty =
		BaseData.RegisterProperty<string>(nameof(BaseData.Core));
	public string Core
	{
		get => this.GetProperty(BaseData.CoreProperty)!;
		set => this.SetProperty(BaseData.CoreProperty, value);
	}
}

[GeneratorSerializable]
public sealed partial class DerivedData
	: BaseData
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<int> CustomProperty =
		DerivedData.RegisterProperty<int>(typeof(DerivedData), new PropertyInfo<int>(nameof(DerivedData.Custom)));
	public int Custom
	{
		get => this.GetProperty(DerivedData.CustomProperty);
		set => this.SetProperty(DerivedData.CustomProperty, value);
	}
}

[GeneratorSerializable]
public sealed partial class ConsumingData
	: BusinessBase<ConsumingData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<BaseData> ContentsProperty =
		RegisterProperty<BaseData>(nameof(ConsumingData.Contents));
	public BaseData Contents
	{
		get => this.GetProperty(ConsumingData.ContentsProperty)!;
		set => this.SetProperty(ConsumingData.ContentsProperty, value);
	}
}

internal static class SealedTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<ConsumingData>>();
		var childPortal = provider.GetRequiredService<IChildDataPortal<DerivedData>>();
		var data = await portal.CreateAsync();

		var childData = await childPortal.CreateChildAsync();
		childData.Core = "ABC";
		childData.Custom = 3;
		data.Contents = childData;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (ConsumingData)formatter.Deserialize(stream)!;

		using (Assert.EnterMultipleScope())
		{
			var dataProperty = (DerivedData)newData.Contents;
			Assert.That(dataProperty.Core, Is.EqualTo("ABC"));
			Assert.That(dataProperty.Custom, Is.EqualTo(3));
		}
	}
}