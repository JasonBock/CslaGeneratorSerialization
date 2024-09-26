using Csla;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using CslaGeneratorSerialization.Extensions;

namespace CslaGeneratorSerialization;

public sealed class GeneratorFormatterWriterContext
{
	private int referenceIdCounter;
	private int typeNameIdCounter;
	private readonly Dictionary<object, int> references = new(new IGeneratorSerializableEqualityComparer());
	private readonly Dictionary<int, int> typeNames = [];

	internal GeneratorFormatterWriterContext(ApplicationContext context, CustomSerializationResolver resolver, BinaryWriter writer) =>
		(this.Context, this.Resolver, this.Writer) = (context, resolver, writer);

	public (bool, int) GetReference(object mobileObject)
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

	public void Write(IGeneratorSerializable? value, bool isSealed)
	{
		if (value is not null)
		{
			(var isReferenceDuplicate, var referenceId) = this.GetReference(value);

			if (isReferenceDuplicate)
			{
				this.Writer.Write((byte)SerializationState.Duplicate);
				this.Writer.Write(referenceId);
			}
			else
			{
				this.Writer.Write((byte)SerializationState.Value);

				if (!isSealed)
				{
					var valueTypeName = value.GetType().AssemblyQualifiedName!;
					(var isTypeNameDuplicate, var typeNameId) = this.GetTypeName(valueTypeName);

					if (isTypeNameDuplicate)
					{
						this.Writer.Write((byte)SerializationState.Duplicate);
						this.Writer.Write(typeNameId);
					}
					else
					{
						this.Writer.Write((byte)SerializationState.Value);
						this.Writer.Write(valueTypeName);
					}
				}

				value.SetState(this);
			}
		}
		else
		{
			this.Writer.Write((byte)SerializationState.Null);
		}
	}

	public void WriteMobileObject(IMobileObject? value)
	{
		if (value is not null)
		{
			(var isReferenceDuplicate, var referenceId) = this.GetReference(value);

			if (isReferenceDuplicate)
			{
				this.Writer.Write((byte)SerializationState.Duplicate);
				this.Writer.Write(referenceId);
			}
			else
			{
				this.Writer.Write((byte)SerializationState.Value);
				var mobileFormatter = new MobileFormatter(this.Context);
				var mobileObjectData = ((ISerializationFormatter)mobileFormatter).Serialize(value);
				this.Writer.Write((mobileObjectData.Length, mobileObjectData));
			}
		}
		else
		{
			this.Writer.Write((byte)SerializationState.Null);
		}
	}

	public void WriteCustom<TType>(TType value) =>
		this.Resolver.Resolve<TType>().Write(value, this.Writer);

	private ApplicationContext Context { get; }
	private CustomSerializationResolver Resolver { get; }

	public BinaryWriter Writer { get; }

	private sealed class IGeneratorSerializableEqualityComparer
		: EqualityComparer<object>
	{
		public override bool Equals(object x, object y) =>
			object.ReferenceEquals(x, y);

		public override int GetHashCode(object obj) =>
			obj.GetHashCode();
	}
}