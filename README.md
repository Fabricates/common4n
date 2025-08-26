# EWMA (Exponential Weighted Moving Average) Go Library

A high-performance Go library for calculating Exponential Weighted Moving Averages, designed to be easily integrated with Visual Basic .NET and C# applications.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Installation](#installation)
- [Building the Library](#building-the-library)
- [Usage](#usage)
  - [Go Usage](#go-usage)
  - [Visual Basic .NET Usage](#visual-basic-net-usage)
  - [C# Usage](#c-usage)
- [API Reference](#api-reference)
- [Mathematical Background](#mathematical-background)
- [Performance](#performance)
- [Examples](#examples)
- [Troubleshooting](#troubleshooting)
- [Contributing](#contributing)
- [License](#license)

## Overview

The EWMA (Exponential Weighted Moving Average) is a statistical method for smoothing time series data by giving exponentially decreasing weights to older observations. This library provides a fast, thread-safe implementation in Go that can be compiled as a shared library for use in .NET applications.

### Key Benefits

- **High Performance**: Written in Go for optimal speed
- **Memory Efficient**: Minimal memory footprint
- **Cross-Platform**: Supports Windows, Linux, and macOS
- **Easy Integration**: Simple C-style API for .NET interop
- **State Persistence**: JSON serialization/deserialization support
- **Batch Processing**: Efficient batch calculation support

## Features

- ✅ Thread-safe EWMA calculations
- ✅ Configurable alpha (smoothing factor) parameter
- ✅ Batch processing capabilities
- ✅ State serialization to JSON
- ✅ Multiple instance management
- ✅ Memory leak prevention
- ✅ Comprehensive error handling
- ✅ .NET wrapper classes (VB.NET and C#)
- ✅ Unit tests and benchmarks

## Installation

### Prerequisites

- Go 1.21 or later
- CGO enabled
- GCC compiler (for building shared libraries)
- .NET Framework or .NET Core (for .NET examples)

### For Windows Development

Install MinGW-w64 for cross-compilation:

```bash
# Ubuntu/Debian
sudo apt-get install gcc-mingw-w64

# macOS with Homebrew
brew install mingw-w64
```

## Building the Library

### Quick Start

```bash
# Clone the repository
git clone https://github.com/Fabricates/common4n.git
cd common4n

# Run tests
make test

# Build for all platforms
make all

# Build for specific platform
make build-windows  # Creates libewma.dll
make build-linux    # Creates libewma.so
make build-darwin   # Creates libewma.dylib
```

### Manual Build

```bash
# For Windows DLL
CGO_ENABLED=1 GOOS=windows GOARCH=amd64 CC=x86_64-w64-mingw32-gcc go build -buildmode=c-shared -o libewma.dll ewma.go

# For Linux SO
CGO_ENABLED=1 GOOS=linux GOARCH=amd64 go build -buildmode=c-shared -o libewma.so ewma.go

# For macOS DYLIB
CGO_ENABLED=1 GOOS=darwin GOARCH=amd64 go build -buildmode=c-shared -o libewma.dylib ewma.go
```

## Usage

### Go Usage

```go
package main

import (
    "fmt"
)

func main() {
    // Create new EWMA with alpha = 0.3
    ewma := NewEWMA(0.3)
    
    // Process individual values
    fmt.Println(ewma.Update(10.0))  // 10.0
    fmt.Println(ewma.Update(20.0))  // 13.0
    fmt.Println(ewma.Update(15.0))  // 13.6
    
    // Batch processing
    values := []float64{25.0, 30.0, 18.0}
    results := ewma.CalculateBatch(values)
    fmt.Println(results)
    
    // State management
    jsonState := ewma.ToJSON()
    fmt.Println("State:", jsonState)
}
```

### Visual Basic .NET Usage

```vb
Imports System

Module Program
    Sub Main()
        ' Create EWMA wrapper
        Using ewma As New EWMAWrapper(0.3)
            ' Update with individual values
            Console.WriteLine(ewma.Update(10.0))  ' 10.0
            Console.WriteLine(ewma.Update(20.0))  ' 13.0
            Console.WriteLine(ewma.Update(15.0))  ' 13.6
            
            ' Batch processing
            Dim values() As Double = {25.0, 30.0, 18.0}
            Dim results() As Double = ewma.ProcessBatch(values)
            
            For Each result In results
                Console.WriteLine(result)
            Next
            
            ' State management
            Dim state As String = ewma.GetStateAsJSON()
            Console.WriteLine("State: " & state)
        End Using
    End Sub
End Module
```

### C# Usage

```csharp
using System;

class Program
{
    static void Main()
    {
        // Create EWMA wrapper
        using (var ewma = new EWMAWrapper(0.3))
        {
            // Update with individual values
            Console.WriteLine(ewma.Update(10.0));  // 10.0
            Console.WriteLine(ewma.Update(20.0));  // 13.0
            Console.WriteLine(ewma.Update(15.0));  // 13.6
            
            // Batch processing
            double[] values = {25.0, 30.0, 18.0};
            double[] results = ewma.ProcessBatch(values);
            
            foreach (var result in results)
            {
                Console.WriteLine(result);
            }
            
            // State management
            string state = ewma.GetStateAsJSON();
            Console.WriteLine($"State: {state}");
        }
    }
}
```

## API Reference

### Go API

#### `NewEWMA(alpha float64) *EWMA`
Creates a new EWMA instance with the specified alpha parameter.

**Parameters:**
- `alpha`: Smoothing factor (0 < alpha ≤ 1)

**Returns:** Pointer to EWMA instance

#### `Update(value float64) float64`
Updates the EWMA with a new observation.

**Parameters:**
- `value`: New observation value

**Returns:** Updated EWMA value

#### `GetValue() float64`
Returns the current EWMA value.

#### `Reset()`
Resets the EWMA to its initial state.

#### `SetAlpha(alpha float64) bool`
Updates the alpha parameter.

**Parameters:**
- `alpha`: New smoothing factor

**Returns:** True if successful, false if invalid alpha

#### `CalculateBatch(values []float64) []float64`
Calculates EWMA for a batch of values.

**Parameters:**
- `values`: Slice of values to process

**Returns:** Slice of EWMA results

#### `ToJSON() string`
Serializes the EWMA state to JSON.

#### `FromJSON(jsonStr string) bool`
Deserializes EWMA state from JSON.

### C API (for .NET interop)

#### `CreateEWMA(alpha float64) int`
Creates a new EWMA instance and returns its ID.

#### `UpdateEWMA(instanceID int, value float64) float64`
Updates the specified EWMA instance with a new value.

#### `GetEWMAValue(instanceID int) float64`
Gets the current value of the specified EWMA instance.

#### `ResetEWMA(instanceID int) bool`
Resets the specified EWMA instance.

#### `SetEWMAAlpha(instanceID int, alpha float64) bool`
Sets the alpha parameter for the specified instance.

#### `DestroyEWMA(instanceID int) bool`
Destroys the specified EWMA instance and frees memory.

#### `GetEWMAStateJSON(instanceID int) *char`
Gets the state of the specified instance as JSON string.

#### `SetEWMAStateJSON(instanceID int, jsonStr *char) bool`
Sets the state of the specified instance from JSON string.

#### `FreeString(str *char)`
Frees memory allocated for C strings.

### .NET Wrapper API

#### `EWMAWrapper(double alpha)`
Constructor that creates a new EWMA instance.

#### `double Update(double value)`
Updates the EWMA with a new value and returns the result.

#### `double GetValue()`
Gets the current EWMA value.

#### `void Reset()`
Resets the EWMA to its initial state.

#### `bool SetAlpha(double alpha)`
Sets a new alpha value.

#### `double[] ProcessBatch(double[] values)`
Processes an array of values and returns the EWMA results.

#### `string GetStateAsJSON()`
Gets the current state as a JSON string.

#### `bool SetStateFromJSON(string jsonStr)`
Sets the state from a JSON string.

## Mathematical Background

The Exponential Weighted Moving Average is calculated using the formula:

```
EWMA(t) = α × X(t) + (1 - α) × EWMA(t-1)
```

Where:
- `EWMA(t)` is the EWMA value at time t
- `X(t)` is the observation at time t
- `α` (alpha) is the smoothing factor (0 < α ≤ 1)
- `EWMA(t-1)` is the previous EWMA value

### Alpha Parameter

The alpha parameter controls how much weight is given to recent observations:

- **Higher α (closer to 1)**: More responsive to recent changes, less smoothing
- **Lower α (closer to 0)**: More smoothing, less responsive to recent changes

Common alpha values:
- `0.1`: Heavy smoothing, slow response
- `0.3`: Moderate smoothing, balanced response
- `0.7`: Light smoothing, fast response

## Performance

### Benchmarks

Based on Go benchmarks (run `make test` to see current results):

```
BenchmarkEWMAUpdate-8    	100000000	        12.5 ns/op
BenchmarkEWMABatch-8     	    5000	    325463 ns/op
```

- Single update: ~12.5 nanoseconds per operation
- Batch processing: ~325 microseconds for 1000 values

### Memory Usage

- Each EWMA instance uses approximately 32 bytes
- No memory allocations during updates
- Batch processing allocates memory for result slice only

## Examples

See the `examples/` directory for complete working examples:

### Wrapper Classes (Include in Your Projects)
- `EWMAWrapper.vb` - VB.NET wrapper class for the EWMA library
- `EWMAWrapper.cs` - C# wrapper class for the EWMA library

### Demo Programs (Learn How to Use)
- `EWMAExample.vb` - Comprehensive VB.NET demonstration
- `EWMAExample.cs` - Comprehensive C# demonstration

### Running Examples

1. Build the library for your platform
2. Copy the appropriate shared library (`.dll`, `.so`, or `.dylib`) to your project directory
3. Compile and run the example:

```bash
# For C# example
csc EWMAExample.cs
./EWMAExample.exe

# For VB.NET example
vbc EWMAExample.vb
./EWMAExample.exe
```

## Troubleshooting

### Common Issues

#### "DLL not found" Error
**Solution:** Ensure the shared library is in the same directory as your executable or in the system PATH.

#### "Entry point not found" Error
**Solution:** Verify that the DLL was built correctly and exports the required functions.

#### Memory Access Violations
**Solution:** Always call `Dispose()` on the wrapper class or use `using` statements.

#### Invalid Alpha Values
**Solution:** Ensure alpha is between 0 and 1 (exclusive of 0, inclusive of 1).

### Debug Mode

To enable debug output, set the environment variable:

```bash
export GODEBUG=cgocheck=1
```

### Platform-Specific Notes

#### Windows
- Requires Visual C++ Redistributable
- Use `libewma.dll`

#### Linux
- May require `sudo ldconfig` after installation
- Use `libewma.so`

#### macOS
- May need to bypass Gatekeeper: `xattr -d com.apple.quarantine libewma.dylib`
- Use `libewma.dylib`

## Contributing

### Development Setup

```bash
# Clone repository
git clone https://github.com/Fabricates/common4n.git
cd common4n

# Install dependencies
make deps

# Run tests
make test

# Format code
make fmt

# Lint code (requires golangci-lint)
make lint
```

### Submitting Changes

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Run the test suite
6. Submit a pull request

### Testing

```bash
# Run unit tests
go test -v

# Run benchmarks
go test -bench=.

# Run with race detection
go test -race

# Test all platforms
make test
```

## License

This project is licensed under the terms specified in the LICENSE file.

## Support

For questions, issues, or contributions:

- GitHub Issues: [https://github.com/Fabricates/common4n/issues](https://github.com/Fabricates/common4n/issues)
- Documentation: This README and inline code comments
- Examples: See the `examples/` directory

---

**Note:** This library is designed for production use but should be thoroughly tested in your specific environment before deployment.
