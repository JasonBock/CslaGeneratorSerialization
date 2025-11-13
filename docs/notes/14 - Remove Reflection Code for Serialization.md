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