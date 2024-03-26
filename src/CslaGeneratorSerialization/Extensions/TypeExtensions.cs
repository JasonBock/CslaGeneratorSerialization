using System.Reflection;

namespace CslaGeneratorSerialization.Extensions;

public static class TypeExtensions
{
	public static FieldInfo? GetField(this Type self, string name)
	{
		var baseType = self;

		while (baseType is not null)
		{
			var field = baseType.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);

			if (field is not null)
			{
				return field;
			}

			baseType = baseType.BaseType;
		}

		return null;
	}
}