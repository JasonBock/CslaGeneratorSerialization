# Business Object Does Not Have GeneratorSerializableAttribute
If the given type does not have `[GeneratorSerializable]`, this diagnostic is created.
```c#
// This is fine.
using CslaGeneratorSerialization;

[GeneratorSerializable]
public partial class Customer
  : BusinessBase<Customer>
{
  [Create]
  private void Create() { }

  public static readonly PropertyInfo<uint> AgeProperty =
    Customer.RegisterProperty<uint>(nameof(Customer.Age));
  public uint Age
  {
    get => this.GetProperty(Customer.AgeProperty);
    set => this.SetProperty(Customer.AgeProperty, value);
  }
}

// This will generate CGSA1
public class Customer
  : BusinessBase<Customer>
{
  [Create]
  private void Create() { }

  public static readonly PropertyInfo<uint> AgeProperty =
    Customer.RegisterProperty<uint>(nameof(Customer.Age));
  public uint Age
  {
    get => this.GetProperty(Customer.AgeProperty);
    set => this.SetProperty(Customer.AgeProperty, value);
  }
}
```

## Code Fix

There is a code fix for this analyzer. It will add the attribute, create the `using` directive if needed, and make the business object `partial` if it isn't already.