using Csla;

namespace CslaGeneratorSerialization.Scenarios;

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

	public static readonly PropertyInfo<string> NameProperty =
		Customer.RegisterProperty<string>(nameof(Customer.Name));
	public string Name
	{
		get => this.GetProperty(Customer.NameProperty)!;
		set => this.SetProperty(Customer.NameProperty, value);
	}
}