namespace CslaGeneratorSerialization;

public interface IGeneratorSerializable
{
	void GetState(GeneratorFormatterReaderContext reader);
	void SetState(GeneratorFormatterWriterContext writer);
}