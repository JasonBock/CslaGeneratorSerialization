So, when I read/write managed properties, I use `GeneratorFormatterWriterContext.Write()` and `GeneratorFormatterReaderContext.Read()`, which will handle `null` values. I also have tests to cover this, like `ChildBusinessObjectTests.RoundtripWithNullable()`. So I think the problem is the code that I'm generated, which is breaking some NRT rules.

When I generate a `Write()` call on `BinaryWriter` for a reference type:

```c#
context.Writer.Write(this.ReadProperty<string>(global::Domains.Customer.NameProperty));

this.LoadProperty(global::Domains.Customer.NameProperty, context.Reader.ReadString());
```

I need to be more defensive now:

```c#
context.Writer.Write<string>(this.ReadProperty<string>(global::Domains.Customer.NameProperty));

this.LoadProperty(global::Domains.Customer.NameProperty, context.Reader.Read<string>(context.Reader.ReadString));

public static class BinaryReaderExtensions
{
    public static T? Read<T>(this BinaryReader @self, Func<T> reader) where T : class
    {
        var state = (SerializationState)@self.ReadByte();

        if (state == SerializationState.Value)
        {
            return reader();
        }
        else
        {
            return null;
        }
    }
}

public static class BinaryWriterExtensions
{
    public static void Write<T>(this BinaryWriter @self, T? value) where T : class
    {
        if (value is not null)
        {
            @self.Write(SerializationState.Value);
            @self.Write(value);
        }
        else
        {
            @self.Write(SerializationState.Null);
        }
    }
}
```

Since I know in the "read" case what I should call if it's not `null`, I can pass that in as a delegate to get the real data.

I'm not sure I have tests around **this** scenario. The test I mention about tests for a `null` child BO, not a `null` property **on** the BO.

I think there's an error with the Reflection code I emit now, but since I'm going to remove it anyway, I'm not going to worry about it.

Another thought...instead of having the `Nullable` extensions on `BinaryReader` and `BinaryWriter`, inline the code to something like, if the managed backing field is a reference type (`string`, `byte[]` or `char[]`), then we inline the check for `null` and do the appropriate actions, rather than passing in delegates. Seems like it'll be quicker that way, and there really are only a handful of types that we can pass into these readers and writers - i.e. there's no `Write(object)` call.

Read inline:

```c#
var state = (SerializationState)self.ReadByte();

if (state == SerializationState.Value)
{
	return reader();
}
else
{
	return null;
}
```

Write inline:

```c#
if (value is not null)
{
	self.Write((byte)SerializationState.Value);
	writer(value!);
}
else
{
	self.Write((byte)SerializationState.Null);
}
```

So what do we need to remove?

* Pretty much all of the builders to update their nullable read/write. Look at `GeneratorSerializationBuilder` as the start:
    * DONE - BusinessBase
    * DONE - BusinessListBase
    * DONE - CommandBase
    * DONE - ReadOnlyBase
    * DONE - ReadOnlyListBase
* Ask Rocky why doesn't `ReadOnlyListBase` implement `IMobileObjectMetastate`, because it has `IsReadOnly`. A serialization author shouldn't be responsible for that.
* Any code that is doing these things (which also means Reflection in some cases) should be removed to look for the `Csla.Serialization.Mobile.IMobileObjectMetastate` interface and call it with a cast to ensure we are calling it the "right" way:
    * `BusinessListBaseAccessors`
    * `IsXYZ` (e.g. `IsNew`) or `DisableIEditableObject`
* Need to figure what the issue is with integration tests and referencing `Csla`.