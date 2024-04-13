using Csla;
using CslaGeneratorSerialization.Extensions;

namespace CslaGeneratorSerialization;

public sealed class GeneratorFormatterReaderContext
{
	private int referenceIdCounter;
	private int typeNameIdCounter;
	private readonly Dictionary<int, object> references = [];
	private readonly Dictionary<int, string> typeNames = [];

	internal GeneratorFormatterReaderContext(ApplicationContext context, BinaryReader reader) => 
		(this.Context, this.Reader) = (context, reader);

	public T CreateInstance<T>() =>
		this.Context.CreateInstance<T>();

	public T CreateInstance<T>(string typeName) =>
		(T)this.Context.CreateInstance(Type.GetType(typeName));

   public void AddReference(object reference)
	{
		this.references.Add(this.referenceIdCounter, reference);
		this.referenceIdCounter++;
	}

	public void AddTypeName(string typeName)
	{
		this.typeNames.Add(this.typeNameIdCounter, typeName);
		this.typeNameIdCounter++;
	}
	
	public object GetReference(int referenceId) => this.references[referenceId];
	public string GetTypeName(int typeNameId) => this.typeNames[typeNameId];

	public void Read<T>(Action<T> propertyLoader, bool isNotSealed)
	{
		switch (this.Reader.ReadStateValue())
		{
			case SerializationState.Duplicate:
				propertyLoader((T)this.GetReference(this.Reader.ReadInt32()));
				break;
			case SerializationState.Value:
				T newValue;

				if (isNotSealed)
				{
					if (this.Reader.ReadStateValue() == SerializationState.Duplicate)
					{
						newValue = this.CreateInstance<T>(this.GetTypeName(this.Reader.ReadInt32()))!;
					}
					else
					{
						var newValueTypeName = this.Reader.ReadString();
						this.AddTypeName(newValueTypeName);
						newValue = this.CreateInstance<T>(newValueTypeName)!;
					}
				}
				else
				{
					newValue = this.CreateInstance<T>()!;
				}

				((IGeneratorSerializable)newValue).GetState(this);
				propertyLoader(newValue);
				this.AddReference(newValue);
				break;
			case SerializationState.Null:
				break;
		}
	}

	private ApplicationContext Context { get; }
	public BinaryReader Reader { get; }
}