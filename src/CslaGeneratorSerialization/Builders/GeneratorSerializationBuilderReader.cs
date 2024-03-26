using CslaGeneratorSerialization.Extensions;
using CslaGeneratorSerialization.Models;
using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Builders;

internal static class GeneratorSerializationBuilderReader
{
	internal static void Build(IndentedTextWriter indentWriter, SerializationModel model)
	{
		indentWriter.WriteLines(
			"""
			void global::CslaGeneratorSerialization.IGeneratorSerializable.GetState(global::System.IO.BinaryReader reader)
			{
				// Get custom object state
			""");
		indentWriter.Indent++;

		foreach (var item in model.Items)
		{
			indentWriter.WriteLines(GeneratorSerializationBuilderReader.BuildReadOperation(item));
		}

		indentWriter.WriteLines(
			"""

			// Get any children...
			
			//The only way I can get these (except for DisableIEditableObject) is through Reflection.
			//Ugly, but...means must.
			var type = this.GetType();
			type.GetField("_isNew")!.SetValue(this, reader.ReadBoolean());
			type.GetField("_isDeleted")!.SetValue(this, reader.ReadBoolean());
			type.GetField("_isDirty")!.SetValue(this, reader.ReadBoolean());
			type.GetField("_isChild")!.SetValue(this, reader.ReadBoolean());
			this.DisableIEditableObject = reader.ReadBoolean();

			type.GetField("_neverCommitted")!.SetValue(this, reader.ReadBoolean());
			type.GetField("_editLevelAdded")!.SetValue(this, reader.ReadInt32());
			type.GetField("_identity")!.SetValue(this, reader.ReadInt32());
			""");

		indentWriter.Indent--;
		indentWriter.WriteLine("}");
	}

	private static string BuildReadOperation(SerializationItemModel item)
	{
		static string GetValueTypeReadOperation(TypeReferenceModel valueType)
		{
			if (valueType.FullyQualifiedName == "global::System.Guid")
			{
				return "new global::System.Guid(reader.ReadBytes(16))";
			}
			if (valueType.FullyQualifiedName == "global::System.Decimal")
			{
				return "new decimal(new [] { reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32() })";
			}
			if (valueType.FullyQualifiedName == "global::System.TimeSpan")
			{
				return "new TimeSpan(reader.ReadInt64())";
			}
			if (valueType.FullyQualifiedName == "global::System.DateTimeOffset")
			{
				return "new DateTimeOffset(reader.ReadInt64(), new TimeSpan(reader.ReadInt64()))";
			}

			// Array

			return valueType.SpecialType switch
			{
				SpecialType.System_Boolean => "reader.ReadBoolean()",
				SpecialType.System_Char => "reader.ReadChar()",
				SpecialType.System_SByte => "reader.ReadSByte()",
				SpecialType.System_Byte => "reader.ReadByte()",
				SpecialType.System_Int16 => "reader.ReadInt16()",
				SpecialType.System_UInt16 => "reader.ReadUInt16()",
				SpecialType.System_Int32 => "reader.ReadInt32()",
				SpecialType.System_UInt32 => "reader.ReadUInt32()",
				SpecialType.System_Int64 => "reader.ReadInt64()",
				SpecialType.System_UInt64 => "reader.ReadUInt64()",
				SpecialType.System_Single => "reader.ReadSingle()",
				SpecialType.System_Double => "reader.ReadDouble()",
				SpecialType.System_DateTime => "new DateTime(reader.ReadInt64())",
				_ => throw new NotImplementedException($"The given type, {valueType.Name}, cannot be deserialized.")
			};
		}

		static string GetLoadProperty(SerializationItemModel item, string readerInvocation) =>
			$"this.LoadProperty({item.PropertyInfoContainingType.FullyQualifiedName}.{item.PropertyInfoFieldName}, {readerInvocation});";

		if (item.PropertyInfoDataType.IsValueType)
		{
			if (item.PropertyInfoDataType.IsNullable)
			{
				return
					$$"""
					if (reader.ReadStateValue() == global::CslaGeneratorSerialization.SerializationState.Value)
					{
						{{GetLoadProperty(item, GetValueTypeReadOperation(item.PropertyInfoDataType.TypeArguments[0]))}}
					}
					""";
			}
			else
			{
				return $"{GetLoadProperty(item, GetValueTypeReadOperation(item.PropertyInfoDataType))}";
			}
		}

		if (item.PropertyInfoDataType.SpecialType == SpecialType.System_String)
		{
			return
				$$"""
				if (reader.ReadStateValue() == global::CslaGeneratorSerialization.SerializationState.Value)
				{
					{{GetLoadProperty(item, "reader.ReadString()")}}
				}
				""";
		}

		throw new NotImplementedException($"The given type, {item.PropertyInfoDataType.Name}, cannot be deserialized.");
	}
}