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
			ReferenceAssemblies = TestAssistants.net10ReferenceAssemblies.Value,
			TestCode = originalCode,
			FixedCode = fixedCode,
			CodeActionIndex = codeActionIndex,
		};

		test.TestState.AdditionalReferences.Add(typeof(TAnalyzer).Assembly);
		test.TestState.AdditionalReferences.Add(typeof(TCodeFix).Assembly);
		test.TestState.AdditionalReferences.Add(typeof(IMobileObject).Assembly);

		await test.RunAsync();
	}

	private static readonly Lazy<ReferenceAssemblies> net10ReferenceAssemblies = new(() =>
	{
		// Always look here for the latest version of a particular runtime:
		// https://www.nuget.org/packages/Microsoft.NETCore.App.Ref
		if (!NuGetFramework.Parse("net10.0").IsPackageBased)
		{
			// The NuGet version provided at runtime does not recognize the 'net10.0' target framework
			throw new NotSupportedException("The 'net10.0' target framework is not supported by this version of NuGet.");
		}

		return new ReferenceAssemblies(
			 "net10.0",
			 new PackageIdentity(
				  "Microsoft.NETCore.App.Ref",
				  "10.0.0"),
			 Path.Combine("ref", "net10.0"));
	}, LazyThreadSafetyMode.ExecutionAndPublication);
}