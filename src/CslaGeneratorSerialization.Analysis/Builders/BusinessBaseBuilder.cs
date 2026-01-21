using CslaGeneratorSerialization.Analysis.Extensions;
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

		for (var i = 0; i < model.Items.Length; i++)
		{
			var item = model.Items[i];
			BusinessBaseBuilder.BuildReadOperation(indentWriter, item);

			if (i < model.Items.Length - 1)
			{
				indentWriter.WriteLine();
			}
		}

		if (model.ImplementsMetastate)
		{
			indentWriter.WriteLines(
				"""
				
				((global::Csla.Serialization.Mobile.IMobileObjectMetastate)this).SetMetastate(context.Reader.ReadByteArray());
				""");
		}

		if (model.IsCustomizable)
		{
			indentWriter.WriteLines(
				"""
				
				this.GetCustomState(context.Reader);
				""");
		}

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
			SpecialType.System_DateTime => "new global::System.DateTime(context.Reader.ReadInt64())",
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

		for (var i = 0; i < model.Items.Length; i++)
		{
			var item = model.Items[i];
			BusinessBaseBuilder.BuildWriteOperation(indentWriter, item, itemId++);

			if (i < model.Items.Length - 1)
			{
				indentWriter.WriteLine();
			}
		}

		if (model.ImplementsMetastate)
		{
			indentWriter.WriteLines(
				"""

				var metastate = ((global::Csla.Serialization.Mobile.IMobileObjectMetastate)this).GetMetastate();
				context.Writer.Write((metastate.Length, metastate));
				""");
		}

		if (model.IsCustomizable)
		{
			indentWriter.WriteLines(
				"""
				
				this.SetCustomState(context.Writer);
				""");
		}

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
			StringBuilder.BuildWriter(indentWriter, propertyType, managedBackingField, valueVariable);
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