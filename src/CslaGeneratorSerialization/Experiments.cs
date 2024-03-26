using Csla;
using System.ComponentModel.DataAnnotations;

#nullable enable

namespace CslaGeneratorSerialization;

/// <summary>
/// Defines a person.
/// </summary>
/// <remarks>
/// This is what a developer would define.
/// </remarks>
[Serializable]
public sealed partial class Person
	: BusinessBase<Person>
{
	[Create]
	private void Create() =>
		this.Id = Guid.NewGuid();

	/// <summary>
	/// 
	/// </summary>
	public static readonly PropertyInfo<uint> AgeProperty =
		RegisterProperty<uint>(_ => _.Age);
	/// <summary>
	/// 
	/// </summary>
	[Required]
	public uint Age
	{
		get => this.GetProperty(Person.AgeProperty);
		set => this.SetProperty(Person.AgeProperty, value);
	}

	/// <summary>
	/// 
	/// </summary>
	public static readonly PropertyInfo<Guid> IdProperty =
		RegisterProperty<Guid>(_ => _.Id);
	/// <summary>
	/// 
	/// </summary>
	[Required]
	public Guid Id
	{
		get => this.GetProperty(Person.IdProperty);
		set => this.SetProperty(Person.IdProperty, value);
	}

	/// <summary>
	/// 
	/// </summary>
	public static readonly PropertyInfo<string> NameProperty =
		RegisterProperty<string>(_ => _.Name);
	/// <summary>
	/// 
	/// </summary>
	[Required]
	public string Name
	{
		get => this.GetProperty(Person.NameProperty);
		set => this.SetProperty(Person.NameProperty, value);
	}
}

/// <remarks>
/// This is what the SG would create
/// </remarks>
public sealed partial class Person
	: IGeneratorSerializable
{
	void IGeneratorSerializable.SetState(BinaryWriter writer)
	{
		// Set custom object state
		writer.Write(this.GetType().FullName);
		writer.Write(this.ReadProperty(Person.AgeProperty));
		writer.Write(this.ReadProperty(Person.IdProperty).ToByteArray());
		writer.Write(this.ReadProperty(Person.NameProperty));

		// Set base object state
		writer.Write(this.IsNew);
		writer.Write(this.IsDeleted);
		writer.Write(this.IsDirty);
		writer.Write(this.IsChild);
		writer.Write(this.DisableIEditableObject);

		//The only way I can get these is through Reflection.
		//Ugly, but...means must.
		var type = this.GetType();
		writer.Write((bool)type.GetField("_neverCommitted")!.GetValue(this));
		writer.Write((int)type.GetField("_editLevelAdded")!.GetValue(this));
		writer.Write((int)type.GetField("_identity")!.GetValue(this));

		// Set any children.
	}

	void IGeneratorSerializable.GetState(BinaryReader reader)
	{
		// Get custom object state
		this.LoadProperty(Person.AgeProperty, reader.ReadUInt32());
		this.LoadProperty(Person.IdProperty, new Guid(reader.ReadBytes(16)));
		this.LoadProperty(Person.NameProperty, reader.ReadString());

		// Get any children...

		//The only way I can get these (except for DisableIEditableObject) is through Reflection.
		//Ugly, but...means must.
		var type = this.GetType();
		type.GetField("_isNew")!.SetValue(this, reader.ReadBoolean());
		type.GetField("_isDeleted")!.SetValue(this, reader.ReadBoolean());
		type.GetField("_isDirty")!.SetValue(this, reader.ReadBoolean());
		type.GetField("_isChild")!.SetValue(this, reader.ReadBoolean());
		this.DisableIEditableObject = reader.ReadBoolean();

		type.GetField("_neverCommitted")!.SetValue(this, reader.ReadBoolean());
		type.GetField("_editLevelAdded")!.SetValue(this, reader.ReadInt32());
		type.GetField("_identity")!.SetValue(this, reader.ReadInt32());
	}
}