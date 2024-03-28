namespace CslaGeneratorSerialization;

public sealed class GeneratorFormatterWriterContext
{
	private int idCounter;
	private readonly Dictionary<IGeneratorSerializable, int> references = new(new IGeneratorSerializableEqualityComparer());

	internal GeneratorFormatterWriterContext(BinaryWriter writer) =>
		this.Writer = writer;

	public (bool isDuplicate, int id) GetReference(IGeneratorSerializable mobileObject)
	{
		if (this.references.TryGetValue(mobileObject, out var value))
		{
			return (true, value);
		}
		else
		{
			this.references.Add(mobileObject, this.idCounter);
			this.idCounter++;
			return (false, this.idCounter);
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