# n-level Undo Does Not Work

Main issue is that `BusinessRules` will need to be serialized, and my formatter was throwing because it didn't implement `IGeneratorSerializable`. So, I added code to use `MobileFormatter`, but then custom serialization didn't work because `MobileFormatter` doesn't know how to handle custom serialization that was registered for my formatter.

Here's the idea to solve this:
* DONE - Create an implementation of `IMobileSerializer`, `MobileCustomSerializer<T>`, that's `public`. 
    * DONE - Provide a constructor that takes a `CustomSerialization<T>` instance.
    * DONE - `Serialize()` and `Deserialize()` are implemented by using a `CustomSerialization<T>` instance, giving it the right reader or writer, getting the byte stream or instance, and using `SerializationInfo` to get or set the value.
* DONE - Create an implementation of `ITypeMap`, `MobileTypeMap<T>`, that is `internal`.
    * DONE - `SerializerType` returns `typeof(MobileCustomSerializer<T>)`
    * DONE - `CanSerialize` returns `(t) => t == typeof(T)`
* DONE - Create an `abstract` `CustomSerialization` class that `CustomSerialization<T>` derives from.
    * DONE - Add a `readonly` `abstract` `TypeMap` property to `CustomSerialization`.
* DONE - Implement `TypeMap` in `CustomSerialization<T>` by returning an instance of `MobileTypeMap<T>`.
* DONE - Change `AddCslaGeneratorSerialization()` to take `params ReadOnlySpan<CustomSerialization> customSerializations`
    * DONE - Create a `MobileFormatterOptions` instance
    * DONE - For each `customSerialization`
        * DONE - Get the `TypeMap` and call `Add()` on `CustomSerializers`.
        * DONE - Do `AddSingleton(customSerialization.GetType(), customSerialization)`
    * DONE - Register the `MobileFormatterOptions`

If this works, we've basically adapted our custom serializers to what CSLA wants when needed. These adapters should **only** be used when `MobileFormatter` is used.

* Write tests for:
    * DONE - `MobileCustomSerializer<T>`
    * DONE - `MobileTypeMap<T>`
    * DONE - `CustomSerialization<T>`
    * `IServiceCollectionExtensions`, specifically `AddCslaGeneratorSerialization()`

TODO:
* I wonder if extensions would solve this cleanly, in that `BusinessRules` could be "extended" to implement `IGeneratorSerializable`.