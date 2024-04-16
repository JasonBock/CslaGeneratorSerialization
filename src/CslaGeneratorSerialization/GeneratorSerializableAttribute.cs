namespace CslaGeneratorSerialization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
public sealed class GeneratorSerializableAttribute
	: Attribute { }