using Microsoft.Extensions.DependencyInjection;

namespace CslaGeneratorSerialization;

public sealed class CustomSerializationResolver
{
	private readonly IServiceProvider serviceProvider;

	public CustomSerializationResolver(IServiceProvider serviceProvider) =>
		this.serviceProvider = serviceProvider;

	public CustomSerialization<TType> Resolve<TType>()
	{
		var serialization = this.serviceProvider.GetService<CustomSerialization<TType>>();

		if (serialization is null)
		{
			throw new NotSupportedException($"No custom serialization was registered for the type {typeof(TType).FullName}");
		}
		else
		{
			return serialization;
		}
	}
}