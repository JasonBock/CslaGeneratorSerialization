using CslaGeneratorSerialization.Extensions;
using CslaGeneratorSerialization.Models;
using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Builders;

internal static class BusinessBaseBuilder
{
	internal static void Build(IndentedTextWriter indentWriter, SerializationModel model)
	{
		indentWriter.Indent++;
		BusinessBaseBuilder.BuildWriter(indentWriter, model);
		indentWriter.WriteLine();
		BusinessBaseBuilder.BuildReader(indentWriter, model);
		indentWriter.Indent--;
	}

	private static void BuildReader(IndentedTextWriter indentWriter, SerializationModel model)
	{
		indentWriter.WriteLines(
			"""
			void global::CslaGeneratorSerialization.IGeneratorSerializable.GetState(global::CslaGeneratorSerialization.GeneratorFormatterReaderContext context)
			{
			""");
		indentWriter.Indent++;

		foreach (var item in model.Items)
		{
			BusinessBaseBuilder.BuildReadOperation(indentWriter, item);
			indentWriter.WriteLine();
		}

		indentWriter.WriteLines(
			"""
			var type = this.GetType();
			type.GetFieldInHierarchy("_isNew")!.SetValue(this, context.Reader.ReadBoolean());
			type.GetFieldInHierarchy("_isDeleted")!.SetValue(this, context.Reader.ReadBoolean());
			type.GetFieldInHierarchy("_isDirty")!.SetValue(this, context.Reader.ReadBoolean());
			type.GetFieldInHierarchy("_isChild")!.SetValue(this, context.Reader.ReadBoolean());
			this.DisableIEditableObject = context.Reader.ReadBoolean();

			type.GetFieldInHierarchy("_neverCommitted")!.SetValue(this, context.Reader.ReadBoolean());
			type.GetFieldInHierarchy("_editLevelAdded")!.SetValue(this, context.Reader.ReadInt32());
			type.GetFieldInHierarchy("_identity")!.SetValue(this, context.Reader.ReadInt32());
			""");

		indentWriter.Indent--;
		indentWriter.WriteLine("}");
	}

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
			return "new TimeSpan(context.Reader.ReadInt64())";
		}
		if (valueType.FullyQualifiedName == "global::System.DateTimeOffset")
		{
			return "new DateTimeOffset(context.Reader.ReadInt64(), new TimeSpan(context.Reader.ReadInt64()))";
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
			SpecialType.System_DateTime => "new DateTime(context.Reader.ReadInt64())",
			_ => throw new NotImplementedException($"The given type, {valueType.Name}, cannot be deserialized.")
		};
	}

	private static void BuildReadOperation(IndentedTextWriter indentWriter, SerializationItemModel item)
	{
		indentWriter.WriteLine($"// {item.PropertyInfoContainingType.FullyQualifiedName}.{item.PropertyInfoFieldName}");
		var propertyType = item.PropertyInfoDataType;

		if (propertyType.TypeKind == TypeKind.Enum)
		{
			EnumBuilder.BuildReader(indentWriter, item);
		}
		else if (propertyType.Array is not null &&
			(propertyType.Array!.ElementType.SpecialType == SpecialType.System_Byte || propertyType.Array.ElementType.SpecialType == SpecialType.System_Char))
		{
			ArrayBuilder.BuildReader(indentWriter, item);
		}
		else if (propertyType.FullyQualifiedName == "global::System.Security.Claims.ClaimsPrincipal")
		{
			ClaimsPrincipalBuilder.BuildReader(indentWriter, item);
		}
		else if (propertyType.BusinessObjectKind != StereotypeKind.None)
		{
			StereotypeBuilder.BuildReader(indentWriter, item);
		}
		else if (propertyType.FullyQualifiedName == "global::System.Collections.Generic.List<int>")
		{
			ListOfIntBuilder.BuildReader(indentWriter, item);
		}
		else if (propertyType.IsNullable && propertyType.IsValueType)
		{
			NullableValueTypeBuilder.BuildReader(indentWriter, item);
		}
		else if (propertyType.SpecialType == SpecialType.System_String)
		{
			StringBuilder.BuildReader(indentWriter, item);
		}
		else if (propertyType.IsValueType)
		{
			ValueTypeBuilder.BuildReader(indentWriter, item);
		}
	}

	private static void BuildWriter(IndentedTextWriter indentWriter, SerializationModel model)
	{
		indentWriter.WriteLines(
			"""
			void global::CslaGeneratorSerialization.IGeneratorSerializable.SetState(global::CslaGeneratorSerialization.GeneratorFormatterWriterContext context)
			{
			""");
		indentWriter.Indent++;

		var itemId = 0;

		foreach (var item in model.Items)
		{
			BusinessBaseBuilder.BuildWriteOperation(indentWriter, item, itemId++);
			indentWriter.WriteLine();
		}

		indentWriter.WriteLines(
			"""
			context.Writer.Write(this.IsNew);
			context.Writer.Write(this.IsDeleted);
			context.Writer.Write(this.IsDirty);
			context.Writer.Write(this.IsChild);
			context.Writer.Write(this.DisableIEditableObject);

			var type = this.GetType();
			context.Writer.Write((bool)type.GetFieldInHierarchy("_neverCommitted")!.GetValue(this)!);
			context.Writer.Write((int)type.GetFieldInHierarchy("_editLevelAdded")!.GetValue(this)!);
			context.Writer.Write((int)type.GetFieldInHierarchy("_identity")!.GetValue(this)!);
			""");

		indentWriter.Indent--;
		indentWriter.WriteLine("}");
	}

	private static void BuildWriteOperation(IndentedTextWriter indentWriter, SerializationItemModel item, int itemId)
	{
		// Note that all of the "Write" invocations should either be handled
		// natively by BinaryWriter or by an extension method I've created.
		var managedBackingField = $"{item.PropertyInfoContainingType.FullyQualifiedName}.{item.PropertyInfoFieldName}";
		var valueVariable = $"value{itemId}";
		var propertyType = item.PropertyInfoDataType;

		indentWriter.WriteLine($"// {managedBackingField}");

		if (propertyType.TypeKind == TypeKind.Enum)
		{
			EnumBuilder.BuildWriter(indentWriter, propertyType, managedBackingField);
		}
		else if (propertyType.Array is not null &&
			(propertyType.Array!.ElementType.SpecialType == SpecialType.System_Byte || propertyType.Array.ElementType.SpecialType == SpecialType.System_Char))
		{
			ArrayBuilder.BuildWriter(indentWriter, propertyType, managedBackingField, valueVariable);
		}
		else if (propertyType.FullyQualifiedName == "global::System.Security.Claims.ClaimsPrincipal")
		{
			ClaimsPrincipalBuilder.BuildWriter(indentWriter, propertyType, managedBackingField, valueVariable);
		}
		else if (propertyType.BusinessObjectKind != StereotypeKind.None)
		{
			StereotypeBuilder.BuildWriter(indentWriter, propertyType, managedBackingField, valueVariable);
		}
		else if (propertyType.FullyQualifiedName == "global::System.Collections.Generic.List<int>")
		{
			ListOfIntBuilder.BuildWriter(indentWriter, propertyType, managedBackingField, valueVariable);
		}
		else if (propertyType.IsNullable && propertyType.IsValueType)
		{
			NullableValueTypeBuilder.BuildWriter(indentWriter, propertyType, managedBackingField, valueVariable);
		}
		else if (propertyType.SpecialType == SpecialType.System_String)
		{
			StringBuilder.BuildWriter(indentWriter, propertyType, managedBackingField);
		}
		else if (propertyType.IsValueType)
		{
			ValueTypeBuilder.BuildWriter(indentWriter, propertyType, managedBackingField);
		}
	}
}