# n-level Undo Does Not Work

Main issue is that `BusinessRules` will need to be serialized, and my formatter was throwing because it didn't implement `IGeneratorSerializable`. So, I added code to use `MobileFormatter`, but then custom serialization didn't work because `MobileFormatter` doesn't know how to handle custom serialization that was registered for my formatter.

Here's the idea to solve this:
* Create an implementation of `ITypeMap`, `MobileTypeMap<T>`, that is `internal`.
    * `SerializerType` returns `typeof(T)`
    * `CanSerialize` returns `(t) => t == typeof(T)`
* Create an implementation of `IMobileSerializer`, `MobileCustomSerializer<T>`, that's `internal`.
    * On construction, this takes a reference to `CustomSerialization<T>`. 
    * `Serialize()` and `Deserialize()` are implemented by using the `CustomSerialization<T>` instance, giving it the right reader or writer, getting the byte stream or instance, and using `SerializationInfo` to get or set the value.
* Create an `abstract` `CustomSerialization` class that `CustomSerialization<T>` derives from.
    * Add a `readonly` `abstract` `TypeMap` property to `CustomSerialization`.
* Implement `TypeMap` in `CustomSerialization<T>` by returning an instance of `MobileTypeMap<T>`.
* Change `AddCslaGeneratorSerialization()` to take `params ReadOnlySpan<CustomSerialization> customSerializations`
    * Create a `MobileFormatterOptions` instance
    * For each, get the `TypeMap` and call `Add()` on `CustomSerializers`.
    * Register the `MobileFormatterOptions`

If this works, we've basically adapted our custom serializers to what CSLA wants when needed. These adapters should **only** be used when `MobileFormatter` is used.

TODO:
* I wonder if extensions would solve this cleanly, in that `BusinessRules` could be "extended" to implement `IGeneratorSerializable`.