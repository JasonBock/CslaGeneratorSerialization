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

* DONE - Create a `IGeneratorSerializable` interface that would be used on the partial classes. It'll need `Serialize()` and `Deserialize()` methods, as well as the ability to get/set the object's state (similar to what is done with `IMobileObject` and `Get/SetState()` and `Get/SetChildren()`). 
    * DONE - See https://github.com/MarimerLLC/csla/blob/main/Source/Csla/Core/MobileObject.cs for how it's done.
    * DONE - OnGet/SetState is here - https://github.com/MarimerLLC/csla/blob/main/Source/Csla/Core/UndoableBase.cs
    * DONE - OnGet/SetState and OnGet/SetChildren is here - https://github.com/MarimerLLC/csla/blob/main/Source/Csla/Core/BusinessBase.cs
* DONE - Get the generator to do a file creation that does "just enough" to get that scaffolding in place.
* DONE - Handle an object that has just simple fields with no child state, and see if that works.
* Add all cases
    * DONE - Value types
        * DONE - Enums
    * DONE - Reference types
    * DONE - "Special" types:
        * DONE - `byte[]`
        * DONE - `byte[][]`
        * DONE - `char[]`
        * DONE - `List<int>`
    * DONE - `IGeneratorSerializable` and duplicates
    * DONE - Should abstract BOs be targeted? They can't be created, so...not sure about this. Maybe, but then make sure to add `abstract` to the partial class.
    * DONE - Inheritance hierarchies and loading/storing fields (e.g. `Customer` derives from `Person` and ensure everything is serialized correctly)
    * DONE - "Invalid" - e.g. symbol has diagnostics, or no managed backing fields.
    * DONE - What if the target in `PropertyInfo<>` is an interface? Or my base type is `IBusinessBase`? Deserialization may need the name of the concrete type. So, if the serializable type is not `sealed`, then we need to get the name of the type at runtime and store that into a `Dictionary<,>`, akin to what I'm doing with duplicate references.
        * DONE - On serialization, store the count and number of type strings at the "front" of the stream (basically need to create two and combine them).
        * DONE - On deserialization, read the count, and deserialize the `string`/`int` pairs if any exist.
        * DONE - Can use a `Dictionary<int, int>` and keep track of the string hash codes, not the strings themselves.
        * DONE - Should change the reader to not return a string, but add to the stream as needed
    * DONE - Claims - `System.Security.Claims.ClaimsPrincipal`
        * DONE - On serialization, then pass that into a `Security.CslaClaimsPrincipal(...)` constructor. That new `CslaClaimsPrincipal` is an `IMobileObject`, so just treat that as such from that point on in terms of duplication. However, the way it serializes is to call `WriteTo()` on its own (this is defined on `ClaimsPrincipal`), and then pass that as a `(int length, byte[] buffer)` to the main stream. See https://github.com/MarimerLLC/csla/blob/main/Source/Csla/Serialization/Mobile/MobileFormatter.cs#L143 for details
        * DONE - On deserialization, read the `byte[]` value, and pass that into a new `BinaryReader`, which will in turn be passed to `new Security.CslaClaimsPrincipal(reader)`. See https://github.com/MarimerLLC/csla/blob/main/Source/Csla/Serialization/Mobile/MobileFormatter.cs#L269 for details.
    * (This would be a good idea **if** we weren't forced to target NS 2.0, because DIMs aren't supported there :( ) What if an object wants to participate in the serialization process without using `PropertyInfo`s? I thought there was something akin to that in CSLA proper. Maybe use `OnCustomGet/SetState()` that are DIM that people can override. Here's a sharplab.io link demonstrating this: https://sharplab.io/#v2:C4LgTgrgdgNAJiA1AHwAICYAMBYAUBgRj1UwAJUCA6ASQHkBuPPAJQHto5gwBLABwAooAUwDupAHKswAWwCGAG34BKJY3wEAnMrVsOXPoNGkAwhADOwVtOWqm6gGzkALKV1ROPAdQDKQngu4ALyFSYFkwAHMhYCU8AG88UiTyTX4wyOjKAHFogBUAT14hZUoAMQh5eXFZaSFbXGTQ8KjgSgARITM/bgDgwQr5AEJ6xvSWylMLKw6u/3kg4qgB4fpGxOSxzN85hf7KlfWkzdbJy2ltnvm+pf36gF87VABmUm4oYD8AM1kAYxCfbq9ITxQ7OUgzQFXYoAITe4XyzCEsjgflIYCRKLAI2SqBcp2mnUhu1hUHhiORqPRFKxpDiKS0ACIATtghNzGcISzikoGapSA8GjiXBcgfwSfCAOo8D5gUgiaV+bFJXEmdlWEVQsVwsD5KXcGVyhU0ukURnMy4LNlTc5Evo8vkCgXEF4YCRSOTyUEgUjmoEgwXKly+qHtQlcrWknXkzFojGK2n0/gMyQyBSh2YWu281YCxoq4OWjXE7W6o2G/Xxk2pZPutNFrMOvBO/Au9Cq61en314G4BIBsEF1mczMwkvRylx42Jhn46Tp23c7P80H57tWjlhkcRsmT2PUpQJ01J2frgkZ0X2nMroNr7vbnV6g3yitTo8ztVzu+X5f91cL082uG4oPmWz4ygeVaMies5fkujpAA
    * Is there a way to let a user register custom type serialization support? Would that be runtime, or could it be compile-time?
    * Maybe I should use `AutoSerializableAttribute` instead.
    * How do I get the "target"?
        * DONE - A `BusinessListBase<,>`, what is `C`? How do I discover that at compile-time?
        * `PropertyInfo<>`. It's possible someone created their own derivation of this type, so how do I find the generic argument?
    * Different base types and different base fields to store/load. One idea is to have a set of extension methods that would load/store these values based on the type, rather than generating that code for each type. 
        * DONE - `ReadOnlyBase`
        * DONE - `ReadOnlyListBase`
        * DONE - `BusinessListBase`
            * State: 
                * `BusinessListBase`
                    * `_isChild` (bool)
                    * `_editLevel` (int)
                    * `_identity` (int)
                * `ObservableBindingList`
                    * `AllowEdit` (bool)
                    * `AllowNew` (bool)
                    * `AllowRemove` (bool)
                    * `RaiseListChangedEvents` (bool)
                    * `_supportsChangeNotificationCore` (bool)
        * DONE - `CommandBase`
        * Maybe...or defer to later
            * `ReadOnlyBindingListBase`
            * `BusinessBindingListBase`
            * `DynamicListBase`
            * `DynamicBindingListBase`
            * `MobileDictionary`
            * `MobileList`
    * Add an analyzer to warn users of unhandled types (though this may go away if I add in support for custom serialization)
* Tests (of course)
    * Ensure that tests here are covered - https://github.com/MarimerLLC/csla/tree/main/Source/Csla.test/Serialization

MobileFormatter:
| Method    | Mean     | Error     | StdDev    | Gen0   | Gen1   | Allocated |
|---------- |---------:|----------:|----------:|-------:|-------:|----------:|
| Roundtrip | 7.884 us | 0.0546 us | 0.0456 us | 1.4038 | 0.0305 |  24.14 KB |

GeneratorFormatter:
| Method    | Mean     | Error     | StdDev    | Gen0   | Allocated |
|---------- |---------:|----------:|----------:|-------:|----------:|
| Roundtrip | 2.778 us | 0.0192 us | 0.0180 us | 0.2518 |   4.26 KB |

2.84 times faster and 5.67 times less memory allocation (and all in Gen0)

Issues
* It is weird that `SerializationFormatter` is `static`, but that's probably a holdover from an earlier design, and changing it might break people. That said, I really should be able to get the configured `ISerializationFormatter` instance from DI, not as a static property.
* `Get/SetState()` and `Get/SetChildren()` appear to require `MobileFormatter`, instead of `ISerializationFormatter`, which makes it hard to do pluggable serialization