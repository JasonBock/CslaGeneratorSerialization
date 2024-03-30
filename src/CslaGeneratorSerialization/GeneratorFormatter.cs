using Csla;
using Csla.Serialization;

namespace CslaGeneratorSerialization;

public sealed class GeneratorFormatter
	: ISerializationFormatter
{
	private readonly ApplicationContext applicationContext;

	public GeneratorFormatter(ApplicationContext applicationContext) =>
		this.applicationContext = applicationContext;

	public object Deserialize(Stream serializationStream)
   {
		var reader = new BinaryReader(serializationStream);
		var graphTypeName = reader.ReadString();

		var graph = this.applicationContext.CreateInstance(Type.GetType(graphTypeName));

		if (graph is not IGeneratorSerializable generatorGraph)
		{
			throw new NotSupportedException("The given object must implement IGeneratorSerializable.");
		}

		generatorGraph.GetState(new GeneratorFormatterReaderContext(this.applicationContext, reader));
		return generatorGraph;
	}

	public object Deserialize(byte[] serializationStream)
	{
		using var stream = new MemoryStream(serializationStream);
		return this.Deserialize(stream);
	}

	/// <summary>
	/// Serialize an object graph.
	/// </summary>
	/// <param name="serializationStream">
	/// Stream to which the serialized data
	/// will be written.
	/// </param>
	/// <param name="graph">
	/// Root object of the object graph
	/// to serialize.
	/// </param>
	public void Serialize(Stream serializationStream, object graph)
	{
		if (graph is not IGeneratorSerializable generatorGraph)
		{
			throw new NotSupportedException("The given object must implement IGeneratorSerializable.");
		}

		var writer = new BinaryWriter(serializationStream);
		writer.Write(graph.GetType().AssemblyQualifiedName);
		generatorGraph.SetState(new GeneratorFormatterWriterContext(writer));
	}

	/// <summary>
	/// Converts an object graph into a byte stream.
	/// </summary>
	/// <param name="graph">Object graph to be serialized.</param>
	public byte[] Serialize(object graph)
	{
		using var buffer = new MemoryStream();
		this.Serialize(buffer, graph);
		buffer.Position = 0;
		return buffer.ToArray();
	}
}
