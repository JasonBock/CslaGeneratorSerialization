using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.Graphs;

[Serializable]
public abstract partial class AbstractData
	: BusinessBase<AbstractData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<string> CoreProperty =
		AbstractData.RegisterProperty<string>(_ => _.Core);
	public string Core
	{
		get => this.GetProperty(AbstractData.CoreProperty);
		set => this.SetProperty(AbstractData.CoreProperty, value);
	}
}

[Serializable]
public partial class BaseData
	: BusinessBase<BaseData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<string> CoreProperty =
		BaseData.RegisterProperty<string>(_ => _.Core);
	public string Core
	{
		get => this.GetProperty(BaseData.CoreProperty);
		set => this.SetProperty(BaseData.CoreProperty, value);
	}
}

[Serializable]
public sealed partial class DerivedFromAbstractData
	: AbstractData
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<int> CustomProperty =
		DerivedFromAbstractData.RegisterProperty<int>(typeof(DerivedFromAbstractData), new PropertyInfo<int>(nameof(DerivedFromAbstractData.Custom)));
	public int Custom
	{
		get => this.GetProperty(DerivedFromAbstractData.CustomProperty);
		set => this.SetProperty(DerivedFromAbstractData.CustomProperty, value);
	}
}

[Serializable]
public sealed partial class DerivedFromBaseData
	: BaseData
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<int> CustomProperty =
		DerivedFromBaseData.RegisterProperty<int>(typeof(DerivedFromBaseData), new PropertyInfo<int>(nameof(DerivedFromBaseData.Custom)));
	public int Custom
	{
		get => this.GetProperty(DerivedFromBaseData.CustomProperty);
		set => this.SetProperty(DerivedFromBaseData.CustomProperty, value);
	}
}

public static class InheritanceTests
{
	[Test]
	public static void RoundtripFromAbstractType()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>());
		var portal = provider.GetRequiredService<IDataPortal<DerivedFromAbstractData>>();
		var data = portal.Create();

		data.Core = "ABC";
		data.Custom = 3;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (DerivedFromAbstractData)formatter.Deserialize(stream);

		Assert.Multiple(() =>
		{
			Assert.That(newData.Core, Is.EqualTo("ABC"));
			Assert.That(newData.Custom, Is.EqualTo(3));
		});
	}

	[Test]
	public static void RoundtripFromBaseType()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>());
		var portal = provider.GetRequiredService<IDataPortal<DerivedFromBaseData>>();
		var data = portal.Create();

		data.Core = "ABC";
		data.Custom = 3;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (DerivedFromBaseData)formatter.Deserialize(stream);

		Assert.Multiple(() =>
		{
			Assert.That(newData.Core, Is.EqualTo("ABC"));
			Assert.That(newData.Custom, Is.EqualTo(3));
		});
	}
}