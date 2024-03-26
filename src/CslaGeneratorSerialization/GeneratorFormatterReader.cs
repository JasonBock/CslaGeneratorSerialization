using Csla.Serialization.Mobile;

namespace CslaGeneratorSerialization;

internal sealed class GeneratorFormatterReader
{
	private readonly Dictionary<string, int> keywords = new();
	private readonly Dictionary<IMobileObject, SerializationInfo> _serializationReferences =
	  new(new ReferenceComparer<IMobileObject>());

   internal GeneratorFormatterReader(IGeneratorSerializable generatorGraph, BinaryReader reader)
	{
		generatorGraph.GetState(reader);
		this.Graph = generatorGraph;
	}

	internal IGeneratorSerializable Graph { get; }
}