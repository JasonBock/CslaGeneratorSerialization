using Csla.Serialization.Mobile;

namespace CslaGeneratorSerialization;

internal sealed class MobileTypeMap<T>
	: ITypeMap
{
	public Type SerializerType => typeof(MobileCustomSerializer<T>);

	public Func<Type, bool> CanSerialize => t => t == typeof(T);
}