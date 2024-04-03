using CslaGeneratorSerialization.Extensions;
using CslaGeneratorSerialization.Models;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Builders;

internal static class ReadOnlyListBaseBuilder
{
	internal static void Build(IndentedTextWriter indentWriter, SerializationModel model)
	{
		indentWriter.Indent++;
		ReadOnlyListBaseBuilder.BuildWriter(indentWriter, model);
		indentWriter.WriteLine();
		ReadOnlyListBaseBuilder.BuildReader(indentWriter, model);
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

		indentWriter.WriteLines(
			$$"""
			var count = context.Reader.ReadInt32();

			using (this.LoadListMode)
			{
				for (var i = 0; i < count; i++)
				{
					switch (context.Reader.ReadStateValue())
					{
						case global::CslaGeneratorSerialization.SerializationState.Duplicate:
							this.Add(({{model.BusinessObject.BusinessObjectTarget!.FullyQualifiedName}})context.GetReference(context.Reader.ReadInt32()));
							break;
						case global::CslaGeneratorSerialization.SerializationState.Value:
			""");

		if (!model.BusinessObject.BusinessObjectTarget!.IsSealed)
		{
			indentWriter.WriteLines(
				$$"""
								{{model.BusinessObject.BusinessObjectTarget!.FullyQualifiedName}} newValue;
								
								if (context.Reader.ReadStateValue() == global::CslaGeneratorSerialization.SerializationState.Duplicate)
								{
									newValue = context.CreateInstance<{{model.BusinessObject.BusinessObjectTarget!.FullyQualifiedName}}>(context.GetTypeName(context.Reader.ReadInt32()));
								}
								else
								{
									var newValueTypeName = context.Reader.ReadString();
									context.AddTypeName(newValueTypeName);
									newValue = context.CreateInstance<{{model.BusinessObject.BusinessObjectTarget!.FullyQualifiedName}}>(newValueTypeName);
								}
				""");
		}
		else
		{
			indentWriter.WriteLine($"			var newValue = context.CreateInstance<{model.BusinessObject.BusinessObjectTarget!.FullyQualifiedName}>();");
		}
	
		indentWriter.WriteLines(
			$$"""
							((global::CslaGeneratorSerialization.IGeneratorSerializable)newValue).GetState(context);
							this.Add(newValue);
							break;
						case global::CslaGeneratorSerialization.SerializationState.Null:
							break;
					}
				}
			}

			this.IsReadOnly = context.Reader.ReadBoolean();
			""");

		indentWriter.Indent--;
		indentWriter.WriteLine("}");
	}

	// I assume that the items in the deleted list are in the list itself,
	// so they should be references.
	private static void BuildWriter(IndentedTextWriter indentWriter, SerializationModel model)
	{
		indentWriter.WriteLines(
			"""
			void global::CslaGeneratorSerialization.IGeneratorSerializable.SetState(global::CslaGeneratorSerialization.GeneratorFormatterWriterContext context)
			{
			""");
		indentWriter.Indent++;

		indentWriter.WriteLines(
			"""
			context.Writer.Write(this.Count);
			
			foreach (var item in this)
			{
				if (item is not null)
				{
					(var isReferenceDuplicate, var referenceId) = context.GetReference(item);
				
					if (isReferenceDuplicate)
					{
						context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Duplicate);
						context.Writer.Write(referenceId);
					}
					else
					{
						context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Value);
			""");

		if (!model.BusinessObject.BusinessObjectTarget!.IsSealed)
		{
			indentWriter.WriteLines(
				$$"""

							var itemTypeName = item.GetType().AssemblyQualifiedName!;
							(var isTypeNameDuplicate, var typeNameId) = context.GetTypeName(itemTypeName);
				
							if (isTypeNameDuplicate)
							{
								context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Duplicate);
								context.Writer.Write(typeNameId);
							}
							else
							{
								context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Value);
								context.Writer.Write(itemTypeName);
							}
				""");
		}

		indentWriter.WriteLines(
			"""
			
						((global::CslaGeneratorSerialization.IGeneratorSerializable)item).SetState(context);			
					}			
				}
				else
				{
					context.Writer.Write((byte)global::CslaGeneratorSerialization.SerializationState.Null);
				}
			}

			context.Writer.Write(this.IsReadOnly);
			""");

		indentWriter.Indent--;
		indentWriter.WriteLine("}");
	}
}