using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace CslaGeneratorSerialization.Analysis.Tests.Generators;

internal static class ErrorTests
{
	[Test]
	public static async Task CreateWhenBusinessObjectHasDiagnosticsAsync()
	{
		// Note that there's no colon before the base type definition,
		// so that should trip compilation errors. The errors may change
		// from one version to another, so the DiagnosticResult values
		// may need to be updated to let the test pass.
		var code =
			"""
			using Csla;
			using CslaGeneratorSerialization;
			using System;

			namespace Domains;

			[GeneratorSerializable]
			public sealed partial class Data
				BusinessBase<Data>
			{
			}
			""";

		await TestAssistants.RunGeneratorAsync<GeneratorSerializationGenerator>(code,
			[],
			[
				new DiagnosticResult("CS1513", DiagnosticSeverity.Error).WithSpan(8, 33, 8, 33),
				new DiagnosticResult("CS1514", DiagnosticSeverity.Error).WithSpan(8, 33, 8, 33),
				new DiagnosticResult("CS1001", DiagnosticSeverity.Error).WithSpan(9, 20, 9, 20),
				new DiagnosticResult("CS0311", DiagnosticSeverity.Error).WithSpan(10, 1, 10, 1),
				new DiagnosticResult("CS0548", DiagnosticSeverity.Error).WithSpan(10, 1, 10, 1),
			]);
	}
}