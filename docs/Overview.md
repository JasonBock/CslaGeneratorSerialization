## Table of Contents
- [Introduction](#introduction)
- [Background and History](#background-and-history)
- [Usage](#usage)
- [Performance](#performance)
- [Conclusion](#conclusion)
  
# Introduction

In this document, I'll review `CslaGeneratorSerialization` and how it works. If something is unclear after reading the documentation, you can always browse the tests in source to see specific examples of a case that may not be covered in detail here. If you are still unable to determine how something works, feel free to add an issue [here](https://github.com/JasonBock/CslaGeneratorSerialization/issues).

# Background and History

I'm a big fan of C# source generators. Under the right circumstances, they can improve performance and reduce memory consumption dramatically. In fact, `System.Text.Json` [has a source generator](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation) for its serialization. So why not add one for CSLA?

One of the difficulties is that `MobileFormatter` is, for lack of a better term, the "dominant" serialization approach in CSLA. In fact, it's still hard-coded in some areas of CSLA where arguably `IMobileFormatter` should be used instead (though that may be resolved in the near future). Furthermore, there's a subtlety in `SerializationInfo` being used a lot during serialization, where this isn't necessarily needed either. That said, this library is an attempt to generate serializable BOs that do not use `MobileFormatter`, in the hope that serialization will be faster, and consume less memory.

# Usage

To enable custom serialization, simply make your BO `partial`:

```c#
[Serializable]
public sealed partial class Person
	: BusinessBase<Person>
{
	[Create]
	private void Create() =>
		this.Id = Guid.NewGuid();

	public static readonly PropertyInfo<uint> AgeProperty =
		RegisterProperty<uint>(_ => _.Age);
	[Required]
	public uint Age
	{
		get => this.GetProperty(Person.AgeProperty);
		set => this.SetProperty(Person.AgeProperty, value);
	}

	public static readonly PropertyInfo<Guid> IdProperty =
		RegisterProperty<Guid>(_ => _.Id);
	[Required]
	public Guid Id
	{
		get => this.GetProperty(Person.IdProperty);
		set => this.SetProperty(Person.IdProperty, value);
	}

	public static readonly PropertyInfo<string> NameProperty =
		RegisterProperty<string>(_ => _.Name);
	[Required]
	public string Name
	{
		get => this.GetProperty(Person.NameProperty);
		set => this.SetProperty(Person.NameProperty, value);
	}
}
```

The generator will create a custom implementation of `IGeneratorSerializable`. Note that you **must** do this for **all** of your BOs, even ones in other libraries. The generator assumes that this interface will be implemented and attempt to cast all BOs to this type.

You will also need to register the custom serializer during application configuration:

```c#
var services = new ServiceCollection();
services.AddCsla(o =>
  o.Serialization(so => so.SerializationFormatter(typeof(GeneratorFormatter))));
var provider = services.BuildServiceProvider();
```

That should do it to get your application to start using this custom serialization formatter.

# Performance

Right now, there aren't a lot of performance tests created. That said, the one in the performance project is encouraging:

```
MobileFormatter:
| Method    | Mean     | Error     | StdDev    | Gen0   | Gen1   | Allocated |
|---------- |---------:|----------:|----------:|-------:|-------:|----------:|
| Roundtrip | 7.884 us | 0.0546 us | 0.0456 us | 1.4038 | 0.0305 |  24.14 KB |

GeneratorFormatter:
| Method    | Mean     | Error     | StdDev    | Gen0   | Allocated |
|---------- |---------:|----------:|----------:|-------:|----------:|
| Roundtrip | 2.778 us | 0.0192 us | 0.0180 us | 0.2518 |   4.26 KB |
```

Again, more work needs to be done to ensure this approach is consistent with all stereotypes.

# Conclusion

You've now seen how `CslaGeneratorSerialization` can be used as a custom serialization library for your CSLA BOs. Remember to peruse the tests within `CslaGeneratorSerialization.IntegrationTests` in case you get stuck. If you'd like, feel free to submit a PR to update this document to improve its contents. If you run into any issues, please submit an issue. Happy coding!