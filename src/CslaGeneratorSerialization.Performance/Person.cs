﻿using Csla;
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

[GeneratorSerializable]
public partial class People
	: BusinessListBase<People, Person>
{
	[Create]
#pragma warning disable CA1822 // Mark members as static
   private void Create() { }
#pragma warning restore CA1822 // Mark members as static
}