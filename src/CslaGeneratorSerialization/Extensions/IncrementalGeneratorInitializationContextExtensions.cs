using Csla;
using Csla.Core;
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
					public extern static ref int GetSetEditLevelAddedField(global::Csla.Core.BusinessBase target);
				
					[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "_identity")]
					public extern static ref int GetSetIdentityField(global::Csla.Core.BusinessBase target);
				
					[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "_isChild")]
					public extern static ref bool GetSetIsChildField(global::Csla.Core.BusinessBase target);
				
					[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "_isDirty")]
					public extern static ref bool GetSetIsDirtyField(global::Csla.Core.BusinessBase target);
				
					[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "_neverCommitted")]
					public extern static ref bool GetSetNeverCommittedField(global::Csla.Core.BusinessBase target);

					[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Method, Name = "set_IsDeleted")]
					public extern static void SetIsDeletedProperty(global::Csla.Core.BusinessBase target, bool value);
				
					[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Method, Name = "set_IsNew")]
					public extern static void SetIsNewProperty(global::Csla.Core.BusinessBase target, bool value);
				}
				"""
			),
			(
				"CslaGeneratorSerialization.BusinessListBaseAccessor.g.cs",
				/* lang=c#-test */
				"""
				#nullable enable

				namespace CslaGeneratorSerialization;

				public static class BusinessListBaseAccessors<T, C>
					where T : global::Csla.BusinessListBase<T, C> 
					where C : global::Csla.Core.IEditableBusinessObject
				{
					[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "_identity")]
					public extern static ref int GetSetIdentityField(global::Csla.BusinessListBase<T, C> target);
				
					[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Field, Name = "_isChild")]
					public extern static ref bool GetSetIsChildField(global::Csla.BusinessListBase<T, C> target);
				
					[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.Method, Name = "set_EditLevel")]
					public extern static void SetEditLevelProperty(global::Csla.BusinessListBase<T, C> target, int value);
				}
				"""
			)
		];

   internal static void RegisterTypes(this IncrementalGeneratorInitializationContext self) => 
		self.RegisterPostInitializationOutput(static postInitializationContext =>
		{
			foreach (var (fileName, code) in IncrementalGeneratorInitializationContextExtensions.GetOutputCode())
			{
				postInitializationContext.AddSource(fileName, SourceText.From(code, Encoding.UTF8));
			}
		});
}