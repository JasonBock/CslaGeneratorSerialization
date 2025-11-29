# BusinessBaseSerialization

| Method                 | Mean     | Error     | StdDev    | Ratio | Gen0   | Allocated | Alloc Ratio |
|----------------------- |---------:|----------:|----------:|------:|-------:|----------:|------------:|
| RoundtripWithGenerator | 5.135 us | 0.0592 us | 0.0524 us |  0.75 | 0.7629 |  13.33 KB |        0.60 |
| RoundtripWithMobile    | 6.841 us | 0.0709 us | 0.0663 us |  1.00 | 1.2817 |  22.37 KB |        1.00 |

# BusinessListBaseSerialization

| Method                 | Mean     | Error   | StdDev  | Ratio | Gen0    | Gen1    | Gen2    | Allocated | Alloc Ratio |
|----------------------- |---------:|--------:|--------:|------:|--------:|--------:|--------:|----------:|------------:|
| RoundtripWithGenerator | 461.0 us | 4.75 us | 3.97 us |  0.87 | 91.7969 | 29.2969 | 29.2969 |   1.25 MB |        0.91 |
| RoundtripWithMobile    | 530.9 us | 5.81 us | 5.44 us |  1.00 | 82.0313 | 41.0156 |       - |   1.38 MB |        1.00 |

# BusinessListBaseSerializationWithDuplicates

| Method                 | Mean     | Error   | StdDev  | Ratio | RatioSD | Gen0    | Gen1    | Gen2    | Allocated | Alloc Ratio |
|----------------------- |---------:|--------:|--------:|------:|--------:|--------:|--------:|--------:|----------:|------------:|
| RoundtripWithGenerator | 437.8 us | 4.95 us | 4.39 us |  0.84 |    0.01 | 91.7969 | 29.2969 | 29.2969 |   1.22 MB |        0.91 |
| RoundtripWithMobile    | 523.0 us | 7.82 us | 7.32 us |  1.00 |    0.02 | 80.0781 | 35.1563 |       - |   1.33 MB |        1.00 |