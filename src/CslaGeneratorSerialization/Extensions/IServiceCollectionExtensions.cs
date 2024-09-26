using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CslaGeneratorSerialization.Extensions;

public static class IServiceCollectionExtensions
{
	public static IServiceCollection AddCslaGeneratorSerialization(this IServiceCollection self)
	{
		// We register MobileFormatterOptions because,
		// if we need to use MobileFormatter to handle serialization
		// for IMobileObject-based objects that
		// do not participate in generator serialization,
		// this type is needed by MobileFormatter.
		self.AddSingleton<CustomSerializationResolver>()
			.AddSingleton<MobileFormatterOptions>();
		return self;
	}
}