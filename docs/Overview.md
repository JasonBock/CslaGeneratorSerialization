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

One of the difficulties is that `MobileFormatter` is, for lack of a better term, the "dominant" serialization approach in CSLA. In fact, it's still hard-coded in some areas of CSLA where arguably `IMobileFormatter` should be used instead (though this is mostly resolved with CSLA 10). Furthermore, there's a subtlety in `SerializationInfo` being used a lot during serialization, where this isn't necessarily needed either. That said, this library is an attempt to generate serializable BOs that do not use `MobileFormatter`, in the hope that serialization will be faster, and consume less memory.

# Usage

To enable custom serialization, make your BO `partial` and mark it with `[GeneratorSerializable]`:

```c#
using CslaGeneratorSerialization;

[GeneratorSerializable]
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

The generator will create another partial definition of `Person` that implements `IGeneratorSerializable`. Note that you **must** mark your BOs with this attribute for the formatter to work. The generator assumes that this interface will be implemented and attempt to cast all BOs to this type.

You will also need to register the custom serializer during application configuration:

```c#
var services = new ServiceCollection();
services.AddCsla(o =>
  o.Serialization(so => so.SerializationFormatter(typeof(GeneratorFormatter))));
services.AddCslaGeneratorSerialization();
var provider = services.BuildServiceProvider();
```

You need to register `GeneratorFormatter` as the serialization formatter CSLA. The call to `AddCslaGeneratorSerialization()` sets up other things that the generator needs. That should do it to get your application to start using this custom serialization formatter. If you find any issues with this serializer, you can always back out by changing the serializer back to `MobileFormatter` in your call to `AddCsla()`.

You can also support for types that the generator doesn't know how to serialize. You need to call `AddCslaGeneratorSerialiation()` on your `IServiceCollection` instance, and then register instances of `CustomSerialization` for each type you want to support. For example, if you wanted to add support for `int[]`, you could do this:

```c#
services.AddCslaGeneratorSerialization();
services.AddSingleton(
  new CustomSerialization<int[]>(
    (data, writer) =>
    {
      writer.Write(data.Length);

      foreach (var item in data)
      {
        writer.Write(item);
      }
    },
    (reader) =>
    {
      var data = new int[reader.ReadInt32()];

      for (var i = 0; i < data.Length; i++)
      {
        data[i] = reader.ReadInt32();
      }

      return data;
    }));
```

Now, if your managed backing field uses `int[]`, it'll serialize without any issues.

You can also perform custom serialization within your business object by implementing `IGeneratorSerializableCustomization`:

```c#
[GeneratorSerializable]
public sealed partial class Data
  : BusinessBase<Data>, IGeneratorSerializableCustomization
{
  public void GetCustomState(BinaryReader reader)
  {
    ArgumentNullException.ThrowIfNull(reader);
    this.Custom = reader.ReadString();
  }

  public void SetCustomState(BinaryWriter writer)
  {
    ArgumentNullException.ThrowIfNull(writer);
    writer.Write(this.Custom);
  }

  public static readonly PropertyInfo<string> ContentsProperty =
    RegisterProperty<string>(_ => _.Contents);
  public string Contents
  {
    get => this.GetProperty(Data.ContentsProperty);
    set => this.SetProperty(Data.ContentsProperty, value);
  }

  public int Custom { get; set; }
}
```

You can read and write whatever you want to the underlying stream. The generator will automatically add the calls to `GetCustomState()` and `SetCustomState()` when it detects that your business object derives from `IGeneratorSerializableCustomization`.

If this serializer encounters an object within a graph that doesn't implement `IGeneratorSerializable`, it'll try to use `MobileFormatter` to handle serialization. Therefore, you don't have to change **every** business object in your application to use `CslaGeneratorSerialization`, though you should if you can. This feature is only there to handle full graph serialization when you don't own the implementation of a type within a graph.

There are more examples in the test projects - feel free to check out what's there.

If you find any issues with this serializer, you can always back out by changing the serializer back to `MobileFormatter` in your call to `AddCsla()`.

# Performance

Performance results can be found [here](https://github.com/JasonBock/CslaGeneratorSerialization/blob/main/src/CslaGeneratorSerialization.Performance/results.md).

# Conclusion

You've now seen how `CslaGeneratorSerialization` can be used as a custom serialization library for your CSLA BOs. Remember to peruse the tests within `CslaGeneratorSerialization.IntegrationTests` in case you get stuck. If you'd like, feel free to submit a PR to update this document to improve its contents. If you run into any issues, please submit an issue. Happy coding!
