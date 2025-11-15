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
* Ask Rocky why doesn't `ReadOnlyListBase` implement `IMobileObjectMetastate`, because it has `IsReadOnly`. A serialization author shouldn't be responsible for that. Currently made an issue for it - https://github.com/MarimerLLC/csla/issues/4765.
* Any code that is doing these things (which also means Reflection in some cases) should be removed to look for the `Csla.Serialization.Mobile.IMobileObjectMetastate` interface and call it with a cast to ensure we are calling it the "right" way:
    * `BusinessListBaseAccessors`
    * `IsXYZ` (e.g. `IsNew`) or `DisableIEditableObject`
* DONE - Need to figure what the issue is with integration tests and referencing `Csla`.
* This is currently generating `Read/WriteCustom()`, and what it should be doing is handling it as an array:
```c#
[GeneratorSerializable]
public sealed partial class Data
	: BusinessBase<Data>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<int[]> ValueProperty =
		RegisterProperty<int[]>(nameof(Data.Value));
#pragma warning disable CA1819 // Properties should not return arrays
	public int[] Value
#pragma warning restore CA1819 // Properties should not return arrays
	{
		get => this.GetProperty(Data.ValueProperty)!;
		set => this.SetProperty(Data.ValueProperty, value);
	}
}
```

This is what the gen'd code looks like:

```c#
public sealed partial class Data
	: global::CslaGeneratorSerialization.IGeneratorSerializable
{
	void global::CslaGeneratorSerialization.IGeneratorSerializable.SetState(global::CslaGeneratorSerialization.GeneratorFormatterWriterContext context)
	{
		// global::CslaGeneratorSerialization.IntegrationTests.Collections.CustomTestsDomain.Data.ValueProperty
		context.WriteCustom<int[]>(this.ReadProperty<int[]>(global::CslaGeneratorSerialization.IntegrationTests.Collections.CustomTestsDomain.Data.ValueProperty));
		
		var metastate = ((global::Csla.Serialization.Mobile.IMobileObjectMetastate)this).GetMetastate();
		context.Writer.Write((metastate.Length, metastate));
	}
	
	void global::CslaGeneratorSerialization.IGeneratorSerializable.GetState(global::CslaGeneratorSerialization.GeneratorFormatterReaderContext context)
	{
		// global::CslaGeneratorSerialization.IntegrationTests.Collections.CustomTestsDomain.Data.ValueProperty
		this.LoadProperty(global::CslaGeneratorSerialization.IntegrationTests.Collections.CustomTestsDomain.Data.ValueProperty, context.ReadCustom<int[]>());
		
		((global::Csla.Serialization.Mobile.IMobileObjectMetastate)this).SetMetastate(context.Reader.ReadByteArray());
	}
}
```

System.InvalidOperationException
  HResult=0x80131509
  Message=SerializationInfo.GetValue: _bindingEdit
  Source=Csla
  StackTrace:
   at Csla.Serialization.Mobile.SerializationInfo.GetValue[T](String name)
   at Csla.Core.UndoableBase.OnSetState(SerializationInfo info, StateMode mode)
   at Csla.Core.BusinessBase.OnSetState(SerializationInfo info, StateMode mode)
   at Csla.Core.MobileObject.Csla.Serialization.Mobile.IMobileObject.SetState(SerializationInfo info)
   at Csla.Serialization.Mobile.MobileObjectMetastateHelper.DeserializeMetastate(IMobileObject mobileObject, Byte[] metastate)
   at Csla.Core.MobileObject.Csla.Serialization.Mobile.IMobileObjectMetastate.SetMetastate(Byte[] metastate)
   at CslaGeneratorSerialization.IntegrationTests.Collections.ByteArrayTestsDomain.ByteArrayData.global::CslaGeneratorSerialization.IGeneratorSerializable.GetState(GeneratorFormatterReaderContext context) in C:\Users\jason\source\repos\JasonBock\CslaGeneratorSerialization\src\artifacts\obj\CslaGeneratorSerialization.IntegrationTests\debug\CslaGeneratorSerialization.Analysis\CslaGeneratorSerialization.Analysis.GeneratorSerializationGenerator\CslaGeneratorSerialization.IntegrationTests.Collections.ByteArrayTestsDomain.ByteArrayData_GeneratorSerialization.g.cs:line 39
   at CslaGeneratorSerialization.GeneratorFormatter.Deserialize(Stream serializationStream) in /_/src/CslaGeneratorSerialization/GeneratorFormatter.cs:line 26
   at CslaGeneratorSerialization.IntegrationTests.Collections.ByteArrayTestsDomain.ByteArrayTests.<RoundtripAsync>d__0.MoveNext() in C:\Users\jason\source\repos\JasonBock\CslaGeneratorSerialization\src\CslaGeneratorSerialization.IntegrationTests\Collections\ByteArrayTests.cs:line 39
   at TUnit.Core.TestMetadata`1.<>c__DisplayClass13_0.<<get_CreateExecutableTestFactory>b__2>d.MoveNext()
   at TUnit.Core.ExecutableTest.<InvokeTestAsync>d__4.MoveNext()
   at TUnit.Engine.TestExecutor.<ExecuteTestAsync>d__8.MoveNext()
   at TUnit.Engine.TestExecutor.<ExecuteAsync>d__7.MoveNext()

  This exception was originally thrown at this call stack:
    [External Code]

Inner Exception 1:
InvalidCastException: Object must implement IConvertible.
