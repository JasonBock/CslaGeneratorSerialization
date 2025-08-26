﻿using CslaGeneratorSerialization.Analysis.Extensions;
using CslaGeneratorSerialization.Analysis.Models;
using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

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

		if (model.IsCustomizable)
		{
			indentWriter.WriteLine("this.GetCustomState(context.Reader);");
			indentWriter.WriteLine();
		}

		indentWriter.WriteLines(
			"""
			global::CslaGeneratorSerialization.BusinessBaseAccessors.SetIsNewProperty(this, context.Reader.ReadBoolean());
			global::CslaGeneratorSerialization.BusinessBaseAccessors.SetIsDeletedProperty(this, context.Reader.ReadBoolean());
			global::CslaGeneratorSerialization.BusinessBaseAccessors.GetSetIsDirtyField(this) = context.Reader.ReadBoolean();
			global::CslaGeneratorSerialization.BusinessBaseAccessors.GetSetIsChildField(this) = context.Reader.ReadBoolean();
			this.DisableIEditableObject = context.Reader.ReadBoolean();
			
			global::CslaGeneratorSerialization.BusinessBaseAccessors.GetSetNeverCommittedField(this) = context.Reader.ReadBoolean();
			global::CslaGeneratorSerialization.BusinessBaseAccessors.GetSetEditLevelAddedField(this) = context.Reader.ReadInt32();
			global::CslaGeneratorSerialization.BusinessBaseAccessors.GetSetIdentityField(this) = context.Reader.ReadInt32();
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
		else if (propertyType.Array is not null)
		{
			ArrayBuilder.BuildReader(indentWriter, item);
		}
		else if (propertyType.FullyQualifiedName == Shared.ClaimsPrincipalFullyQualifiedName)
		{
			ClaimsPrincipalBuilder.BuildReader(indentWriter, item);
		}
		else if (propertyType.BusinessObjectKind != StereotypeKind.None)
		{
			StereotypeBuilder.BuildReader(indentWriter, item);
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
		else
		{
			CustomBuilder.BuildReader(indentWriter, item);
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

		if (model.IsCustomizable)
		{
			indentWriter.WriteLine("this.SetCustomState(context.Writer);");
			indentWriter.WriteLine();
		}

		indentWriter.WriteLines(
			$$"""
			context.Writer.Write(this.IsNew);
			context.Writer.Write(this.IsDeleted);
			context.Writer.Write(this.IsDirty);
			context.Writer.Write(this.IsChild);
			context.Writer.Write(this.DisableIEditableObject);
						
			context.Writer.Write(global::CslaGeneratorSerialization.BusinessBaseAccessors.GetSetNeverCommittedField(this));
			context.Writer.Write(global::CslaGeneratorSerialization.BusinessBaseAccessors.GetSetEditLevelAddedField(this));
			context.Writer.Write(global::CslaGeneratorSerialization.BusinessBaseAccessors.GetSetIdentityField(this));
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
		else if (propertyType.Array is not null)
		{
			ArrayBuilder.BuildWriter(indentWriter, propertyType, managedBackingField, valueVariable);
		}
		else if (propertyType.FullyQualifiedName == Shared.ClaimsPrincipalFullyQualifiedName)
		{
			ClaimsPrincipalBuilder.BuildWriter(indentWriter, propertyType, managedBackingField, valueVariable);
		}
		else if (propertyType.BusinessObjectKind != StereotypeKind.None)
		{
			StereotypeBuilder.BuildWriter(indentWriter, propertyType, managedBackingField);
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
		else
		{
			CustomBuilder.BuildWriter(indentWriter, propertyType, managedBackingField);
		}
	}
}