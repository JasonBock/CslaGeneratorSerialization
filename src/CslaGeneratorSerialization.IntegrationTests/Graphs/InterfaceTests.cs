using Csla;
using CslaGeneratorSerialization.IntegrationTests.Graphs.InterfaceTestsDomain;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.Graphs
{
	namespace InterfaceTestsDomain
	{
		public partial interface IInterfaceData
			: IBusinessBase
		{
			string Contents { get; set; }
		}

		[Serializable]
		public sealed partial class InterfaceData
			: BusinessBase<InterfaceData>, IInterfaceData
		{
			[Create]
			private void Create() { }

			public static readonly PropertyInfo<string> ContentsProperty =
				RegisterProperty<string>(_ => _.Contents);
			public string Contents
			{
				get => this.GetProperty(InterfaceData.ContentsProperty);
				set => this.SetProperty(InterfaceData.ContentsProperty, value);
			}

			public static readonly PropertyInfo<int> ExtraProperty =
				RegisterProperty<int>(_ => _.Extra);
			public int Extra
			{
				get => this.GetProperty(InterfaceData.ExtraProperty);
				set => this.SetProperty(InterfaceData.ExtraProperty, value);
			}
		}

		[Serializable]
		public sealed partial class ConsumeData
			: BusinessBase<ConsumeData>
		{
			[Create]
			private void Create() { }

			public static readonly PropertyInfo<IInterfaceData> ContentsProperty =
				RegisterProperty<IInterfaceData>(_ => _.Contents);
			public IInterfaceData Contents
			{
				get => this.GetProperty(ConsumeData.ContentsProperty);
				set => this.SetProperty(ConsumeData.ContentsProperty, value);
			}
		}
	}

	public static class InterfaceTests
	{
		[Test]
		public static void Roundtrip() 
		{
			var provider = Shared.ServiceProvider;
			var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>());
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
}