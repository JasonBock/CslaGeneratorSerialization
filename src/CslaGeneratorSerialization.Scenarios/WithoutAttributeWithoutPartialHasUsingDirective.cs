using Csla;
using CslaGeneratorSerialization;

namespace ABC.Scenarios;

public class WithoutAttributeWithoutPartialHasUsingDirective
	: BusinessBase<WithoutAttributeWithoutPartialHasUsingDirective>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<uint> AgeProperty =
		WithoutAttributeWithoutPartialHasUsingDirective.RegisterProperty<uint>(nameof(WithoutAttributeWithoutPartialHasUsingDirective.Age));
	public uint Age
	{
		get => this.GetProperty(WithoutAttributeWithoutPartialHasUsingDirective.AgeProperty);
		set => this.SetProperty(WithoutAttributeWithoutPartialHasUsingDirective.AgeProperty, value);
	}

	public static readonly PropertyInfo<string> NameProperty =
		WithoutAttributeWithoutPartialHasUsingDirective.RegisterProperty<string>(nameof(WithoutAttributeWithoutPartialHasUsingDirective.Name));
	public string Name
	{
		get => this.GetProperty(WithoutAttributeWithoutPartialHasUsingDirective.NameProperty)!;
		set => this.SetProperty(WithoutAttributeWithoutPartialHasUsingDirective.NameProperty, value);
	}
}