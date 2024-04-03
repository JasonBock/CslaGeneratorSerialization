using CslaGeneratorSerialization.Extensions;
using CslaGeneratorSerialization.Models;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Builders;

internal static class StereotypeBuilder
{
	internal static void BuildReader(IndentedTextWriter indentWriter, SerializationItemModel item)
	{
		var propertyType = item.PropertyInfoDataType;

		indentWriter.WriteLines(
			$$"""
			switch (context.Reader.ReadStateValue())
			{
				case global::CslaGeneratorSerialization.SerializationState.Duplicate:
					{{BuilderHelpers.GetLoadProperty(item, "context.GetReference(context.Reader.ReadInt32())")}}
					break;
				case global::CslaGeneratorSerialization.SerializationState.Value:
			""");

		if (!propertyType.IsSealed)
		{
			indentWriter.WriteLines(
				$$"""
						{{propertyType.FullyQualifiedNameNoNullableAnnotation}} newValue;
								
						if (context.Reader.ReadStateValue() == global::CslaGeneratorSerialization.SerializationState.Duplicate)
						{
							newValue = context.CreateInstance<{{propertyType.FullyQualifiedNameNoNullableAnnotation}}>(context.GetTypeName(context.Reader.ReadInt32()));
						}
						else
						{
							var newValueTypeName = context.Reader.ReadString();
							context.AddTypeName(newValueTypeName);
							newValue = context.CreateInstance<{{propertyType.FullyQualifiedNameNoNullableAnnotation}}>(newValueTypeName);
						}
				""");
		}
		else
		{
			indentWriter.WriteLine($"		var newValue = context.CreateInstance<{propertyType.FullyQualifiedNameNoNullableAnnotation}>();");
		}

		indentWriter.WriteLines(
			$$"""
					((global::CslaGeneratorSerialization.IGeneratorSerializable)newValue).GetState(context);
					{{BuilderHelpers.GetLoadProperty(item, "newValue")}}
					context.AddReference(newValue);
					break;
				case global::CslaGeneratorSerialization.SerializationState.Null:
					break;
			}
			""");
	}

	internal static void BuildWriter(IndentedTextWriter indentWriter, TypeReferenceModel propertyType, string managedBackingField, string valueVariable) 
	{
		indentWriter.WriteLines(
			$$"""
			var {{valueVariable}} = this.ReadProperty<{{propertyType.FullyQualifiedName}}>({{managedBackingField}});

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
}