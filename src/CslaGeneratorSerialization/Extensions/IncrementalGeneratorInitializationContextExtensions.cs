using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace CslaGeneratorSerialization.Extensions;

internal static class IncrementalGeneratorInitializationContextExtensions
{
   internal static ImmutableArray<(string fileName, string code)> GetOutputCode() => 
		[
		   (
				"CslaGeneratorSerialization.BusinessBaseAccessor.g.cs",
				/* lang=c#-test */
				"""
				#nullable enable

				namespace CslaGeneratorSerialization;

				public static class BusinessBaseAccessors
				{
					[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "_editLevelAdded")]
					public extern static ref int GetSetEditLevelAddedField(global::Csla.Core.BusinessBase businessBase);
				
					[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "_identity")]
					public extern static ref int GetSetIdentityField(global::Csla.Core.BusinessBase businessBase);
				
					[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "_isChild")]
					public extern static ref bool GetSetIsChildField(global::Csla.Core.BusinessBase businessBase);
				
					[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "_isDirty")]
					public extern static ref bool GetSetIsDirtyField(global::Csla.Core.BusinessBase businessBase);
				
					[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "_neverCommitted")]
					public extern static ref bool GetSetNeverCommittedField(global::Csla.Core.BusinessBase businessBase);

					[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Method, Name = "set_IsDeleted")]
					public extern static void SetIsDeletedProperty(global::Csla.Core.BusinessBase businessBase, bool value);
				
					[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Method, Name = "set_IsNew")]
					public extern static void SetIsNewProperty(global::Csla.Core.BusinessBase businessBase, bool value);
				}
				"""
			)
	   ];

   internal static void RegisterTypes(this IncrementalGeneratorInitializationContext self) => 
		self.RegisterPostInitializationOutput(static postInitializationContext =>
		{
			foreach(var (fileName, code) in IncrementalGeneratorInitializationContextExtensions.GetOutputCode())
			{
				postInitializationContext.AddSource(fileName, SourceText.From(code, Encoding.UTF8));
			}
		});
}
