using System.Reflection;

namespace CslaGeneratorSerialization.Extensions;

public static class TypeExtensions
{
	public static FieldInfo? GetFieldInHierarchy(this Type self, string name)
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

	public static PropertyInfo? GetPropertyInHierarchy(this Type self, string name)
	{
		var baseType = self;

		while (baseType is not null)
		{
			var property = baseType.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			if (property is not null)
			{
				return property.DeclaringType.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			}

			baseType = baseType.BaseType;
		}

		return null;
	}
}