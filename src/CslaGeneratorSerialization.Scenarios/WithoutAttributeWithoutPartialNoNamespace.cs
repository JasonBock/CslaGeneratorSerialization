using Csla;

namespace ABC.CslaGeneratorSerialization.Scenarios;

public class WithoutAttributeWithoutPartialNoNamespace
	: BusinessBase<WithoutAttributeWithoutPartialNoNamespace>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<uint> AgeProperty =
		WithoutAttributeWithoutPartialNoNamespace.RegisterProperty<uint>(nameof(WithoutAttributeWithoutPartialNoNamespace.Age));
	public uint Age
	{
		get => this.GetProperty(WithoutAttributeWithoutPartialNoNamespace.AgeProperty);
		set => this.SetProperty(WithoutAttributeWithoutPartialNoNamespace.AgeProperty, value);
	}

	public static readonly PropertyInfo<string> NameProperty =
		WithoutAttributeWithoutPartialNoNamespace.RegisterProperty<string>(nameof(WithoutAttributeWithoutPartialNoNamespace.Name));
	public string Name
	{
		get => this.GetProperty(WithoutAttributeWithoutPartialNoNamespace.NameProperty)!;
		set => this.SetProperty(WithoutAttributeWithoutPartialNoNamespace.NameProperty, value);
	}
}