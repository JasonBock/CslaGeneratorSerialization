namespace CslaGeneratorSerialization;

public interface IGeneratorSerializableCustomization
{
	void GetCustomState(BinaryReader reader);
	void SetCustomState(BinaryWriter writer);
}