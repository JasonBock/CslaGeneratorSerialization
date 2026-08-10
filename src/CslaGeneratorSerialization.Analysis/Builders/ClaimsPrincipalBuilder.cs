using CslaGeneratorSerialization.Analysis.Extensions;
using CslaGeneratorSerialization.Analysis.Models;
using System.CodeDom.Compiler;

namespace CslaGeneratorSerialization.Analysis.Builders;

internal static class ClaimsPrincipalBuilder
{
	internal static void BuildPropertyReader(IndentedTextWriter indentWriter, SerializationItemModel item) =>
		indentWriter.WriteLines(
			$$"""
			switch (context.Reader.ReadStateValue())
			{
				case global::CslaGeneratorSerialization.SerializationState.Duplicate:
					{{BuilderHelpers.GetLoadProperty(item, "context.GetReference(context.Reader.ReadInt32())")}}
					break;
				case global::CslaGeneratorSerialization.SerializationState.Value:
					var buffer = context.Reader.ReadByteArray();
				
					using (var stream = new global::System.IO.MemoryStream(buffer))
					{
						using (var reader = new global::System.IO.BinaryReader(stream))
						{
							var principal = new global::System.Security.Claims.ClaimsPrincipal(reader);
							{{BuilderHelpers.GetLoadProperty(item, "principal")}}
							context.AddReference(principal);
						}
					}
					break;
				case global::CslaGeneratorSerialization.SerializationState.Null:
					break;
			}
			""");

	internal static void BuildPropertyWriter(IndentedTextWriter indentWriter, TypeReferenceModel propertyType, string valueVariable) =>
		indentWriter.WriteLines(
			$$"""
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

					using (var {{valueVariable}}stream = new global::System.IO.MemoryStream())
					{
						using (var {{valueVariable}}writer = new global::System.IO.BinaryWriter({{valueVariable}}stream))
						{
							{{valueVariable}}.WriteTo({{valueVariable}}writer);
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

	internal static void BuildUnionReader(IndentedTextWriter indentWriter, TypeReferenceModel unionType) =>
		indentWriter.WriteLines(
			$$"""
			switch (context.Reader.ReadStateValue())
			{
				case global::CslaGeneratorSerialization.SerializationState.Duplicate:
					return ({{unionType.FullyQualifiedName}})(global::System.Security.Claims.ClaimsPrincipal)context.GetReference(context.Reader.ReadInt32())!;
				case global::CslaGeneratorSerialization.SerializationState.Value:
					var buffer = context.Reader.ReadByteArray();
				
					using (var stream = new global::System.IO.MemoryStream(buffer))
					{
						using (var reader = new global::System.IO.BinaryReader(stream))
						{
							var principal = new global::System.Security.Claims.ClaimsPrincipal(reader);
							context.AddReference(principal);
							return ({{unionType.FullyQualifiedName}})principal;
						}
					}
			}
			break;
			""");

	internal static void BuildUnionWriter(IndentedTextWriter indentWriter, TypeReferenceModel propertyType, string valueVariable) =>
		indentWriter.WriteLines(
			$$"""
			(var isReferenceDuplicate, var referenceId) = context.GetReference({{valueVariable}});
				
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
						{{valueVariable}}.WriteTo({{valueVariable}}writer);
						var {{valueVariable}}buffer = {{valueVariable}}stream.ToArray();
						context.Writer.Write(({{valueVariable}}buffer.Length, {{valueVariable}}buffer));
					}
				}
			}
			""");
}