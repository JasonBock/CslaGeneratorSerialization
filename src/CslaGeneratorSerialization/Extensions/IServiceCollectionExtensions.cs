using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CslaGeneratorSerialization.Extensions;

public static class IServiceCollectionExtensions
{
	public static IServiceCollection AddCslaGeneratorSerialization(this IServiceCollection self,
		params ReadOnlySpan<CustomSerialization> customSerializations)
	{
		var options = new MobileFormatterOptions();

		// We register MobileFormatterOptions because,
		// if we need to use MobileFormatter to handle serialization
		// for IMobileObject-based objects that
		// do not participate in generator serialization,
		// this type is needed by MobileFormatter.
		foreach (var customSerialization in customSerializations)
		{
			self = self.AddSingleton(customSerialization.GetType(), customSerialization);
			options.CustomSerializers.Add(customSerialization.TypeMap);
		}

		return self.AddSingleton<CustomSerializationResolver>()
			.AddSingleton(options);
	}
}