using Csla;

namespace CslaGeneratorSerialization;

public sealed class GeneratorFormatterReaderContext
{
	private int idCounter;
	private readonly Dictionary<int, IGeneratorSerializable> references = [];

   internal GeneratorFormatterReaderContext(ApplicationContext context, BinaryReader reader) => 
		(this.Context, this.Reader) = (context, reader);

	public T CreateInstance<T>() =>
		this.Context.CreateInstance<T>();

   public void AddReference(IGeneratorSerializable reference)
	{
		this.references.Add(this.idCounter, reference);
		this.idCounter++;
	}

   public IGeneratorSerializable this[int id] => this.references[id];

   private ApplicationContext Context { get; }
	public BinaryReader Reader { get; }
}