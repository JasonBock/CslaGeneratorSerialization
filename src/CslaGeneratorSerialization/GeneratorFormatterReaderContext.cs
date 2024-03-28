using Csla;

namespace CslaGeneratorSerialization;

public sealed class GeneratorFormatterReaderContext
{
	private readonly Dictionary<int, IGeneratorSerializable> references = [];

   internal GeneratorFormatterReaderContext(ApplicationContext context, BinaryReader reader) => 
		(this.Context, this.Reader) = (context, reader);

	public T CreateInstance<T>() =>
		this.Context.CreateInstance<T>();

	public IGeneratorSerializable this[int id]
	{
		get => this.references[id];
		set => this.references[id] = value;
	}

	private ApplicationContext Context { get; }
	public BinaryReader Reader { get; }
}