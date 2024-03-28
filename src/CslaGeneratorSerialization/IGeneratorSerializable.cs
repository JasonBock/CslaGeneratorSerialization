namespace CslaGeneratorSerialization;

public interface IGeneratorSerializable
{
	void SetState(GeneratorFormatterWriterContext writer);
	void GetState(GeneratorFormatterReaderContext reader);
}