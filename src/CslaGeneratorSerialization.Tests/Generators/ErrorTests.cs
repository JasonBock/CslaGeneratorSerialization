﻿using Microsoft.CodeAnalysis.Testing;
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
				new DiagnosticResult("CS0116", DiagnosticSeverity.Error).WithSpan(9, 19, 9, 20),
				new DiagnosticResult("CS1022", DiagnosticSeverity.Error).WithSpan(10, 1, 10, 2),
				new DiagnosticResult("CS1022", DiagnosticSeverity.Error).WithSpan(11, 1, 11, 2),
			]);
	}
}