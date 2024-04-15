# BusinessBaseSerialization

| Method                 | Mean     | Error     | StdDev    | Ratio | Gen0   | Gen1   | Allocated | Alloc Ratio |
|----------------------- |---------:|----------:|----------:|------:|-------:|-------:|----------:|------------:|
| RoundtripWithGenerator | 2.818 us | 0.0220 us | 0.0206 us |  0.38 | 0.2594 |      - |   4.43 KB |        0.18 |
| RoundtripWithMobile    | 7.501 us | 0.0673 us | 0.0596 us |  1.00 | 1.4038 | 0.0305 |  24.01 KB |        1.00 |

# BusinessListBaseSerialization

| Method                 | Mean     | Error   | StdDev  | Ratio | Gen0    | Gen1    | Allocated  | Alloc Ratio |
|----------------------- |---------:|--------:|--------:|------:|--------:|--------:|-----------:|------------:|
| RoundtripWithGenerator | 149.8 us | 1.41 us | 1.32 us |  0.25 | 14.6484 |  2.9297 |  248.17 KB |        0.16 |
| RoundtripWithMobile    | 606.8 us | 4.91 us | 4.10 us |  1.00 | 89.8438 | 39.0625 | 1530.66 KB |        1.00 |