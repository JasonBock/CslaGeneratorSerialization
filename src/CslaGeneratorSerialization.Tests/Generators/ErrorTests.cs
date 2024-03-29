using GeneratorSerialization.Tests;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace CslaGeneratorSerialization.Tests.Generators;

public static class ErrorTests
{
	[Test]
	public static async Task CreateWhenBusinessObjectHasDiagnosticsAsync()
	{
		var code =
			"""
			using Csla;
			using System;

			namespace Domains;

			[Serializable]
			public sealed partial class Data
				: BusinessBase<Data>
			{
				// Bad method declaration
				public void Proc
			}
			""";

		await TestAssistants.RunGeneratorAsync<GeneratorSerializationGenerator>(code,
			[],
			[
				new DiagnosticResult("CS0670", DiagnosticSeverity.Error).WithSpan(11, 9, 11, 13),
				new DiagnosticResult("CS1002", DiagnosticSeverity.Error).WithSpan(11, 18, 11, 18),
			]);
	}

	[Test]
	public static async Task CreateWhenBusinessObjectHasNoFieldsAsync()
	{
		var code =
			"""
			using Csla;
			using System;

			namespace Domains;
			
			[Serializable]
			public sealed partial class Data
				: BusinessBase<Data>
			{ }
			""";

		await TestAssistants.RunGeneratorAsync<GeneratorSerializationGenerator>(code,
			[],
			[]);
	}
}