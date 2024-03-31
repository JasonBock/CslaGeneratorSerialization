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
			void global::CslaGeneratorSerialization.IGeneratorSerializable.GetState(global::CslaGeneratorSerialization.GeneratorFormatterReaderContext context)
			{
				// Get custom object state
			""");
		indentWriter.Indent++;

		foreach (var item in model.Items)
		{
			GeneratorSerializationBuilderReader.BuildReadOperation(indentWriter, item);
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

	private static void BuildReadOperation(IndentedTextWriter indentWriter, SerializationItemModel item)
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

					indentWriter.WriteLines(
						$$"""
						// {{item.PropertyInfoContainingType.FullyQualifiedName}}.{{item.PropertyInfoFieldName}}
						if (context.Reader.ReadStateValue() == global::CslaGeneratorSerialization.SerializationState.Value)
						{
							{{loadProperty}}
						}
						""");
				}
			}
		}
		else if (propertyType.TypeKind == TypeKind.Enum)
		{
			var loadProperty = GetLoadProperty(item,
				$"({propertyType.FullyQualifiedName}){GetValueTypeReadOperation(propertyType.EnumUnderlyingType!)}");
			indentWriter.WriteLines(
				$$"""
				// {{item.PropertyInfoContainingType.FullyQualifiedName}}.{{item.PropertyInfoFieldName}}
				{{loadProperty}}
				""");
		}
		else if (propertyType.FullyQualifiedName == "global::System.Security.Claims.ClaimsPrincipal")
		{
			indentWriter.WriteLines(
				$$"""
				// {{item.PropertyInfoContainingType.FullyQualifiedName}}.{{item.PropertyInfoFieldName}}
				switch (context.Reader.ReadStateValue())
				{
					case global::CslaGeneratorSerialization.SerializationState.Duplicate:
						{{GetLoadProperty(item, "context.GetReference(context.Reader.ReadInt32())")}}
						break;
					case global::CslaGeneratorSerialization.SerializationState.Value:
						var buffer = context.Reader.ReadByteArray();
				
						using (var stream = new global::System.IO.MemoryStream(buffer))
						{
							using (var reader = new global::System.IO.BinaryReader(stream))
							{
								var principal = new global::Csla.Security.CslaClaimsPrincipal(reader);
								{{GetLoadProperty(item, "principal")}}
								context.AddReference(principal);
							}
						}
						break;
					case global::CslaGeneratorSerialization.SerializationState.Null:
						break;
				}
				""");
		}
		else if (propertyType.IsSerializable)
		{
			indentWriter.WriteLines(
				$$"""
				// {{item.PropertyInfoContainingType.FullyQualifiedName}}.{{item.PropertyInfoFieldName}}
				switch (context.Reader.ReadStateValue())
				{
					case global::CslaGeneratorSerialization.SerializationState.Duplicate:
						{{GetLoadProperty(item, "context.GetReference(context.Reader.ReadInt32())")}}
						break;
					case global::CslaGeneratorSerialization.SerializationState.Value:
				""");

			if (!propertyType.IsSealed)
			{
				indentWriter.WriteLines(
					$$"""
							{{propertyType.FullName}} newValue;
								
							if (context.Reader.ReadStateValue() == global::CslaGeneratorSerialization.SerializationState.Duplicate)
							{
								newValue = context.CreateInstance<{{propertyType.FullName}}>(context.GetTypeName(context.Reader.ReadInt32()));
							}
							else
							{
								var newValueTypeName = context.Reader.ReadString();
								context.AddTypeName(newValueTypeName);
								newValue = context.CreateInstance<{{propertyType.FullName}}>(newValueTypeName);
							}
					""");
			}
			else
			{
				indentWriter.WriteLine($"		var newValue = context.CreateInstance<{propertyType.FullName}>();");
			}

			indentWriter.WriteLines(
				$$"""
						((global::CslaGeneratorSerialization.IGeneratorSerializable)newValue).GetState(context);
						{{GetLoadProperty(item, "newValue")}}
						context.AddReference(newValue);
						break;
					case global::CslaGeneratorSerialization.SerializationState.Null:
						break;
				}
				""");
		}
		else if (propertyType.FullyQualifiedName == "global::System.Collections.Generic.List<int>")
		{
			indentWriter.WriteLines(
				$$"""
				// {{item.PropertyInfoContainingType.FullyQualifiedName}}.{{item.PropertyInfoFieldName}}
				if (context.Reader.ReadStateValue() == global::CslaGeneratorSerialization.SerializationState.Value)
				{
					{{GetLoadProperty(item, "context.Reader.ReadListOfInt32()")}}
				}
				""");
		}
		else if (propertyType.IsValueType)
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

				indentWriter.WriteLines(
					$$"""
					// {{item.PropertyInfoContainingType.FullyQualifiedName}}.{{item.PropertyInfoFieldName}}
					if (context.Reader.ReadStateValue() == global::CslaGeneratorSerialization.SerializationState.Value)
					{
						{{loadProperty}}
					}
					""");
			}
			else
			{
				indentWriter.WriteLines(
					$$"""
					// {{item.PropertyInfoContainingType.FullyQualifiedName}}.{{item.PropertyInfoFieldName}}
					{{GetLoadProperty(item, GetValueTypeReadOperation(propertyType))}}
					""");
			}
		}
		else if (propertyType.SpecialType == SpecialType.System_String)
		{
			indentWriter.WriteLines(
				$$"""
				// {{item.PropertyInfoContainingType.FullyQualifiedName}}.{{item.PropertyInfoFieldName}}
				if (context.Reader.ReadStateValue() == global::CslaGeneratorSerialization.SerializationState.Value)
				{
					{{GetLoadProperty(item, "context.Reader.ReadString()")}}
				}
				""");
		}
	}
}