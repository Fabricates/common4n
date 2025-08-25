# Quick Start Guide

## For Linux/macOS Users

1. **Build the library:**
   ```bash
   ./build.sh
   ```

2. **For VB.NET projects:**
   ```bash
   # Copy the shared library
   cp libewma.so /path/to/your/project/
   
   # Copy the wrapper
   cp examples/EWMAExample.vb /path/to/your/project/
   
   # Update DllImport in the VB file:
   # Change "libewma.dll" to "libewma.so"
   ```

3. **For C# projects:**
   ```bash
   # Copy the shared library  
   cp libewma.so /path/to/your/project/
   
   # Copy the wrapper
   cp examples/EWMAExample.cs /path/to/your/project/
   
   # Update DllImport in the CS file:
   # Change "libewma.dll" to "libewma.so"
   ```

## For Windows Users

1. **Build the library:**
   ```cmd
   build.bat
   ```

2. **For VB.NET/.NET projects:**
   - Copy `libewma.dll` to your project directory
   - Copy `examples/EWMAExample.vb` or `examples/EWMAExample.cs`
   - The DllImport is already set correctly for Windows

## Basic Usage

### VB.NET
```vb
Using ewma As New EWMAWrapper(0.3)
    Dim result As Double = ewma.Update(10.0)
    Console.WriteLine(result)
End Using
```

### C#
```csharp
using (var ewma = new EWMAWrapper(0.3))
{
    double result = ewma.Update(10.0);
    Console.WriteLine(result);
}
```

## Common Issues

- **Linux**: If you get "library not found", run: `export LD_LIBRARY_PATH=.:$LD_LIBRARY_PATH`
- **Windows**: Make sure `libewma.dll` is in the same folder as your executable
- **macOS**: You might need to run: `xattr -d com.apple.quarantine libewma.dylib`
