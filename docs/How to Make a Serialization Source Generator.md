# How to Make a Serialization Source Generator in CSLA

First, `ApplicationContext.SerializationFormatter` needs to be set immediately. Actually, it's `internal`, so you can't, but it is set through `AddCsla()`. Look at the `SerializationTests.UseCustomFormatter()` test. 

Then, I need to create a class that derives from `ISerializationFormatter`. Call it `GeneratorSerializationFormatter`. This will not be generated, as it will assume things that are done with the SG.

Next, create an interface called `IGeneratorSerialization`. This will be implemented on each BO that is serializable. Therefore, each BO must be a `partial` type. This would be a big breaking change with CSLA in terms of how people define BOs, but hopefully there's a performance payoff that will justify it.

The SG will look for types that are serializable. `[Serializable]` isn't enough if I remember right. It must derive from `IMobileObject`. `IsBusinessObjectSerializableAnalyzer` enforces that `[Serializable]` is on a BO (`CSLA0001`), so we can use `ForAttributeWithMetadataName` to look for occurrences of that attribute, and then check that it derives from `IMobileObject` (see `ITypeSymbolExtensions.IsMobileObject()`). If it does, then we need to grab information about that object that will used to create the partial type.

What specifically do we need to grab?

Look at the tests here, and ensure that I can reproduce them - https://github.com/MarimerLLC/csla/blob/main/Source/Csla.test/Serialization/SerializationTests.cs - as well as any others that are relevant.

Rocky suggested looking at `ObjectFactory` in the `Csla.Server` namespace (maybe?) as it uses a field manager outside of "mobile serialization" and may be useful for what I need to do.

States:
* Null - skip reading/writing the value, and go to the next one
* Duplicate - next value is an Int32 identifying the 
* Value - has a value

Each value:
* If it's a value type
    * Non-nullable, just read/write the value
    * Nullable, add a state value
        * non-nullable, read/write the value.
* If it's a reference type
    * Add a state value
        * Null
        * Items - this 
        * Value - if it's a `IGeneratorSerializable`, then we need to get a trackable ID value, read/write that, and then tell it to read/write. Otherwise, just store the value
        * Duplicate - this is only for objects that are `IGeneratorSerializable`. If we see this, read/write the trackable ID

Plan of attack:

* Create a `IGeneratorSerializable` interface that would be used on the partial classes. It'll need `Serialize()` and `Deserialize()` methods, as well as the ability to get/set the object's state (similar to what is done with `IMobileObject` and `Get/SetState()` and `Get/SetChildren()`). 
    * See https://github.com/MarimerLLC/csla/blob/main/Source/Csla/Core/MobileObject.cs for how it's done.
    * OnGet/SetState is here - https://github.com/MarimerLLC/csla/blob/main/Source/Csla/Core/UndoableBase.cs
    * OnGet/SetState and OnGet/SetChildren is here - https://github.com/MarimerLLC/csla/blob/main/Source/Csla/Core/BusinessBase.cs
* Get the generator to do a file creation that does "just enough" to get that scaffolding in place.
* Handle an object that has just simple fields with no child state, and see if that works.
* Add child objects
* Ensure string tables and duplicate references are handled correctly.
* Claims
* Tests
    * Value types
    * Reference types
    * Duplicates
    * "Invalid" - e.g. symbol has diagnostics, or no managed backing fields.

MobileFormatter:
| Method    | Mean     | Error     | StdDev    | Gen0   | Gen1   | Allocated |
|---------- |---------:|----------:|----------:|-------:|-------:|----------:|
| Roundtrip | 7.981 us | 0.0393 us | 0.0307 us | 1.4038 | 0.0305 |  23.83 KB |

GeneratorFormatter:
| Method    | Mean     | Error     | StdDev    | Gen0   | Allocated |
|---------- |---------:|----------:|----------:|-------:|----------:|
| Roundtrip | 2.054 us | 0.0122 us | 0.0114 us | 0.1755 |   3.08 KB |

Issues
* It is weird that `SerializationFormatter` is `static`, but that's probably a holdover from an earlier design, and changing it might break people. That said, I really should be able to get the configured `ISerializationFormatter` instance from DI, not as a static property.
* `Get/SetState()` and `Get/SetChildren()` appear to require `MobileFormatter`, instead of `ISerializationFormatter`, which makes it hard to do pluggable serialization