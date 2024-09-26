For a `BusinessListBase`, we should be able to use `BusinessObjectTarget` on the `TypeReferenceModel` for the list type itself, and that gives us the `C` value.

Before:

`BusinessBaseSerialization`

| Method                 | Mean     | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|----------------------- |---------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| RoundtripWithGenerator | 2.508 us | 0.0201 us | 0.0188 us |  0.34 |    0.00 | 0.2441 |   4.12 KB |        0.18 |
| RoundtripWithMobile    | 7.467 us | 0.1045 us | 0.0978 us |  1.00 |    0.02 | 1.3428 |  23.47 KB |        1.00 |

`BusinessListBaseSerialization`

| Method                 | Mean     | Error   | StdDev  | Ratio | Gen0    | Gen1    | Allocated | Alloc Ratio |
|----------------------- |---------:|--------:|--------:|------:|--------:|--------:|----------:|------------:|
| RoundtripWithGenerator | 131.1 us | 0.91 us | 0.85 us |  0.21 | 13.6719 |  2.9297 |  242.3 KB |        0.16 |
| RoundtripWithMobile    | 609.9 us | 3.58 us | 3.35 us |  1.00 | 89.8438 | 39.0625 | 1519.6 KB |        1.00 |

After:

`BusinessBaseSerialization`

| Method                 | Mean     | Error     | StdDev    | Ratio | Gen0   | Gen1   | Allocated | Alloc Ratio |
|----------------------- |---------:|----------:|----------:|------:|-------:|-------:|----------:|------------:|
| RoundtripWithGenerator | 1.930 us | 0.0096 us | 0.0090 us |  0.26 | 0.2289 |      - |   3.88 KB |        0.17 |
| RoundtripWithMobile    | 7.407 us | 0.0737 us | 0.0653 us |  1.00 | 1.3733 | 0.0305 |  23.47 KB |        1.00 |

`BusinessListBaseSerialization`

| Method                 | Mean      | Error     | StdDev    | Ratio | RatioSD | Gen0    | Gen1    | Allocated  | Alloc Ratio |
|----------------------- |----------:|----------:|----------:|------:|--------:|--------:|--------:|-----------:|------------:|
| RoundtripWithGenerator |  76.29 us |  0.461 us |  0.431 us |  0.13 |    0.00 | 12.6953 |  2.4414 |  218.67 KB |        0.14 |
| RoundtripWithMobile    | 580.20 us | 11.186 us | 10.464 us |  1.00 |    0.02 | 89.8438 | 39.0625 | 1519.59 KB |        1.00 |

Note that this isn't **quite** fair because I can't handle `EditLevel` due to a bug with `UnsafeAccessorAttribute` in .NET 9 RC1. This should be fixed in RC2, but I don't think that'll add a ton of overhead back.

TODO:
* If this works, we can get rid the `GetXYZInHierarchy()` extension methods
* Add a test to a simple `BusinessBase<Customer>`