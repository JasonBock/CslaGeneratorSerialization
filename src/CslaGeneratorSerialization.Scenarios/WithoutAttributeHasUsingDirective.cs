using Csla;
using CslaGeneratorSerialization;

namespace ABC.Scenarios;

public partial class WithoutAttributeHasUsingDirective
	: BusinessBase<WithoutAttributeHasUsingDirective>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<uint> AgeProperty =
		WithoutAttributeHasUsingDirective.RegisterProperty<uint>(nameof(WithoutAttributeHasUsingDirective.Age));
	public uint Age
	{
		get => this.GetProperty(WithoutAttributeHasUsingDirective.AgeProperty);
		set => this.SetProperty(WithoutAttributeHasUsingDirective.AgeProperty, value);
	}

	public static readonly PropertyInfo<string> NameProperty =
		WithoutAttributeHasUsingDirective.RegisterProperty<string>(nameof(WithoutAttributeHasUsingDirective.Name));
	public string Name
	{
		get => this.GetProperty(WithoutAttributeHasUsingDirective.NameProperty)!;
		set => this.SetProperty(WithoutAttributeHasUsingDirective.NameProperty, value);
	}
}