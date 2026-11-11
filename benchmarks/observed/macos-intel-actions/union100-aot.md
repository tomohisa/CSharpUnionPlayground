C# Native: aot; abstract .NET 10: True.

| implementation | median batch s | encode s | decode s | switch s | aggregate s | JSON MiB | peak RSS MiB |
|---|---:|---:|---:|---:|---:|---:|---:|
| [csharp-closed-explicit](code/aot/json/csharp-closed-explicit/Program.cs) | 3.7901 | 0.5949 | 3.0300 | 0.2203 | 20.04 | 82.59 | 2150.3 |
| [csharp-closed-inferred](code/aot/json/csharp-closed-inferred/Program.cs) | 4.1178 | 0.7294 | 3.1543 | 0.2041 | 21.24 | 82.59 | 2152.3 |
| [csharp-abstract](code/aot/json/csharp-abstract/Program.cs) | 4.3562 | 0.6779 | 3.0571 | 0.2392 | 23.61 | 82.59 | 2607.0 |
| [csharp-union-generated](code/aot/json/csharp-union-generated/Program.cs) | 6.3086 | 0.8444 | 5.0874 | 0.1902 | 32.46 | 87.74 | 2182.1 |
| [csharp-union-custom](code/aot/json/csharp-union-custom/Program.cs) | 6.9179 | 0.6306 | 6.0613 | 0.1819 | 34.99 | 87.74 | 2085.2 |

| implementation | startup s | preparation s | exit s | total wall s |
|---|---:|---:|---:|---:|
| [csharp-closed-explicit](code/aot/json/csharp-closed-explicit/Program.cs) | 0.0603 | 1.468 | 0.1174 | 22.37 |
| [csharp-closed-inferred](code/aot/json/csharp-closed-inferred/Program.cs) | 0.0350 | 1.207 | 0.1377 | 23.37 |
| [csharp-abstract](code/aot/json/csharp-abstract/Program.cs) | 0.0336 | 0.928 | 0.0865 | 25.26 |
| [csharp-union-generated](code/aot/json/csharp-union-generated/Program.cs) | 0.0370 | 1.155 | 0.1243 | 34.49 |
| [csharp-union-custom](code/aot/json/csharp-union-custom/Program.cs) | 0.0301 | 1.172 | 0.1768 | 37.02 |
