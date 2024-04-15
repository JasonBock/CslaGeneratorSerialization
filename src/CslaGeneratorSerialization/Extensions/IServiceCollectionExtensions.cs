using Microsoft.Extensions.DependencyInjection;

namespace CslaGeneratorSerialization.Extensions;

public static class IServiceCollectionExtensions
{
	public static IServiceCollection AddCslaGeneratorSerialization(this IServiceCollection self)
	{
		self.AddSingleton<CustomSerializationResolver>();
		return self;
	}
}