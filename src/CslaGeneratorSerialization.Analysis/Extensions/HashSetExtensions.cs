namespace CslaGeneratorSerialization.Analysis.Extensions;

internal static class HashSetExtensions
{
	extension<T>(HashSet<T> self)
	{
		internal void AddRange(IEnumerable<T> values)
		{
			foreach (var value in values)
			{
				_ = self.Add(value);
			}
		}
	}
}