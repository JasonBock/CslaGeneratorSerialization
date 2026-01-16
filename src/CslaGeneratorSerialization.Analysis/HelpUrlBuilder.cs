namespace CslaGeneratorSerialization.Analysis;

internal static class HelpUrlBuilder
{
	internal static string Build(string identifier, string title) =>
		$"https://github.com/JasonBock/CslaGeneratorSerialization/tree/main/docs/{identifier}-{title}.md";
}