using CslaGeneratorSerialization.Extensions;
using CslaGeneratorSerialization.Models;
using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Builders;

internal static class GeneratorSerializationBuilderWriter
{
	internal static void Build(IndentedTextWriter indentWriter, SerializationModel model)
	{
		indentWriter.WriteLines(
			"""
			void global::CslaGeneratorSerialization.IGeneratorSerializable.SetState(global::CslaGeneratorSerialization.GeneratorFormatterWriterContext context)
			{
				// Set custom object state
			""");
		indentWriter.Indent++;

		var itemId = 0;

		foreach (var item in model.Items)
		{
			GeneratorSerializationBuilderWriter.BuildWriteOperation(indentWriter, item, itemId++);
			indentWriter.WriteLine();
		}

		indentWriter.WriteLines(
			"""
			// Set base object state
			context.Writer.Write(this.IsNew);
			context.Writer.Write(this.IsDeleted);
			context.Writer.Write(this.IsDirty);
			context.Writer.Write(this.IsChild);
			context.Writer.Write(this.DisableIEditableObject);

			//The only way I can get these is through Reflection.
			//Ugly, but...means must.
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
		var propertyRead = $"{item.PropertyInfoContainingType.FullyQualifiedName}.{item.PropertyInfoFieldName}";
		var valueVariable = $"value{itemId}";

		indentWriter.WriteLine($"// {propertyRead}");

		var propertyType = item.PropertyInfoDataType;

		if (propertyType.TypeKind == TypeKind.Enum)
		{
			indentWriter.WriteLine($"context.Writer.Write(({propertyType.EnumUnderlyingType!.FullyQualifiedName})this.ReadProperty({propertyRead}));");
		}
		else if (propertyType.FullyQualifiedName == "global::System.Security.Claims.ClaimsPrincipal")
		{
			indentWriter.WriteLines(
				$$"""
				var {{valueVariable}} = this.ReadProperty({{propertyRead}});
				
				if ({{valueVariable}} is not null)
				{
					var {{valueVariable}}Principal = new global::Csla.Security.CslaClaimsPrincipal(this.ReadProperty({{propertyRead}}));
					(var isReferenceDuplicate, var referenceId) = context.GetReference({{valueVariable}}Principal);
				
					if (isReferenceDuplicate)
					{
						context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Duplicate);
						context.Writer.Write(referenceId);
					}
					else
					{
						context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Value);

						using (var {{valueVariable}}stream = new global::System.IO.MemoryStream())
						{
							using (var {{valueVariable}}writer = new global::System.IO.BinaryWriter({{valueVariable}}stream))
							{
								{{valueVariable}}Principal.WriteTo({{valueVariable}}writer);
								var {{valueVariable}}buffer = {{valueVariable}}stream.ToArray();
								context.Writer.Write(({{valueVariable}}buffer.Length, {{valueVariable}}buffer));
							}
						}
					}
				}
				else
				{
					context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Null);
				}
				""");
		}
		else if (propertyType.IsSerializable)
		{
			indentWriter.WriteLines(
				$$"""
				var {{valueVariable}} = this.ReadProperty({{propertyRead}});

				if ({{valueVariable}} is not null)
				{
					(var isReferenceDuplicate, var referenceId) = context.GetReference({{valueVariable}});

					if (isReferenceDuplicate)
					{
						context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Duplicate);
						context.Writer.Write(referenceId);
					}
					else
					{
						context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Value);				
				""");

			if (!propertyType.IsSealed)
			{
				indentWriter.WriteLines(
					$$"""

							var {{valueVariable}}TypeName = {{valueVariable}}.GetType().AssemblyQualifiedName!;
							(var isTypeNameDuplicate, var typeNameId) = context.GetTypeName({{valueVariable}}TypeName);

							if (isTypeNameDuplicate)
							{
								context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Duplicate);
								context.Writer.Write(typeNameId);
							}
							else
							{
								context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Value);
								context.Writer.Write({{valueVariable}}TypeName);
							}

					""");
			}

			indentWriter.WriteLines(
				$$"""
						((global::CslaGeneratorSerialization.IGeneratorSerializable){{valueVariable}}).SetState(context);
					}
				}
				else
				{
					context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Null);
				}
				""");
		}
		else if (propertyType.FullyQualifiedName == "global::System.Collections.Generic.List<int>")
		{
			indentWriter.WriteLines(
				$$"""
				var {{valueVariable}} = this.ReadProperty({{propertyRead}});

				if ({{valueVariable}} is not null)
				{
					context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Value);
					context.Writer.Write({{valueVariable}});
				}
				else
				{
					context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Null);
				}
				""");
		}
		else if (propertyType.IsNullable ||
			!propertyType.IsValueType)
		{
			var valueToWrite = propertyType.IsValueType ?
				$"{valueVariable}.Value" :
					propertyType.Array is not null &&
						(propertyType.Array.ElementType.SpecialType == SpecialType.System_Byte || propertyType.Array.ElementType.SpecialType == SpecialType.System_Char) &&
							propertyType.Array.Rank == 1 ?
							$"({valueVariable}.Length, {valueVariable})" :
					valueVariable;

			var enumCast = propertyType.IsNullable && propertyType.TypeArguments[0].TypeKind == TypeKind.Enum ?
				$"({propertyType.TypeArguments[0].EnumUnderlyingType!.FullyQualifiedName})" : string.Empty;

			indentWriter.WriteLines(
				$$"""
				var {{valueVariable}} = this.ReadProperty({{propertyRead}});

				if ({{valueVariable}} is not null)
				{
					context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Value);
					context.Writer.Write({{enumCast}}{{valueToWrite}});
				}
				else
				{
					context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Null);
				}
				""");
		}
		else
		{
			indentWriter.WriteLine($"context.Writer.Write(this.ReadProperty({propertyRead}));");
		}
	}
}