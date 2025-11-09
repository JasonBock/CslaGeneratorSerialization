using Csla;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
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
		get => this.GetProperty(ClaimsPrincipalData.ContentsProperty);
		set => this.SetProperty(ClaimsPrincipalData.ContentsProperty, value);
	}
}

public static class ClaimsPrincipalTests
{
	[Test]
	public static void Roundtrip()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<ClaimsPrincipalData>>();
		var data = portal.Create();

		data.Contents = new ClaimsPrincipal(
			new ClaimsIdentity(
			[
				new Claim(ClaimTypes.Role, "admin")
			], "fake auth"));

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (ClaimsPrincipalData)formatter.Deserialize(stream);

		using (Assert.EnterMultipleScope())
		{
			var identity = newData.Contents.Identities.Single();
			Assert.That(identity.AuthenticationType, Is.EqualTo("fake auth"));
			var claim = identity.Claims.Single();
			Assert.That(claim.Type, Is.EqualTo("http://schemas.microsoft.com/ws/2008/06/identity/claims/role"));
			Assert.That(claim.Value, Is.EqualTo("admin"));
		}
	}

	[Test]
	public static void RoundtripWithNullable()
	{
		var provider = Shared.ServiceProvider;
		var formatter = new GeneratorFormatter(provider.GetRequiredService<ApplicationContext>(), new(provider));
		var portal = provider.GetRequiredService<IDataPortal<ClaimsPrincipalData>>();
		var data = portal.Create();

		data.Contents = new ClaimsPrincipal(
			new ClaimsIdentity(
			[
				new Claim(ClaimTypes.Role, "admin")
			], "fake auth")); ;
		data.Contents = null!;

		using var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		stream.Position = 0;
		var newData = (ClaimsPrincipalData)formatter.Deserialize(stream);

		Assert.That(newData.Contents, Is.Null);
	}
}