# BusinessBaseSerialization

| Method                 | Mean     | Error     | StdDev    | Ratio | Gen0   | Gen1   | Allocated | Alloc Ratio |
|----------------------- |---------:|----------:|----------:|------:|-------:|-------:|----------:|------------:|
| RoundtripWithGenerator | 1.627 us | 0.0167 us | 0.0156 us |  0.25 | 0.2289 |      - |   3.87 KB |        0.18 |
| RoundtripWithMobile    | 6.413 us | 0.0281 us | 0.0263 us |  1.00 | 1.2817 | 0.0305 |  22.01 KB |        1.00 |


# BusinessListBaseSerialization

| Method                 | Mean      | Error    | StdDev   | Ratio | Gen0    | Gen1    | Allocated  | Alloc Ratio |
|----------------------- |----------:|---------:|---------:|------:|--------:|--------:|-----------:|------------:|
| RoundtripWithGenerator |  65.63 us | 0.592 us | 0.554 us |  0.13 | 14.1602 |  2.4414 |  240.04 KB |        0.17 |
| RoundtripWithMobile    | 488.46 us | 3.681 us | 3.443 us |  1.00 | 80.0781 | 37.1094 | 1373.19 KB |        1.00 |

# BusinessListBaseSerializationWithDuplicates

| Method                 | Mean      | Error    | StdDev   | Ratio | RatioSD | Gen0    | Gen1    | Allocated  | Alloc Ratio |
|----------------------- |----------:|---------:|---------:|------:|--------:|--------:|--------:|-----------:|------------:|
| RoundtripWithGenerator |  65.30 us | 0.792 us | 0.740 us |  0.13 |    0.00 | 13.6719 |  2.9297 |  235.58 KB |        0.18 |
| RoundtripWithMobile    | 494.16 us | 6.450 us | 6.033 us |  1.00 |    0.02 | 78.1250 | 33.2031 | 1328.93 KB |        1.00 |