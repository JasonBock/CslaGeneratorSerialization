# How to Make a Serialization Source Generator in CSLA

First, `ApplicationContext.SerializationFormatter` needs to be set immediately. Actually, it's `internal`, so you can't, but it is set through `AddCsla()`. Look at the `SerializationTests.UseCustomFormatter()` test. It is weird that `SerializationFormatter` is `static`, but that's probably a holdover from an earlier design, and changing it might break people. That said, I really should be able to get the configured `ISerializationFormatter` instance from DI, not as a static property.

Then, I need to create a class that derives from `ISerializationFormatter`. Call it `GeneratorSerializationFormatter`. This will not be generated, as it will assume things that are done with the SG.

Next, create an interface called `IGeneratorSerialization`. This will be implemented on each BO that is serializable. Therefore, each BO must be a `partial` type. This would be a big breaking change with CSLA in terms of how people define BOs, but hopefully there's a performance payoff that will justify it.

The SG will look for types that are serializable. `[Serializable]` isn't enough if I remember right. It must derive from `IMobileObject`. `IsBusinessObjectSerializableAnalyzer` enforces that `[Serializable]` is on a BO (`CSLA0001`), so we can use `ForAttributeWithMetadataName` to look for occurrences of that attribute, and then check that it derives from `IMobileObject` (see `ITypeSymbolExtensions.IsMobileObject()`). If it does, then we need to grab information about that object that will used to create the partial type.

What specifically do we need to grab?