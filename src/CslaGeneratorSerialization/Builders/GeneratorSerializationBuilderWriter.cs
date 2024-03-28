using CslaGeneratorSerialization.Extensions;
using CslaGeneratorSerialization.Models;
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
			""");
		indentWriter.Indent++;

		indentWriter.WriteLine("// Set custom object state");

		foreach (var item in model.Items)
		{
			GeneratorSerializationBuilderWriter.BuildWriteOperation(indentWriter, item);
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

	private static void BuildWriteOperation(IndentedTextWriter indentWriter, SerializationItemModel item)
	{
		// Note that all of the "Write" invocations should either be handled
		// natively by BinaryWriter or by an extension method I've created.
		var propertyRead = $"{item.PropertyInfoContainingType.FullyQualifiedName}.{item.PropertyInfoFieldName}";

		if (item.PropertyInfoDataType.IsSerializable)
		{
			indentWriter.WriteLines(
				$$"""
				var value = this.ReadProperty({{propertyRead}});

				if (value is not null)
				{
					(var isDuplicate, var id) = context.GetReference(value);

					if (isDuplicate)
					{
						context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Duplicate);
						context.Writer.Write(id);
					}
					else
					{
						context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Value);
						((global::CslaGeneratorSerialization.IGeneratorSerializable)value).SetState(context);
					}
				}
				else
				{
					context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Null);
				}
				""");
		}
		else if (item.PropertyInfoDataType.IsNullable ||
			!item.PropertyInfoDataType.IsValueType)
		{
			var valueToWrite = item.PropertyInfoDataType.IsValueType ?
				"value.Value" : "value";

			indentWriter.WriteLines(
				$$"""
				var value = this.ReadProperty({{propertyRead}});

				if (value is not null)
				{
					context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Value);
					context.Writer.Write({{valueToWrite}});
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