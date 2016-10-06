# IntelHexFormatReader #

A .NET library to parse [an Intel HEX file](https://en.wikipedia.org/wiki/Intel_HEX) (often used for representing code used to program microprocessors and microcontrollers) into a representative "memory representation".

## How to use the .NET library##

Link the following nuget package in your project in order to use the Intel HEX format reader: https://www.nuget.org/packages/IntelHexFormatReader/

Alternatively, install the package using the nuget package manager console:

```
Install-Package IntelHexFormatReader
```

The following minimal snippet demonstrates loading an Intel HEX file and printing out it's resulting memory representation:

```csharp
using System.Diagnostics;
using IntelHexFormatReader;

namespace IntelHexFormatReaderDemo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var reader = new HexFileReader(@"C:\\MyHexFiles\\myHexFile.hex", 32768);
            var memoryRepresentation = reader.Parse();
            foreach (var cell in memoryRepresentation.Cells)
                Debug.WriteLine(cell);
        }
    }
}
```
HexFileReader has a second constructor which allows you to pass any ```IEnumerable<string>``` directly.
