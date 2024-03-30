namespace CslaGeneratorSerialization;

public sealed class GeneratorFormatterWriterContext
{
	private int referenceIdCounter;
	private int typeNameIdCounter;
	private readonly Dictionary<IGeneratorSerializable, int> references = new(new IGeneratorSerializableEqualityComparer());
	private readonly Dictionary<int, int> typeNames = [];

	internal GeneratorFormatterWriterContext(BinaryWriter writer) =>
		this.Writer = writer;

	public (bool, int) GetReference(IGeneratorSerializable mobileObject)
	{
		if (this.references.TryGetValue(mobileObject, out var value))
		{
			return (true, value);
		}
		else
		{
			var id = this.referenceIdCounter;
			this.references.Add(mobileObject, id);
			this.referenceIdCounter++;
			return (false, id);
		}
	}

	public (bool, int) GetTypeName(string typeName)
	{
	  var typeNameHashCode = typeName.GetHashCode();

	  if (this.typeNames.TryGetValue(typeNameHashCode, out var value))
		{
			return (true, value);
		}
		else
		{
			var id = this.typeNameIdCounter;
			this.typeNames.Add(typeNameHashCode, id);
			this.typeNameIdCounter++;
			return (false, id);
		}
	}

	public BinaryWriter Writer { get; }

	private sealed class IGeneratorSerializableEqualityComparer
		: EqualityComparer<IGeneratorSerializable>
	{
		public override bool Equals(IGeneratorSerializable x, IGeneratorSerializable y) => 
			object.ReferenceEquals(x, y);

		public override int GetHashCode(IGeneratorSerializable obj) => 
			obj.GetHashCode();
	}
}