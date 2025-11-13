using Csla;
using Microsoft.Extensions.DependencyInjection;

namespace CslaGeneratorSerialization.IntegrationTests.Graphs.MultiplePropertiesTestsDomain;

[GeneratorSerializable]
public sealed partial class ParentPropertiesData
	: BusinessBase<ParentPropertiesData>
{
	[Create]
	private void Create() =>
		this.ChildContents = this.ApplicationContext.GetRequiredService<IChildDataPortal<ChildPropertiesData>>().CreateChild();

	public static readonly PropertyInfo<string> StringContentsProperty =
		RegisterProperty<string>(nameof(ParentPropertiesData.StringContents));
	public string StringContents
	{
		get => this.GetProperty(ParentPropertiesData.StringContentsProperty)!;
		set => this.SetProperty(ParentPropertiesData.StringContentsProperty, value);
	}

	public static readonly PropertyInfo<ChildPropertiesData> ChildContentsProperty =
		RegisterProperty<ChildPropertiesData>(nameof(ParentPropertiesData.ChildContents));
	public ChildPropertiesData ChildContents
	{
		get => this.GetProperty(ParentPropertiesData.ChildContentsProperty)!;
		set => this.SetProperty(ParentPropertiesData.ChildContentsProperty, value);
	}

	public static readonly PropertyInfo<int> Int32ContentsProperty =
		RegisterProperty<int>(nameof(ParentPropertiesData.Int32Contents));
	public int Int32Contents
	{
		get => this.GetProperty(ParentPropertiesData.Int32ContentsProperty);
		set => this.SetProperty(ParentPropertiesData.Int32ContentsProperty, value);
	}
}

[GeneratorSerializable]
public sealed partial class ChildPropertiesData
	: BusinessBase<ChildPropertiesData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<string> ValueProperty =
		RegisterProperty<string>(nameof(ChildPropertiesData.Value));
	public string Value
	{
		get => this.GetProperty(ChildPropertiesData.ValueProperty)!;
		set => this.SetProperty(ChildPropertiesData.ValueProperty, value);
	}
}

public sealed class MultiplePropertiesTests
{
	[Test]
	public async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<ParentPropertiesData>>();
		var data = await portal.CreateAsync();

		data.Int32Contents = 3;
		data.StringContents = "4";
		data.ChildContents.Value = "ABC";

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (ParentPropertiesData)formatter.Deserialize(stream)!;

		using (Assert.Multiple())
		{
			await Assert.That(newData.Int32Contents).IsEqualTo(3);
			await Assert.That(newData.StringContents).IsEqualTo("4");
			await Assert.That(newData.ChildContents.Value).IsEqualTo("ABC");
		}
	}

	[Test]
	public async Task RoundtripWithNullableAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<ParentPropertiesData>>();
		var data = await portal.CreateAsync();

		data.StringContents = "4";
		data.ChildContents.Value = "ABC";
		data.StringContents = null!;
		data.ChildContents = null!;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (ParentPropertiesData)formatter.Deserialize(stream)!;

		using (Assert.Multiple())
		{
			await Assert.That(newData.StringContents).IsEqualTo(string.Empty);
			await Assert.That(newData.ChildContents).IsNull();
		}
	}
}