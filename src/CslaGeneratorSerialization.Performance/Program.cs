using BenchmarkDotNet.Running;
using CslaGeneratorSerialization.Performance;
using System.Globalization;
using System.Numerics;

var value = new BigInteger(4444);
value.ToByteArray();

/*
new Int128()
var value = UInt128.Parse("-3333", CultureInfo.CurrentCulture);// new UInt128(222, 333);
Console.WriteLine(value);

value.

using (var ms = new MemoryStream())
using (var writer = new BinaryWriter(ms))
using (var reader = new BinaryReader(ms))
{
	// WriteInt128 and WriteUInt128
	Span<byte> buffer = stackalloc byte[16];
	BitConverter.TryWriteBytes(buffer[..8], (long)(value & long.MaxValue)); // Low 64 bits
	BitConverter.TryWriteBytes(buffer[8..], (long)(value >> 64));           // High 64 bits
	writer.Write(buffer);

	// ReadInt128
	ms.Position = 0;
	var data = reader.ReadBytes(16);
	var low = BitConverter.ToUInt64(data, 0);
	var high = BitConverter.ToUInt64(data, 8);
	var newValue = ((UInt128)high << 64) | (ulong)low;
	Console.WriteLine(newValue);
}
*/

//BenchmarkRunner.Run<BusinessListBaseSerializationWithDuplicates>();