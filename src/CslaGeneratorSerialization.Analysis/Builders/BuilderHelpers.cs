using CslaGeneratorSerialization.Analysis.Models;
using Microsoft.CodeAnalysis;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class BuilderHelpers
{
	internal static string GetLoadProperty(SerializationItemModel item, string readerInvocation) =>
		$"this.LoadProperty({item.PropertyInfoContainingType.FullyQualifiedName}.{item.PropertyInfoFieldName}, {readerInvocation});";

	internal static string GetReadOperation(TypeReferenceModel type)
	{
		// Unions
		if (type.UnionCaseTypes.Length > 0)
		{
			return $"context.ReadUnion<{type.FullyQualifiedName}>(context.Reader.ReadByteArray(), 0)";
		}

		// Stereotype
		if (type.BusinessObjectKind != StereotypeKind.None)
		{
			return type.ParticipatesInGeneratorSerialization ?
				$$"""context.Read<{{type.FullyQualifiedNameNoNullableAnnotation}}>({{type.IsSealed.ToString().ToLower()}})!""" :
				$$"""context.ReadMobileObject<{{type.FullyQualifiedNameNoNullableAnnotation}}>()!""";
		}

		// "Special" value types that we can easily handle.
		if (type.FullyQualifiedName == "global::System.Guid")
		{
			return "new global::System.Guid(context.Reader.ReadBytes(16))";
		}
		if (type.FullyQualifiedName == "global::System.Decimal")
		{
			return "new decimal(new [] { context.Reader.ReadInt32(), context.Reader.ReadInt32(), context.Reader.ReadInt32(), context.Reader.ReadInt32() })";
		}
		if (type.FullyQualifiedName == "global::System.TimeSpan")
		{
			return "new global::System.TimeSpan(context.Reader.ReadInt64())";
		}
		if (type.FullyQualifiedName == "global::System.DateTimeOffset")
		{
			return "new global::System.DateTimeOffset(context.Reader.ReadInt64(), new global::System.TimeSpan(context.Reader.ReadInt64()))";
		}
		if (type.FullyQualifiedName == "global::System.Half")
		{
			return "context.Reader.ReadHalf()";
		}
		if (type.FullyQualifiedName == "global::System.Numerics.BigInteger")
		{
			return "context.Reader.ReadBigInteger()";
		}

		// Array
		if (type.IsSupportedArray)
		{
			var elementSpecialType = type.Array!.ElementType.SpecialType;
			var readType = elementSpecialType == SpecialType.System_Byte ? "Byte" : "Char";
			return $"context.Reader.Read{readType}Array()";
		}

		// Nullable value
		if (type.IsNullable && type.IsValueType)
		{
			// This is because a nullable, like "byte?",
			// is actually a "Nullable<byte>" -
			// hence that's why we're pulling out the 0th
			// type argument.
			var nullableType = type.TypeArguments[0];
			var enumCast = string.Empty;

			if (nullableType.TypeKind == TypeKind.Enum)
			{
				nullableType = nullableType.EnumUnderlyingType!;
				enumCast = $"({type.TypeArguments[0].FullyQualifiedName})";
			}

			return $"{enumCast}{BuilderHelpers.GetReadOperation(nullableType)}";
		}

		// Common value types -
		// note that we fall through to a custom serialization
		// if nothing matches
		return type.SpecialType switch
		{
			SpecialType.System_Boolean => "context.Reader.ReadBoolean()",
			SpecialType.System_Char => "context.Reader.ReadChar()",
			SpecialType.System_String => "context.Reader.ReadString()",
			SpecialType.System_SByte => "context.Reader.ReadSByte()",
			SpecialType.System_Byte => "context.Reader.ReadByte()",
			SpecialType.System_Int16 => "context.Reader.ReadInt16()",
			SpecialType.System_UInt16 => "context.Reader.ReadUInt16()",
			SpecialType.System_Int32 => "context.Reader.ReadInt32()",
			SpecialType.System_UInt32 => "context.Reader.ReadUInt32()",
			SpecialType.System_Int64 => "context.Reader.ReadInt64()",
			SpecialType.System_UInt64 => "context.Reader.ReadUInt64()",
			SpecialType.System_Single => "context.Reader.ReadSingle()",
			SpecialType.System_Double => "context.Reader.ReadDouble()",
			SpecialType.System_Decimal => "context.Reader.ReadDecimal()",
			SpecialType.System_DateTime => "new global::System.DateTime(context.Reader.ReadInt64())",
			_ => $"context.ReadCustom<{type.FullyQualifiedNameNoNullableAnnotation}>()"
		};
	}
}