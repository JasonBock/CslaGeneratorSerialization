namespace CslaGeneratorSerialization;

public interface IGeneratorSerializable
{
	void SetState(BinaryWriter writer);
	void GetState(BinaryReader reader);
}