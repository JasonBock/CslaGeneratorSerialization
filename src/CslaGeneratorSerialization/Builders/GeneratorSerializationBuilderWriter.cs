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
			void global::CslaGeneratorSerialization.IGeneratorSerializable.SetState(global::System.IO.BinaryWriter writer)
			{
			""");
		indentWriter.Indent++;

		indentWriter.WriteLines(
			"""
			writer.Write(this.GetType().FullName!);

			// Set custom object state
			""");

		foreach (var item in model.Items)
		{
			GeneratorSerializationBuilderWriter.BuildWriteOperation(indentWriter, item);
		}

		indentWriter.WriteLines(
			"""

			// Set any children...
			
			// Set base object state
			writer.Write(this.IsNew);
			writer.Write(this.IsDeleted);
			writer.Write(this.IsDirty);
			writer.Write(this.IsChild);
			writer.Write(this.DisableIEditableObject);

			//The only way I can get these is through Reflection.
			//Ugly, but...means must.
			var type = this.GetType();
			writer.Write((bool)type.GetField("_neverCommitted")!.GetValue(this)!);
			writer.Write((int)type.GetField("_editLevelAdded")!.GetValue(this)!);
			writer.Write((int)type.GetField("_identity")!.GetValue(this)!);
			""");

		indentWriter.Indent--;
		indentWriter.WriteLine("}");
	}

	private static void BuildWriteOperation(IndentedTextWriter indentWriter, SerializationItemModel item)
	{
		// Note that all of the "Write" invocations should either be handled
		// natively by BinaryWriter or by an extension method I've created.
		var propertyRead = $"{item.PropertyInfoContainingType.FullyQualifiedName}.{item.PropertyInfoFieldName}";

		if (item.PropertyInfoDataType.IsNullable ||
			!item.PropertyInfoDataType.IsValueType)
		{
			indentWriter.WriteLines(
				$$"""
				var value = this.ReadProperty({{propertyRead}});

				if (value is not null)
				{
					writer.Write(value.Value);
				}
				else
				{
					writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Null);
				}
				""");
		}
		else
		{
			indentWriter.WriteLine($"writer.Write(this.ReadProperty({propertyRead}));");
		}
	}
}