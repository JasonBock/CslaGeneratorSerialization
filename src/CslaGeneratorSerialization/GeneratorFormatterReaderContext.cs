using Csla;
using CslaGeneratorSerialization.Extensions;

namespace CslaGeneratorSerialization;

public sealed class GeneratorFormatterReaderContext
{
	private int referenceIdCounter;
	private int typeNameIdCounter;
	private readonly Dictionary<int, object> references = [];
	private readonly Dictionary<int, string> typeNames = [];

	internal GeneratorFormatterReaderContext(ApplicationContext context, CustomSerializationResolver resolver, BinaryReader reader) => 
		(this.Context, this.Resolver, this.Reader) = (context, resolver, reader);

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

	public T? Read<T>(bool isSealed)
		where T : class, IGeneratorSerializable
	{
		var state = this.Reader.ReadStateValue();

		if (state == SerializationState.Duplicate) 
		{
			return (T)this.GetReference(this.Reader.ReadInt32());
		}
		else if (state == SerializationState.Value)
		{
			T newValue;

			if (isSealed)
			{
				newValue = this.CreateInstance<T>()!;
			}
			else
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

			((IGeneratorSerializable)newValue).GetState(this);
			this.AddReference(newValue);
			return newValue;
		}
		else
		{
			return null;
		}
	}

	public TType ReadCustom<TType>() => 
		this.Resolver.Resolve<TType>().Read(this.Reader);

	private ApplicationContext Context { get; }
	public BinaryReader Reader { get; }
	private CustomSerializationResolver Resolver { get; }
}