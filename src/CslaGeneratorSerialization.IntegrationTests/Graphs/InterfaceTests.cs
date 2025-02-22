﻿using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.Graphs.InterfaceTestsDomain;

[GeneratorSerializable]
public partial interface IInterfaceData
	: IBusinessBase
{
	string Contents { get; set; }
}

[GeneratorSerializable]
public sealed partial class InterfaceData
	: BusinessBase<InterfaceData>, IInterfaceData
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<string> ContentsProperty =
		RegisterProperty<string>(nameof(InterfaceData.Contents));
	public string Contents
	{
		get => this.GetProperty(InterfaceData.ContentsProperty);
		set => this.SetProperty(InterfaceData.ContentsProperty, value);
	}

	public static readonly PropertyInfo<int> ExtraProperty =
		RegisterProperty<int>(nameof(InterfaceData.Extra));
	public int Extra
	{
		get => this.GetProperty(InterfaceData.ExtraProperty);
		set => this.SetProperty(InterfaceData.ExtraProperty, value);
	}
}

[GeneratorSerializable]
public sealed partial class ConsumeData
	: BusinessBase<ConsumeData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<IInterfaceData> ContentsProperty =
		RegisterProperty<IInterfaceData>(nameof(ConsumeData.Contents));
	public IInterfaceData Contents
	{
		get => this.GetProperty(ConsumeData.ContentsProperty);
		set => this.SetProperty(ConsumeData.ContentsProperty, value);
	}
}

public static class InterfaceTests
{
	[Test]
	public static void Roundtrip()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<ConsumeData>>();
		var childPortal = provider.GetRequiredService<IChildDataPortal<InterfaceData>>();
		var data = portal.Create();

		var childData = childPortal.CreateChild();
		childData.Contents = "ABC";
		childData.Extra = 3;
		data.Contents = childData;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (ConsumeData)formatter.Deserialize(stream);

		Assert.Multiple(() =>
		{
			var dataProperty = (InterfaceData)newData.Contents;
			Assert.That(dataProperty.Contents, Is.EqualTo("ABC"));
			Assert.That(dataProperty.Extra, Is.EqualTo(3));
		});
	}
}