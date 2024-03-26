using Csla.Serialization.Mobile;

namespace CslaGeneratorSerialization;

public sealed class GeneratorFormatterWriter
{
	private readonly Dictionary<string, int> keywords = new();
	private readonly Dictionary<IMobileObject, SerializationInfo> _serializationReferences =
	  new(new ReferenceComparer<IMobileObject>());

   internal GeneratorFormatterWriter(Stream serializationStream, IGeneratorSerializable graph) => 
		graph.SetState(new BinaryWriter(serializationStream));
}