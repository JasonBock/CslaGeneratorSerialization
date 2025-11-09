using Csla;
using Csla.Serialization;
using Csla.Serialization.Mobile;

namespace CslaGeneratorSerialization;

public sealed class GeneratorFormatter
	: ISerializationFormatter
{
   private const string SerializationExceptionMessage = "The given object must implement IGeneratorSerializable or derive from IMobileObject.";
   private readonly ApplicationContext applicationContext;
	private readonly CustomSerializationResolver resolver;

	public GeneratorFormatter(ApplicationContext applicationContext, CustomSerializationResolver resolver) =>
		(this.applicationContext, this.resolver) = (applicationContext, resolver);

	public object? Deserialize(Stream serializationStream)
	{
		var reader = new BinaryReader(serializationStream);
		var graphTypeName = reader.ReadString();

		var graph = this.applicationContext.CreateInstance(Type.GetType(graphTypeName));

		if (graph is IGeneratorSerializable generatorGraph)
		{
			generatorGraph.GetState(new GeneratorFormatterReaderContext(this.applicationContext, this.resolver, reader));
			return generatorGraph;
		}
		else if (graph is IMobileObject)
		{
			var mobileFormatter = new MobileFormatter(this.applicationContext);
			return mobileFormatter.Deserialize(serializationStream);
		}
		else
		{
			throw new NotSupportedException(GeneratorFormatter.SerializationExceptionMessage);
		}
	}

	public object? Deserialize(byte[] serializationStream)
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
	public void Serialize(Stream serializationStream, object? graph)
	{
		if (graph is IGeneratorSerializable generatorGraph)
		{
			var writer = new BinaryWriter(serializationStream);
			writer.Write(graph.GetType().AssemblyQualifiedName);
			generatorGraph.SetState(new GeneratorFormatterWriterContext(this.applicationContext, this.resolver, writer));
		}
		else if (graph is IMobileObject mobileGraph)
		{
			var writer = new BinaryWriter(serializationStream);
			writer.Write(graph.GetType().AssemblyQualifiedName);
			var mobileFormatter = new MobileFormatter(this.applicationContext);
			mobileFormatter.Serialize(serializationStream, mobileGraph);
		}
		else
		{
			throw new NotSupportedException(GeneratorFormatter.SerializationExceptionMessage);
		}
	}

	/// <summary>
	/// Converts an object graph into a byte stream.
	/// </summary>
	/// <param name="graph">Object graph to be serialized.</param>
	public byte[] Serialize(object? graph)
	{
		using var buffer = new MemoryStream();
		this.Serialize(buffer, graph);
		buffer.Position = 0;
		return buffer.ToArray();
	}
}
