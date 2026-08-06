using System.Runtime.CompilerServices;

var stringStuff = (Stuff)"hello";
Console.WriteLine(stringStuff.Value);

var intStuff = (Stuff)42;
Console.WriteLine(intStuff.Value);

var guidStuff = (Stuff)Guid.NewGuid();
Console.WriteLine(guidStuff.Value);

Console.WriteLine();

var stringAbstract = (AbstractOutcome)"abstract hello";
Console.WriteLine(stringAbstract.Value);

var intAbstract = (AbstractOutcome)69420;
Console.WriteLine(intAbstract.Value);

#pragma warning disable CA1050 // Declare types in namespaces
#pragma warning disable CA1815 // Override equals and operator equals on value types
#pragma warning disable IDE0250 // Make struct 'readonly'
public union Stuff(string, int, Guid);

[Union]
public sealed record class AbstractOutcome
	: AbstractOutcome.IUnionMembers
{
	private AbstractOutcome(object? value) => this.Value = value;

#pragma warning disable CA1034 // Nested types should not be visible
	public interface IUnionMembers
	{
		static abstract AbstractOutcome Create(string value);
		static abstract AbstractOutcome Create(int value);
		object? Value { get; }
	}

	public object? Value { get; }

	public static AbstractOutcome Create(string value) => new((object?)value);

	public static AbstractOutcome Create(int value) => new((object?)value);
}

/*
using System.Numerics;

var value = new BigInteger(4444);
value.ToByteArray();

new Int128()
var value = UInt128.Parse("-3333", CultureInfo.InvariantCulture);// new UInt128(222, 333);
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