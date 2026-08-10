I've got problems:
* `GetUnionCaseTypes()` won't stop if there are recursive unions
* On the reader side, I need to know what the type is, so I'll have to store a count/ID value before the actual value
* Even with that, I'll also need to construct the type if it's another union type
* I **may** have an "out" - https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview6/libraries.md#systemtextjson-serializes-c-union-types

OK...
* JSON serialization didn't work
* I think I need tracking to determine what value is serialized (and when, if there are recursive unions)

public union Stuff(string, int, MoreStuff)
public union MoreStuff(Guid, DateTimeOffset)

array of type identifiers - for example:

"new Stuff("abc")" -> [0]
"new Stuff(new MoreStuff(1234bacd...))" -> [2, 0]

void WriteUnion(this GeneratorFormatterWriterContext self, Stuff value, List<byte> typeIdentifiers)
{
    var context = self;

    switch(value)
    {
        case string u0:
            // It's a string
            typeIdentifiers.Add(0);
            context.Write(typeIdentifiers.ToArray());
            context.Writer.Write(u0);
            break;
        case int u1:
            typeIdentifiers.Add(1);
            context.Write(typeIdentifiers.ToArray());
            context.Writer.Write(u1);
            break;
        case MoreStuff u2:
            typeIdentifiers.Add(2);
            context.WriteUnion(u2, typeIdentifiers);
            break;
    }
}

void WriteUnion(this GeneratorFormatterWriterContext self, MoreStuff value, List<byte> typeIdentifiers)
{
    var context = self;

    switch(value)
    {
        case Guid u0:
            // It's a Guid
            typeIdentifiers.Add(0);
            context.Write(typeIdentifiers.ToArray());
            // Write the Guid valuetype
            context.Writer.Write(u0);
            break;
        case DateTimeOffset u1:
            typeIdentifiers.Add(1);
            context.Write(typeIdentifiers.ToArray());
            // Write the DateTimeOffset valuetype
            context.Writer.Write(u1);
            break;
    }
}


Stuff ReadStuffUnion(this GeneratorFormatterReaderContext self, byte[] typeIdentifiers, int typeIdentifierIndex)
{
    switch(typeIdentifiers[typeIdentifierIndex])
    {
        case 0:
            // It's a string
            return new Stuff(stringreader);
            break;
        case 1:
            // It's an int
            return new Stuff(valuetypereader);
            break;
        case 2:
            typeIdentifierIndex++;
            return new Stuff(ReadMoreStuff(typeIdentifiers, typeIdentifierIndex))"
            break;
    }
}

MoreStuff ReadMoreStuffUnion(this GeneratorFormatterReaderContext self, byte[] typeIdentifiers, int typeIdentifierIndex)
{
    switch(typeIdentifiers[typeIdentifierIndex])
    {
        case 0:
            // It's a Guid
            return new MoreStuff(valuetypereader);
            break;
        case 1:
            // It's a DateTimeOffset
            return new MoreStuff(valuetypereader);
            break;
    }
}

Actually, more like this for the reader:

```c#
public static class GeneratorFormatterWriterContextExtensions
{
    extension(GeneratorFormatterWriterContext context)
    {
        public void WriteUnion(Stuff value, List<byte> typeIdentifiers)
        {
            switch(value)
            {
                case string u0:
                    typeIdentifiers.Add(0);
                    context.Write(typeIdentifiers.ToArray());
                    if (u0 is not null)
                    {
                        context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Value);
                        context.Writer.Write(u0);
                    }
                    else
                    {
                        context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Null);
                    }
                    break;
                case int u1:
                    typeIdentifiers.Add(1);
                    context.Write(typeIdentifiers.ToArray());
                    context.Writer.Write(u1);
                    break;
                case MoreStuff u2:
                    typeIdentifiers.Add(2);
                    context.WriteUnion(u2, typeIdentifiers);
                    break;
            }
        }

        public void WriteUnion(MoreStuff value, List<byte> typeIdentifiers)
        {
            switch(value)
            {
                case Guid u0:
                    typeIdentifiers.Add(0);
                    context.Write(typeIdentifiers.ToArray());
                    context.Writer.Write(u0);
                    break;
                case DateTimeOffset u1:
                    typeIdentifiers.Add(1);
                    context.Write(typeIdentifiers.ToArray());
                    context.Writer.Write(u1);
                    break;
            }
        }        
    }
}

public static class GeneratorFormatterReaderContextExtensions
{
    extension(GeneratorFormatterReaderContext context)
    {
        public object ReadUnion<T>(byte[] typeIdentifiers, int typeIdentifierIndex)
        {
            if (typeof(T) == typeof(Stuff))
            {
                switch(typeIdentifiers[typeIdentifierIndex])
                {
                    case 0:
                        // string
                        if (context.Reader.ReadStateValue() == global::CslaGeneratorSerialization.SerializationState.Value)
                        {
                            return (Stuff)context.ReadString();                            
                        }

                        return (Stuff)(null as string?);
                    case 1:
                        // int
                        return (Stuff)context.Reader.ReadInt32();
                    case 2:
                        // MoreStuff
                        typeIdentifierIndex++;
                        return (Stuff)(MoreStuff)context.ReadUnion<MoreStuff>(typeIdentifiers, typeIdentifierIndex);
                    default:
                        throw new global::System.NotSupportedException($"Unexpected case identifier for type Stuff at index {typeIdentifierIndex}: {typeIdentifiers[typeIdentifierIndex]}");
                }
            }
            
            if (typeof(T) == typeof(MoreStuff))
            {
                switch(typeIdentifiers[typeIdentifierIndex])
                {
                    case 0:
                        // Guid
                        return (MoreStuff)(new global::System.Guid(context.Reader.ReadBytes(16)));                            
                    case 1:
                        // DateTimeOffset
                        return (MoreStuff)(new global::System.DateTimeOffset(context.Reader.ReadInt64(), new global::System.TimeSpan(context.Reader.ReadInt64())));
                    default:
                        throw new global::System.NotSupportedException($"Unexpected case identifier at index {typeIdentifierIndex}: {typeIdentifiers[typeIdentifierIndex]}");
                }
            }
        }
    }
}
```

Right now, `ValueTypeBuilder.BuildReader` uses a `TypeReferenceModel` from the property to determine what kind of value type it is. For a union, we'll know based on the index value, but we'll have to figure out how to get that to the reader without using a `TypeReferenceModel` (probably).

OK, reset...

* DONE - I'll assume for writers that there will be an overloaded `WriteUnion(UnionType, ...)` methods and a `Read<TUnion>(...)` method. These will be extension methods put on the readers and writers.
* DONE - Update all readers in `OperationBuilder.BuildReadOperation()` so the "read" can be "reused"
* DONE - Rename the current `BuildReader()` methods to `BuildPropertyReader()`
* DONE -Add a `BuildUnionReader()` to handle union creation, along with possible nullable values passed into the union constructor
* DONE - Generate a separate .cs extension files for the readers and writers mentioned above. I can use the property item types as the root union types, and add other nested union types as needed.
    * DONE -The readers for some types, like array, will need to be changed so the property assignment can be separated out.

TODO:
* Ask the compiler team why this is OK: `public union Stuff(string)`, why would you have a union of just one case type?
* Need to address recursive `TypeReferenceModel`
    * Need tests for recursive unions
    * Need tests for recursive types
* DONE - `OperationBuilder.BuildReadOperation()` - `itemId` isn't used.
* DONE - Why are we doing casts in the `BuildWriter()` methods?
* DONE - Why can't we push all logic into methods on the reader and writer contexts? e.g. look at `StringBuilder.BuildWriter()`.
* DONE - Why does `EquatableArray<>` not like it when you assign `[]` to a value and then look at `.IsEmpty` or `.Length`?
* DONE - Need tests for 
    * DONE - `GetFullyQualifiedName()` in `StringExtensions`
    * DONE - Write unit and integration tests for these cases:
        * DONE - Union with multiple case types (common scenario)
        * DONE - All the possible types from `OperationBuilder` (`string`, custom type, enum, child union, etc.)
        * DONE - BO has multiple properties with:
            * DONE - Different union types
            * DONE - Shared union types
* I think custom types need nullability checks in place. Actually, I think if there are **any** nullable value types or reference types (null or not), I think we need to do the trick to put `case null:` in first, and use the (count of union case types + 1) trick.
* Given the little "hack" I did for nullable value types by using the (count of union case types + 1) as the "marker" for the null value, maybe I add native support for `uint[]`, and then I'm not limited to 254 union case types because I can use a `uint[]` and it'll work (though seriously, who is going to create a union with over 4 billion union case types?)
* Can we make a `IBuilder` interface with the static methods `BuildWriter()`, `BuildPropertyReader()`, and `BuildUnionReader()`, and have all the builders implement that so we're consistent?

FUTURE:
* Union null testing. This is problematic right now because the default `union` is a `struct`, but people can make custom unions that are `class`-based. So trying to figure out if it's a `Nullable<MyUnion>` or `MyUnion?` and specifically the union case types...I should wait until Preview 7 to see if the `UnionCaseTypes` collection is added (https://github.com/dotnet/roslyn/pull/84707) - I'm guessing it will be, at the very least, it should be in by .NET 11 final release.