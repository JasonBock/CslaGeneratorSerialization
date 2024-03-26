namespace CslaGeneratorSerialization;

public enum SerializationState
	: byte
{
	Null = 0,
	Duplicate = 1,
	Value = 2,
}