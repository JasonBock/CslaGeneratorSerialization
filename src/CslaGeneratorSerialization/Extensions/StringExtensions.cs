﻿namespace CslaGeneratorSerialization.Extensions;

internal static class StringExtensions
{
	internal static string GenerateFileName(this string self) =>
		self.Replace("global::", string.Empty)
			.Replace(":", string.Empty)
			.Replace("<", string.Empty)
			.Replace(">", string.Empty)
			.Replace("?", "_null_");
}