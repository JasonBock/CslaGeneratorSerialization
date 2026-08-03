var pet = new Pet(new Dog("Rex"));

#pragma warning disable IDE0010 // Add missing cases
#pragma warning disable IDE0072

switch (pet)
{
	case global::Dog u1:
		Console.WriteLine($"Dog: {u1.Name}");
		break;
	case global::Cat u2:
		Console.WriteLine($"Cat: {u2.Name}");
		break;
	case global::BirdUnion u3:
		UnionHandlers.Handle(u3);
		break;
}

pet = new Cat("Tank");

switch (pet)
{
	case global::Dog u1:
		Console.WriteLine($"Dog: {u1.Name}");
		break;
	case global::Cat u2:
		Console.WriteLine($"Cat: {u2.Name}");
		break;
	case global::BirdUnion u3:
		UnionHandlers.Handle(u3);
		break;
}

var a = new A(new A1());

switch (a)
{
	case A1 u1:
		Console.WriteLine($"A1: {u1}");
		break;
	case A2 u2:
		Console.WriteLine($"A2: {u2}");
		break;
	case B u3:
		UnionHandlers.Handle(u3);
		break;
}

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

public union Simple(Cat);

public record class A1;
public record class A2;
public record class B1;
public record class B2;

public union B(B1, B2, A);
public union A(A1, A2, B);

public record class R1(R2 Value);
public record class R2(R1 Value);

public static class Stuff
{
	public static void Process(string value) => Console.WriteLine(value);
	public static void Process(int value) => Console.WriteLine(value);
}

public static class UnionHandlers
{
	public static void Handle(global::BirdUnion birdUnion)
	{
		switch (birdUnion)
		{
			case global::Parakeet u1:
				Console.WriteLine($"Parakeet: {u1.Name}");
				break;
			case global::Hummingbird u2:
				Console.WriteLine($"Hummingbird: {u2.Name}");
				break;
			case global::Robin u3:
				Console.WriteLine($"Robin: {u3.Name}");
				break;
		}
	}

	public static void Handle(A a)
	{
		switch (a)
		{
			case A1 u1:
				Console.WriteLine($"A1: {u1}");
				break;
			case A2 u2:
				Console.WriteLine($"A2: {u2}");
				break;
			case B u3:
				UnionHandlers.Handle(u3);
				break;
		}
	}

	public static void Handle(B b)
	{
		switch (b)
		{
			case B1 u1:
				Console.WriteLine($"B1: {u1}");
				break;
			case B2 u2:
				Console.WriteLine($"B1: {u2}");
				break;
			case A u3:
				Handle(u3);
				break;
		}
	}
}

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