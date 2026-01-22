using BenchmarkDotNet.Running;
using CslaGeneratorSerialization.Performance;
using System.Globalization;

var value = Half.Parse("3.14", CultureInfo.CurrentCulture);
Console.WriteLine(value);

using (var ms = new MemoryStream())
using (var writer = new BinaryWriter(ms))
{
	writer.Write(value); // Writes as 2 bytes
}
//BenchmarkRunner.Run<BusinessListBaseSerializationWithDuplicates>();