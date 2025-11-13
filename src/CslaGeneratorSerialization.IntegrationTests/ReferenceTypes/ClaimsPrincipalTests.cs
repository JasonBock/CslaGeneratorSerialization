using Csla;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace CslaGeneratorSerialization.IntegrationTests.ReferenceTypes.ClaimsPrincipalTestsDomain;

[GeneratorSerializable]
public sealed partial class ClaimsPrincipalData
	: BusinessBase<ClaimsPrincipalData>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<ClaimsPrincipal> ContentsProperty =
		RegisterProperty<ClaimsPrincipal>(nameof(ClaimsPrincipalData.Contents));
	public ClaimsPrincipal Contents
	{
		get => this.GetProperty(ClaimsPrincipalData.ContentsProperty)!;
		set => this.SetProperty(ClaimsPrincipalData.ContentsProperty, value);
	}
}

public sealed class ClaimsPrincipalTests
{
	[Test]
	public async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<ClaimsPrincipalData>>();
		var data = await portal.CreateAsync();

		data.Contents = new ClaimsPrincipal(
			new ClaimsIdentity(
			[
				new Claim(ClaimTypes.Role, "admin")
			], "fake auth"));

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (ClaimsPrincipalData)formatter.Deserialize(stream)!;

		using (Assert.Multiple())
		{
			var identity = newData.Contents.Identities.Single();
			await Assert.That(identity.AuthenticationType).IsEqualTo("fake auth");
			var claim = identity.Claims.Single();
			await Assert.That(claim.Type).IsEqualTo("http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
			await Assert.That(claim.Value).IsEqualTo("admin");
		}
	}

	[Test]
	public async Task RoundtripWithNullableAsync()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<ClaimsPrincipalData>>();
		var data = await portal.CreateAsync();

		data.Contents = new ClaimsPrincipal(
			new ClaimsIdentity(
			[
				new Claim(ClaimTypes.Role, "admin")
			], "fake auth")); ;
		data.Contents = null!;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (ClaimsPrincipalData)formatter.Deserialize(stream)!;

		await Assert.That(newData.Contents).IsNull();
	}
}