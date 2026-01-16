using Csla;

namespace ABC.CslaGeneratorSerialization.Scenarios;

public partial class WithoutAttributeNoNamespace
	: BusinessBase<WithoutAttributeNoNamespace>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<uint> AgeProperty =
		WithoutAttributeNoNamespace.RegisterProperty<uint>(nameof(WithoutAttributeNoNamespace.Age));
	public uint Age
	{
		get => this.GetProperty(WithoutAttributeNoNamespace.AgeProperty);
		set => this.SetProperty(WithoutAttributeNoNamespace.AgeProperty, value);
	}

	public static readonly PropertyInfo<string> NameProperty =
		WithoutAttributeNoNamespace.RegisterProperty<string>(nameof(WithoutAttributeNoNamespace.Name));
	public string Name
	{
		get => this.GetProperty(WithoutAttributeNoNamespace.NameProperty)!;
		set => this.SetProperty(WithoutAttributeNoNamespace.NameProperty, value);
	}
}