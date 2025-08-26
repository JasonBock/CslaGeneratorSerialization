using CslaGeneratorSerialization.Analysis.Extensions;
using CslaGeneratorSerialization.Analysis.Models;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class BusinessListBaseBuilder
{
	internal static void Build(IndentedTextWriter indentWriter, SerializationModel model)
	{
		indentWriter.Indent++;
		BusinessListBaseBuilder.BuildWriter(indentWriter, model);
		indentWriter.WriteLine();
		BusinessListBaseBuilder.BuildReader(indentWriter, model);
		indentWriter.Indent--;
	}

	private static void BuildReader(IndentedTextWriter indentWriter, SerializationModel model)
	{
		indentWriter.WriteLines(
			$$"""
			void global::CslaGeneratorSerialization.IGeneratorSerializable.GetState(global::CslaGeneratorSerialization.GeneratorFormatterReaderContext context)
			{
				var count = context.Reader.ReadInt32();

				for (var i = 0; i < count; i++)
				{
					this.Add(context.Read<{{model.BusinessObject.BusinessObjectTarget!.FullyQualifiedNameNoNullableAnnotation}}>({{model.BusinessObject.BusinessObjectTarget!.IsSealed.ToString().ToLower()}})!);
				}

				var deletedCount = context.Reader.ReadInt32();

				for (var d = 0; d < deletedCount; d++)
				{
					this.DeletedList.Add(({{model.BusinessObject.BusinessObjectTarget!.FullyQualifiedNameNoNullableAnnotation}})context.GetReference(context.Reader.ReadInt32()));
				}

			""");

		if (model.IsCustomizable)
		{
			indentWriter.WriteLines(
				"""
					this.GetCustomState(context.Reader);
				
				""");
		}

		// TODO: Once this is fixed, I should uncomment the EditLevel call.
		// https://github.com/dotnet/runtime/issues/107132
		var accessorType = $"global::CslaGeneratorSerialization.BusinessListBaseAccessors<{model.BusinessObject.FullyQualifiedNameNoNullableAnnotation}, {model.BusinessObject.BusinessObjectTarget.FullyQualifiedNameNoNullableAnnotation}>";

		indentWriter.WriteLines(
			$$"""
				{{accessorType}}.GetSetIsChildField(this) = context.Reader.ReadBoolean();
				//{{accessorType}}.SetEditLevelProperty(this, context.Reader.ReadInt32());
				{{accessorType}}.GetSetIdentityField(this) = context.Reader.ReadInt32();
						
				this.AllowEdit = context.Reader.ReadBoolean();
				this.AllowNew = context.Reader.ReadBoolean();
				this.AllowRemove = context.Reader.ReadBoolean();
				this.RaiseListChangedEvents = context.Reader.ReadBoolean();
			}
			""");
	}

   // I assume that the items in the deleted list are in the list itself,
   // so they should be references.
   private static void BuildWriter(IndentedTextWriter indentWriter, SerializationModel model)
   {
		indentWriter.WriteLines(
			$$"""
			void global::CslaGeneratorSerialization.IGeneratorSerializable.SetState(global::CslaGeneratorSerialization.GeneratorFormatterWriterContext context)
			{
				context.Writer.Write(this.Count);
			
				foreach (var item in this)
				{
					context.Write(item, {{model.BusinessObject.BusinessObjectTarget!.IsSealed.ToString().ToLower()}});
				}

				context.Writer.Write(this.DeletedList.Count);

				foreach (var deletedItem in this.DeletedList)
				{
					(_, var deletedReferenceId) = context.GetReference(deletedItem);
					context.Writer.Write(deletedReferenceId);
				}

			""");

		if (model.IsCustomizable)
		{
			indentWriter.WriteLines(
				"""
					this.SetCustomState(context.Writer);
				
				""");
		}

		var accessorType = $"global::CslaGeneratorSerialization.BusinessListBaseAccessors<{model.BusinessObject.FullyQualifiedNameNoNullableAnnotation}, {model.BusinessObject.BusinessObjectTarget.FullyQualifiedNameNoNullableAnnotation}>";

		// TODO: Once this is fixed, I should uncomment the EditLevel call.
		// https://github.com/dotnet/runtime/issues/107132
		indentWriter.WriteLines(
			$$"""
				context.Writer.Write({{accessorType}}.GetSetIsChildField(this));
				//context.Writer.Write(this.EditLevel);
				context.Writer.Write({{accessorType}}.GetSetIdentityField(this));
			
				context.Writer.Write(this.AllowEdit);
				context.Writer.Write(this.AllowNew);
				context.Writer.Write(this.AllowRemove);
				context.Writer.Write(this.RaiseListChangedEvents);
			}
			""");
   }
}