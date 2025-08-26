Restructing packages

Look for `global::CslaGeneratorSerialization` to see what is referenced by gen'd code.

* CslaGeneratorSerialization.Analysis - Takes SG stuff and put it in here.
* CslaGeneratorSerialization.Analysis.Tests - Tests analysis library
* CslaGeneratorSerialization.Analysis.IntegrationTests - Does integration tests on SG stuff
* CslaGeneratorSerialization - attribute and code gen'd from `IncrementalGeneratorInitializationContextExtensions`
* CslaGeneratorSerialization.Tests
* CslaGeneratorSerialization.Package
* CslaGeneratorSerialization.Performance - changes to reference `CslaGeneratorSerialization.Analysis`
