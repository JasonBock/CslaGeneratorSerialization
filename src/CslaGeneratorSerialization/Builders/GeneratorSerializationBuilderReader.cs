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
			indentWriter.WriteLine();
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

		var propertyType = item.PropertyInfoDataType;

		if (propertyType.Array is not null)
		{
			var elementSpecialType = propertyType.Array.ElementType.SpecialType;

			if (elementSpecialType == SpecialType.System_Byte || elementSpecialType == SpecialType.System_Char)
			{
				if (propertyType.Array.Rank < 3)
				{
					var arrayMethod = propertyType.Array.Rank == 1 ? "Array" : "ArrayArray";
					var readType = elementSpecialType == SpecialType.System_Byte ? "Byte" : "Char";
					var loadProperty = GetLoadProperty(item, $"context.Reader.Read{readType}{arrayMethod}()");

					return
						$$"""
						// {{item.PropertyInfoContainingType.FullyQualifiedName}}.{{item.PropertyInfoFieldName}}
						if (context.Reader.ReadStateValue() == global::CslaGeneratorSerialization.SerializationState.Value)
						{
							{{loadProperty}}
						}
						""";
				}
			}
		}

		if (propertyType.TypeKind == TypeKind.Enum)
		{
			var loadProperty = GetLoadProperty(item,
				$"({propertyType.FullyQualifiedName}){GetValueTypeReadOperation(propertyType.EnumUnderlyingType!)}");
			return
				$$"""
				// {{item.PropertyInfoContainingType.FullyQualifiedName}}.{{item.PropertyInfoFieldName}}
				{{loadProperty}}
				""";
		}

		if (propertyType.IsSerializable)
		{
			return
				$$"""
				// {{item.PropertyInfoContainingType.FullyQualifiedName}}.{{item.PropertyInfoFieldName}}
				switch (context.Reader.ReadStateValue())
				{
					case global::CslaGeneratorSerialization.SerializationState.Duplicate:
						{{GetLoadProperty(item, "context[context.Reader.ReadInt32()]")}}
						break;
					case global::CslaGeneratorSerialization.SerializationState.Value:
						var newValue = context.CreateInstance<{{propertyType.FullName}}>();
						((global::CslaGeneratorSerialization.IGeneratorSerializable)newValue).GetState(context);
						{{GetLoadProperty(item, "newValue")}}
						break;
					case global::CslaGeneratorSerialization.SerializationState.Null:
						break;
				}
				""";
		}

		if (propertyType.FullyQualifiedName == "global::System.Collections.Generic.List<int>")
		{
			return
				$$"""
				// {{item.PropertyInfoContainingType.FullyQualifiedName}}.{{item.PropertyInfoFieldName}}
				if (context.Reader.ReadStateValue() == global::CslaGeneratorSerialization.SerializationState.Value)
				{
					{{GetLoadProperty(item, "context.Reader.ReadListOfInt32()")}}
				}
				""";
		}

		if (propertyType.IsValueType)
		{
			if (propertyType.IsNullable)
			{
				var nullableType = propertyType.TypeArguments[0];
				var enumCast = string.Empty;

				if (nullableType.TypeKind == TypeKind.Enum) 
				{
					nullableType = nullableType.EnumUnderlyingType!;
					enumCast = $"({propertyType.TypeArguments[0].FullyQualifiedName})";
				}

				var loadProperty = GetLoadProperty(item, 
					$"{enumCast}{GetValueTypeReadOperation(nullableType)}");

				return
					$$"""
					// {{item.PropertyInfoContainingType.FullyQualifiedName}}.{{item.PropertyInfoFieldName}}
					if (context.Reader.ReadStateValue() == global::CslaGeneratorSerialization.SerializationState.Value)
					{
						{{loadProperty}}
					}
					""";
			}
			else
			{
				return
					$$"""
					// {{item.PropertyInfoContainingType.FullyQualifiedName}}.{{item.PropertyInfoFieldName}}
					{{GetLoadProperty(item, GetValueTypeReadOperation(propertyType))}}
					""";
			}
		}

		if (propertyType.SpecialType == SpecialType.System_String)
		{
			return
				$$"""
				// {{item.PropertyInfoContainingType.FullyQualifiedName}}.{{item.PropertyInfoFieldName}}
				if (context.Reader.ReadStateValue() == global::CslaGeneratorSerialization.SerializationState.Value)
				{
					{{GetLoadProperty(item, "context.Reader.ReadString()")}}
				}
				""";
		}

		throw new NotImplementedException($"The given type, {propertyType.Name}, cannot be deserialized.");
	}
}