using Csla;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

//BenchmarkRunner.Run<BusinessBaseSerialization>();

const int TypeCount = 4;
var unionCaseTypes = 
	string.Join(',', Enumerable.Range(0, TypeCount).Select(i => $"T{i}"));
var caseTypeDefinitions = 
	string.Join(' ', Enumerable.Range(0, TypeCount).Select(i => $$"""public class T{{i}}{}"""));
var source = $"public union T({unionCaseTypes}); {caseTypeDefinitions}";
var syntaxTree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Preview));

var compilation = CSharpCompilation.Create("generator", [syntaxTree],
	AppDomain.CurrentDomain.GetAssemblies()
		.Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
		.Select(_ => MetadataReference.CreateFromFile(_.Location))
	.Concat([MetadataReference.CreateFromFile(typeof(BusinessBase<>).Assembly.Location)]),
	new CSharpCompilationOptions(
		OutputKind.DynamicallyLinkedLibrary));

using var outputStream = new MemoryStream();
var result = compilation.Emit(outputStream);

Console.WriteLine($"Was emit successful? {result.Success}");
var errors = result.Diagnostics
	.Where(_ => _.Severity == DiagnosticSeverity.Error)
	.Select(_ => new
	{
		_.Id,
		Description = _.ToString(),
	})
	.OrderBy(_ => _.Id).ToArray();

var ignoredWarnings = Array.Empty<string>();
var warnings = result.Diagnostics
	.Where(_ => _.Severity == DiagnosticSeverity.Warning && !ignoredWarnings.Contains(_.Id))
	.Select(_ => new
	{
		_.Id,
		Description = _.ToString(),
	})
	.OrderBy(_ => _.Id).ToArray();

Console.WriteLine($"{errors.Length} error{(errors.Length != 1 ? "s" : string.Empty)}, {warnings.Length} warning{(warnings.Length != 1 ? "s" : string.Empty)}");
Console.WriteLine();

foreach (var error in errors)
{
	Console.WriteLine(
		$"Error - Id: {error.Id}, Description: {error.Description}");
}

foreach (var warning in warnings)
{
	Console.WriteLine(
		$"Warning - Id: {warning.Id}, Description: {warning.Description}");
}