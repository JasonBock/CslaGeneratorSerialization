using Csla.Serialization.Mobile;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using NuGet.Frameworks;

namespace CslaGeneratorSerialization.Completions.Tests;

internal static class TestAssistants
{
	internal static async Task RunCodeFixAsync<TAnalyzer, TCodeFix>(string originalCode, string fixedCode, int codeActionIndex)
		where TAnalyzer : DiagnosticAnalyzer, new()
		where TCodeFix : CodeFixProvider, new()
	{
		var test = new CodeFixTest<TAnalyzer, TCodeFix>
		{
			ReferenceAssemblies = TestAssistants.net11ReferenceAssemblies.Value,
			TestCode = originalCode,
			FixedCode = fixedCode,
			CodeActionIndex = codeActionIndex,
		};

		test.TestState.AdditionalReferences.Add(typeof(TAnalyzer).Assembly);
		test.TestState.AdditionalReferences.Add(typeof(TCodeFix).Assembly);
		test.TestState.AdditionalReferences.Add(typeof(IMobileObject).Assembly);
		test.TestState.AdditionalReferences.Add(typeof(GeneratorSerializableAttribute).Assembly);

		await test.RunAsync();
	}

	private static readonly Lazy<ReferenceAssemblies> net11ReferenceAssemblies = new(() =>
	{
		// Always look here for the latest version of a particular runtime:
		// https://www.nuget.org/packages/Microsoft.NETCore.App.Ref
		if (!NuGetFramework.Parse("net11.0").IsPackageBased)
		{
			// The NuGet version provided at runtime does not recognize the 'net11.0' target framework
			throw new NotSupportedException("The 'net11.0' target framework is not supported by this version of NuGet.");
		}

		return new ReferenceAssemblies(
			 "net11.0",
			 new PackageIdentity(
				  "Microsoft.NETCore.App.Ref",
				  "11.0.0-preview.7.26381.103"),
			 Path.Combine("ref", "net11.0"));
	}, LazyThreadSafetyMode.ExecutionAndPublication);
}