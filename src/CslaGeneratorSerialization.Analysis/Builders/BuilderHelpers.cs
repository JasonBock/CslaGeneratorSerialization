using CslaGeneratorSerialization.Analysis.Models;
using Microsoft.CodeAnalysis;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class BuilderHelpers
{
	// TODO: This can probably go away...
	internal static string GetLoadProperty(SerializationItemModel item, string readerInvocation) =>
		$"this.LoadProperty({item.PropertyInfoContainingType.FullyQualifiedName}.{item.PropertyInfoFieldName}, {readerInvocation});";

	internal static string GetReadOperation(TypeReferenceModel valueType)
	{
		if (valueType.FullyQualifiedName == "global::System.Guid")
		{
			return "new global::System.Guid(context.Reader.ReadBytes(16))";
		}
		if (valueType.FullyQualifiedName == "global::System.Decimal")
		{
			return "new decimal(new [] { context.Reader.ReadInt32(), context.Reader.ReadInt32(), context.Reader.ReadInt32(), context.Reader.ReadInt32() })";
		}
		if (valueType.FullyQualifiedName == "global::System.TimeSpan")
		{
			return "new global::System.TimeSpan(context.Reader.ReadInt64())";
		}
		if (valueType.FullyQualifiedName == "global::System.DateTimeOffset")
		{
			return "new global::System.DateTimeOffset(context.Reader.ReadInt64(), new global::System.TimeSpan(context.Reader.ReadInt64()))";
		}

		return valueType.SpecialType switch
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
			SpecialType.System_DateTime => "new DateTime(context.Reader.ReadInt64())",
			_ => $"context.ReadCustom<{valueType.FullyQualifiedNameNoNullableAnnotation}>()"
		};
	}
}