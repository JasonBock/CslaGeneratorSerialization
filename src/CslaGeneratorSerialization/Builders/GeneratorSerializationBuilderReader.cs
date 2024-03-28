using CslaGeneratorSerialization.Extensions;
using CslaGeneratorSerialization.Models;
using Microsoft.CodeAnalysis;
using System;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Builders;

internal static class GeneratorSerializationBuilderReader
{
	internal static void Build(IndentedTextWriter indentWriter, SerializationModel model)
	{
		indentWriter.WriteLines(
			"""
			void global::CslaGeneratorSerialization.IGeneratorSerializable.GetState(global::CslaGeneratorSerialization.GeneratorFormatterReaderContext context)
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

			//The only way I can get these (except for DisableIEditableObject) is through Reflection.
			//Ugly, but...means must.
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

	private static string BuildReadOperation(SerializationItemModel item)
	{
		static string GetValueTypeReadOperation(TypeReferenceModel valueType)
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

		static string GetLoadProperty(SerializationItemModel item, string readerInvocation) =>
			$"this.LoadProperty({item.PropertyInfoContainingType.FullyQualifiedName}.{item.PropertyInfoFieldName}, {readerInvocation});";

		if (item.PropertyInfoDataType.Array is not null)
		{
			if (item.PropertyInfoDataType.Array.ElementType.SpecialType == SpecialType.System_Byte)
			{
				if (item.PropertyInfoDataType.Array.Rank < 3)
				{
					var arrayMethod = item.PropertyInfoDataType.Array.Rank == 1 ? "Array" : "ArrayArray";
					var loadProperty = GetLoadProperty(item, $"context.Reader.ReadByte{arrayMethod}()");

					if (item.PropertyInfoDataType.IsNullable)
					{
						return
							$$"""
							if (context.Reader.ReadStateValue() == global::CslaGeneratorSerialization.SerializationState.Value)
							{
								{{loadProperty}}
							}
							""";
					}
					else
					{
						return loadProperty;
					}
				}
			}
		}

		if (item.PropertyInfoDataType.IsSerializable)
		{
			return
				$$"""
				switch (context.Reader.ReadStateValue())
				{
					case global::CslaGeneratorSerialization.SerializationState.Duplicate:
						{{GetLoadProperty(item, "context[context.Reader.ReadInt32()]")}}
						break;
					case global::CslaGeneratorSerialization.SerializationState.Value:
						var newValue = context.CreateInstance<{{item.PropertyInfoDataType.FullName}}>();
						((global::CslaGeneratorSerialization.IGeneratorSerializable)newValue).GetState(context);
						{{GetLoadProperty(item, "newValue")}}
						break;
					case global::CslaGeneratorSerialization.SerializationState.Null:
						break;
				}
				""";
		}

		if (item.PropertyInfoDataType.IsValueType)
		{
			if (item.PropertyInfoDataType.IsNullable)
			{
				return
					$$"""
					if (context.Reader.ReadStateValue() == global::CslaGeneratorSerialization.SerializationState.Value)
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
				if (context.Reader.ReadStateValue() == global::CslaGeneratorSerialization.SerializationState.Value)
				{
					{{GetLoadProperty(item, "context.Reader.ReadString()")}}
				}
				""";
		}

		throw new NotImplementedException($"The given type, {item.PropertyInfoDataType.Name}, cannot be deserialized.");
	}
}