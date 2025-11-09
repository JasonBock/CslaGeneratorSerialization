using Csla;
using System.ComponentModel.DataAnnotations;

namespace CslaGeneratorSerialization.Performance;

[GeneratorSerializable]
public sealed partial class Person
	: BusinessBase<Person>
{
	[Create]
	private void Create() =>
		this.Id = Guid.NewGuid();

	[CreateChild]
	private void CreateChild() =>
		this.Id = Guid.NewGuid();

	/// <summary>
	/// 
	/// </summary>
	public static readonly PropertyInfo<uint> AgeProperty =
		RegisterProperty<uint>(nameof(Age));
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
		RegisterProperty<Guid>(nameof(Id));
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
		RegisterProperty<string>(nameof(Name));
	/// <summary>
	/// 
	/// </summary>
	[Required]
	public string Name
	{
		get => this.GetProperty(Person.NameProperty)!;
		set => this.SetProperty(Person.NameProperty, value);
	}
}

[GeneratorSerializable]
public partial class People
	: BusinessListBase<People, Person>
{
	[Create]
#pragma warning disable CA1822 // Mark members as static
   private void Create() { }
#pragma warning restore CA1822 // Mark members as static
}