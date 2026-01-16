using Csla;

namespace CslaGeneratorSerialization.Scenarios;

[GeneratorSerializable]
public partial class HasAttribute
	: BusinessBase<HasAttribute>
{
	[Create]
	private void Create() { }

	public static readonly PropertyInfo<uint> AgeProperty =
		HasAttribute.RegisterProperty<uint>(nameof(HasAttribute.Age));
	public uint Age
	{
		get => this.GetProperty(HasAttribute.AgeProperty);
		set => this.SetProperty(HasAttribute.AgeProperty, value);
	}

	public static readonly PropertyInfo<string> NameProperty =
		HasAttribute.RegisterProperty<string>(nameof(HasAttribute.Name));
	public string Name
	{
		get => this.GetProperty(HasAttribute.NameProperty)!;
		set => this.SetProperty(HasAttribute.NameProperty, value);
	}
}