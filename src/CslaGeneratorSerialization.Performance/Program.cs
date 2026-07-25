var pet = new Pet(new Dog("Rex"));

#pragma warning disable IDE0010 // Add missing cases
#pragma warning disable IDE0072

switch (pet)
{
	case Dog u1:
		Console.WriteLine($"Dog: {u1.Name}");
		break;
	case Cat u2:
		Console.WriteLine($"Cat: {u2.Name}");
		break;
	case BirdUnion u3:
		HandleBirdUnion(u3);
		break;
}

var a = new A(new A1());

var aValue = a switch
{
	A1 u1 => u1.ToString(),
	A2 u2 => u2.ToString(),
	B u3 => HandleB(u3).ToString()
};

static void HandleBirdUnion(BirdUnion birdUnion)
{
	switch (birdUnion)
	{
		case Parakeet u1:
			Console.WriteLine($"Parakeet: {u1.Name}");
			break;
		case Hummingbird u2:
			Console.WriteLine($"Hummingbird: {u2.Name}");
			break;
		case Robin u3:
			Console.WriteLine($"Robin: {u3.Name}");
			break;
	}
}

static A HandleA(A a) => a switch
{
	A1 u1 => u1,
	A2 u2 => u2,
	B u3 => HandleB(u3)
};

static B HandleB(B b) => b switch
{
	B1 u1 => u1,
	B2 u2 => u2,
	A u3 => HandleA(u3)
};

Console.WriteLine(aValue);

#pragma warning disable CA1050 // Declare types in namespaces
public record class Cat(string Name);
public record class Dog(string Name);
public record class Bird(string Name);

public record class Parakeet(string Name);
public record class Hummingbird(string Name);
public record class Robin(string Name);

#pragma warning disable CA1815 // Override equals and operator equals on value types
#pragma warning disable IDE0250 // Make struct 'readonly'

public union BirdUnion(Parakeet, Hummingbird, Robin);

public union Pet(Cat, Dog, BirdUnion);

public record class A1;
public record class A2;
public record class B1;
public record class B2;

public union B(B1, B2, A);
public union A(A1, A2, B);

/*
using System.Numerics;

var value = new BigInteger(4444);
value.ToByteArray();

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