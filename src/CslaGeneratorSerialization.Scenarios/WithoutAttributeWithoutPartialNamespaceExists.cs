using Csla;

namespace CslaGeneratorSerialization.Scenarios;

public class WithoutAttributeWithoutPartialNamespaceExists
	: BusinessBase<WithoutAttributeWithoutPartialNamespaceExists>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<uint> AgeProperty =
		WithoutAttributeWithoutPartialNamespaceExists.RegisterProperty<uint>(nameof(WithoutAttributeWithoutPartialNamespaceExists.Age));
	public uint Age
	{
		get => this.GetProperty(WithoutAttributeWithoutPartialNamespaceExists.AgeProperty);
		set => this.SetProperty(WithoutAttributeWithoutPartialNamespaceExists.AgeProperty, value);
	}

	public static readonly PropertyInfo<string> NameProperty =
		WithoutAttributeWithoutPartialNamespaceExists.RegisterProperty<string>(nameof(WithoutAttributeWithoutPartialNamespaceExists.Name));
	public string Name
	{
		get => this.GetProperty(WithoutAttributeWithoutPartialNamespaceExists.NameProperty)!;
		set => this.SetProperty(WithoutAttributeWithoutPartialNamespaceExists.NameProperty, value);
	}
}