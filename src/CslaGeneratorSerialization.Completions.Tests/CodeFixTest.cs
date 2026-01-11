using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace CslaGeneratorSerialization.Completions.Tests;

internal sealed class CodeFixTest<TAnalyzer, TCodeFix> 
	: CSharpCodeFixTest<TAnalyzer, TCodeFix, DefaultVerifier>
	where TAnalyzer : DiagnosticAnalyzer, new()
	where TCodeFix : CodeFixProvider, new()
{ }