using Csla;
using Csla.DataPortalClient;
using Csla.Server;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CslaGeneratorSerialization.IntegrationTests;

[GeneratorSerializable]
public sealed partial class DataPortalData
	: BusinessBase<DataPortalData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<string> ContentsProperty =
		RegisterProperty<string>(nameof(DataPortalData.Contents));
	public string Contents
	{
		get => this.GetProperty(DataPortalData.ContentsProperty)!;
		set => this.SetProperty(DataPortalData.ContentsProperty, value);
	}
}

internal static class DataPortalTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var proxy = new CustomProxy(provider.GetRequiredService<ApplicationContext>());
		var portal = provider.GetRequiredService<IDataPortal<DataPortalData>>();
		var data = await portal.CreateAsync();

		data.Contents = "ABC";

		var result = await proxy.Update(data, new DataPortalContext(provider.GetRequiredService<ApplicationContext>(), true), true);

		Assert.That(result.ReturnObject, Is.Not.Null);
	}
}

file sealed class CustomProxy 
	: DataPortalProxy
{
	public CustomProxy(ApplicationContext applicationContext)
		: base(applicationContext) { }

	public override string DataPortalUrl => "http://localhost:5000/dataportal";

	protected override Task<byte[]> CallDataPortalServer(
		byte[] serialized, string operation, string? routingToken, bool isSync) => Task.FromResult(serialized);
}