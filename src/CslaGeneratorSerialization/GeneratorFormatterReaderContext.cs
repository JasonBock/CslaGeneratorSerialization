using Csla;

namespace CslaGeneratorSerialization;

public sealed class GeneratorFormatterReaderContext
{
	private int referenceIdCounter;
	private int typeNameIdCounter;
	private readonly Dictionary<int, IGeneratorSerializable> references = [];
	private readonly Dictionary<int, string> typeNames = [];

	internal GeneratorFormatterReaderContext(ApplicationContext context, BinaryReader reader) => 
		(this.Context, this.Reader) = (context, reader);

	public T CreateInstance<T>() =>
		this.Context.CreateInstance<T>();

	public T CreateInstance<T>(string typeName) =>
		(T)this.Context.CreateInstance(Type.GetType(typeName));

   public void AddReference(IGeneratorSerializable reference)
	{
		this.references.Add(this.referenceIdCounter, reference);
		this.referenceIdCounter++;
	}

	public void AddTypeName(string typeName)
	{
		this.typeNames.Add(this.typeNameIdCounter, typeName);
		this.typeNameIdCounter++;
	}
	
	public IGeneratorSerializable GetReference(int referenceId) => this.references[referenceId];
	public string GetTypeName(int typeNameId) => this.typeNames[typeNameId];

	private ApplicationContext Context { get; }
	public BinaryReader Reader { get; }
}