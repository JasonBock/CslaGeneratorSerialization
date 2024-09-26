﻿using Csla.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using NuGet.Frameworks;

namespace CslaGeneratorSerialization.Tests;

internal static class TestAssistants
{
	internal static async Task RunAnalyzerAsync<TAnalyzer>(string code,
		IEnumerable<DiagnosticResult> expectedDiagnostics,
		OutputKind outputKind = OutputKind.DynamicallyLinkedLibrary,
		IEnumerable<MetadataReference>? additionalReferences = null)
		where TAnalyzer : DiagnosticAnalyzer, new()
	{
		var test = new AnalyzerTest<TAnalyzer>()
		{
			//ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
			ReferenceAssemblies = TestAssistants.GetNet90(),
			TestState =
			{
				Sources = { code },
				OutputKind = outputKind,
			},
		};

		test.TestState.AdditionalReferences.Add(typeof(TAnalyzer).Assembly);

		if (additionalReferences is not null)
		{
			test.TestState.AdditionalReferences.AddRange(additionalReferences);
		}

		test.TestState.ExpectedDiagnostics.AddRange(expectedDiagnostics);
		await test.RunAsync();
	}

	internal static async Task RunGeneratorAsync<TGenerator>(string code,
		IEnumerable<(string, string)> generatedSources,
		IEnumerable<DiagnosticResult> expectedDiagnostics,
		OutputKind outputKind = OutputKind.DynamicallyLinkedLibrary,
		IEnumerable<MetadataReference>? additionalReferences = null,
		ReportDiagnostic generalDiagnosticOption = ReportDiagnostic.Default,
		List<string>? disabledDiagnostics = null)
		where TGenerator : IIncrementalGenerator, new()
	{
		var test = new IncrementalGeneratorTest<TGenerator>(generalDiagnosticOption)
		{
			//ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
			ReferenceAssemblies = TestAssistants.GetNet90(),
			TestState =
			{
				Sources = { code },
				OutputKind = outputKind,
			},
		};

		if (disabledDiagnostics is not null)
		{
			test.DisabledDiagnostics.AddRange(disabledDiagnostics);
		}

		foreach (var (generatedFileName, generatedCode) in generatedSources)
		{
			test.TestState.GeneratedSources.Add((typeof(GeneratorSerializationGenerator), generatedFileName, generatedCode));
		}

		test.TestState.AdditionalReferences.Add(typeof(TGenerator).Assembly);
		test.TestState.AdditionalReferences.Add(typeof(BusinessBase).Assembly);

		if (additionalReferences is not null)
		{
			test.TestState.AdditionalReferences.AddRange(additionalReferences);
		}

		test.TestState.ExpectedDiagnostics.AddRange(expectedDiagnostics);
		await test.RunAsync();
	}

	private static ReferenceAssemblies GetNet90()
	{
		// Always look here for the latest version of a particular runtime:
		// https://www.nuget.org/packages/Microsoft.NETCore.App.Ref
		if (!NuGetFramework.Parse("net9.0").IsPackageBased)
		{
			// The NuGet version provided at runtime does not recognize the 'net9.0' target framework
			throw new NotSupportedException("The 'net9.0' target framework is not supported by this version of NuGet.");
		}

		return new ReferenceAssemblies(
			 "net9.0",
			 new PackageIdentity(
				  "Microsoft.NETCore.App.Ref",
				  "9.0.0-rc.1.24431.7"),
			 Path.Combine("ref", "net9.0"));
	}
}