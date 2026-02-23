using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests.ISerializationNotificationTestsDomain;

[GeneratorSerializable]
public partial class Point
	: BusinessBase<Point>
{
	[Create]
	private void Create() { }

	protected override void Deserialized() =>
		this.Sum = this.X + this.Y;

	public int Sum { get; private set; }

	public static readonly PropertyInfo<int> XProperty =
		Point.RegisterProperty<int>(nameof(Point.X));
	public int X
	{
		get => this.GetProperty(Point.XProperty);
		set => this.SetProperty(Point.XProperty, value);
	}

	public static readonly PropertyInfo<int> YProperty =
		Point.RegisterProperty<int>(nameof(Point.Y));
	public int Y
	{
		get => this.GetProperty(Point.YProperty);
		set => this.SetProperty(Point.YProperty, value);
	}
}

internal static class ISerializationNotificationTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<Point>>();
		var data = await portal.CreateAsync();

		data.X = 10;
		data.Y = 20;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(data.Sum, Is.Zero);

			using var stream = new MemoryStream();
			formatter.Serialize(stream, data);
			stream.Position = 0;
			var newData = (Point)formatter.Deserialize(stream)!;

			Assert.That(newData.Sum, Is.EqualTo(30));
		}
	}
}