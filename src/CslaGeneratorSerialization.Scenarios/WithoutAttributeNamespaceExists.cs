using Csla;

namespace CslaGeneratorSerialization.Scenarios;

public partial class WithoutAttributeNamespaceExists
	: BusinessBase<WithoutAttributeNamespaceExists>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<uint> AgeProperty =
		WithoutAttributeNamespaceExists.RegisterProperty<uint>(nameof(WithoutAttributeNamespaceExists.Age));
	public uint Age
	{
		get => this.GetProperty(WithoutAttributeNamespaceExists.AgeProperty);
		set => this.SetProperty(WithoutAttributeNamespaceExists.AgeProperty, value);
	}

	public static readonly PropertyInfo<string> NameProperty =
		WithoutAttributeNamespaceExists.RegisterProperty<string>(nameof(WithoutAttributeNamespaceExists.Name));
	public string Name
	{
		get => this.GetProperty(WithoutAttributeNamespaceExists.NameProperty)!;
		set => this.SetProperty(WithoutAttributeNamespaceExists.NameProperty, value);
	}
}