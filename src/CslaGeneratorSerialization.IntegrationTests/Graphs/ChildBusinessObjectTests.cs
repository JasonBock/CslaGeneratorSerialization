using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.Graphs.ChildBusinessObjectTestsDomain;

[GeneratorSerializable]
public sealed partial class ParentData
	: BusinessBase<ParentData>
{
	[Create]
	private void Create() =>
		this.Contents = this.ApplicationContext.GetRequiredService<IChildDataPortal<ChildData>>().CreateChild();

   public static readonly PropertyInfo<ChildData> ContentsProperty =
		RegisterProperty<ChildData>(nameof(ParentData.Contents));
	public ChildData Contents
	{
		get => this.GetProperty(ParentData.ContentsProperty);
		set => this.SetProperty(ParentData.ContentsProperty, value);
	}
}

[GeneratorSerializable]
public sealed partial class ChildData
	: BusinessBase<ChildData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<string> ChildContentsProperty =
		RegisterProperty<string>(nameof(ChildData.ChildContents));
	public string ChildContents
	{
		get => this.GetProperty(ChildData.ChildContentsProperty);
		set => this.SetProperty(ChildData.ChildContentsProperty, value);
	}
}

[GeneratorSerializable]
public sealed partial class NonParticipatingParentData
	: BusinessBase<NonParticipatingParentData>
{
	[Create]
	private void Create() =>
		this.Contents = this.ApplicationContext.GetRequiredService<IChildDataPortal<NonParticipatingChildData>>().CreateChild();

	public static readonly PropertyInfo<NonParticipatingChildData> ContentsProperty =
		RegisterProperty<NonParticipatingChildData>(nameof(NonParticipatingParentData.Contents));
	public NonParticipatingChildData Contents
	{
		get => this.GetProperty(NonParticipatingParentData.ContentsProperty);
		set => this.SetProperty(NonParticipatingParentData.ContentsProperty, value);
	}
}

public sealed class NonParticipatingChildData
	: BusinessBase<NonParticipatingChildData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<string> ChildContentsProperty =
		RegisterProperty<string>(nameof(NonParticipatingChildData.ChildContents));
	public string ChildContents
	{
		get => this.GetProperty(NonParticipatingChildData.ChildContentsProperty);
		set => this.SetProperty(NonParticipatingChildData.ChildContentsProperty, value);
	}
}

public static class ChildBusinessObjectTests
{
	[Test]
	public static void Roundtrip()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<ParentData>>();
		var data = portal.Create();

		data.Contents.ChildContents = "ABC";

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (ParentData)formatter.Deserialize(stream);

		Assert.That(newData.Contents.ChildContents, Is.EqualTo("ABC"));
	}

	[Test]
	public static void RoundtripWithNonParticipatingChild()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<NonParticipatingParentData>>();
		var data = portal.Create();

		data.Contents.ChildContents = "ABC";

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (NonParticipatingParentData)formatter.Deserialize(stream);

		Assert.That(newData.Contents.ChildContents, Is.EqualTo("ABC"));
	}

	[Test]
	public static void RoundtripWithNullable()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<ParentData>>();
		var data = portal.Create();

		data.Contents.ChildContents = "ABC";
		data.Contents = null!;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (ParentData)formatter.Deserialize(stream);

		Assert.That(newData.Contents, Is.Null);
	}
}