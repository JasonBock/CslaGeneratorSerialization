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

ReadStuffUnion(int[] typeIdentifiers, int typeIdentifierIndex)
{
    switch(typeIdentifiers[typeIdentifierIndex])
    {
        case 0:
            // It's a string
            stringreader;
            break;
        case 1:
            // It's an int
            valuetypereader;
            break;
        case 2:
            typeIdentifierIndex++;
            LoadProperty "new MoreStuff(ReadMoreStuff(typeIdentifiers, typeIdentifierIndex))"
            break;
    }
}

ReadMoreStuffUnion(int[] typeIdentifiers, int typeIdentifierIndex)
{
    switch(typeIdentifiers[typeIdentifierIndex])
    {
        case 0:
            // It's a Guid
            valuetypereader;
            break;
        case 1:
            // It's a DateTimeOffset
            valuetypereader;
            break;
    }
}

Right now, `ValueTypeBuilder.BuildReader` uses a `TypeReferenceModel` from the property to determine what kind of value type it is. For a union, we'll know based on the index value, but we'll have to figure out how to get that to the reader without using a `TypeReferenceModel` (probably).

TODO:
* Need a test for `GetFullyQualifiedName()` in `StringExtensions`
* Why does `EquatableArray<>` not like it when you assign `[]` to a value and then look at `.IsEmpty` or `.Length`?