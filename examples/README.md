# Examples Directory

This directory contains the wrapper classes and demonstration programs for the EWMA library.

## File Structure

### Wrapper Classes (Algorithm Implementation)
- **`EWMAWrapper.vb`** - VB.NET wrapper class for the EWMA Go library
- **`EWMAWrapper.cs`** - C# wrapper class for the EWMA Go library

These files contain the actual interface to the native Go library and should be included in your projects.

### Demo Programs (Usage Examples)
- **`EWMAExample.vb`** - Comprehensive VB.NET demonstration program
- **`EWMAExample.cs`** - Comprehensive C# demonstration program

These files show how to use the wrapper classes and demonstrate all available features.

## Usage Instructions

### For VB.NET Projects

1. **Copy the wrapper class:**
   ```
   Copy EWMAWrapper.vb to your project
   ```

2. **Copy the shared library:**
   ```bash
   # Linux/macOS
   cp ../libewma.so /path/to/your/project/
   
   # Windows
   copy ..\libewma.dll C:\path\to\your\project\
   ```

3. **Update DllImport paths if needed:**
   - For Linux: Change `"libewma.dll"` to `"libewma.so"`
   - For macOS: Change `"libewma.dll"` to `"libewma.dylib"`

4. **Basic usage:**
   ```vb
   Using ewma As New EWMAWrapper(0.3)
       Dim result As Double = ewma.Update(10.0)
       Console.WriteLine(result)
   End Using
   ```

### For C# Projects

1. **Copy the wrapper class:**
   ```
   Copy EWMAWrapper.cs to your project
   ```

2. **Copy the shared library:**
   ```bash
   # Linux/macOS
   cp ../libewma.so /path/to/your/project/
   
   # Windows
   copy ..\libewma.dll C:\path\to\your\project\
   ```

3. **Update DllImport paths if needed:**
   - For Linux: Change `"libewma.dll"` to `"libewma.so"`
   - For macOS: Change `"libewma.dll"` to `"libewma.dylib"`

4. **Basic usage:**
   ```csharp
   using (var ewma = new EWMAWrapper(0.3))
   {
       double result = ewma.Update(10.0);
       Console.WriteLine(result);
   }
   ```

## Running the Demos

### VB.NET Demo
```bash
# Compile and run (requires VB.NET compiler)
vbc EWMAExample.vb EWMAWrapper.vb -reference:System.Linq.dll
./EWMAExample.exe
```

### C# Demo
```bash
# Compile and run (requires C# compiler)
csc EWMAExample.cs EWMAWrapper.cs
./EWMAExample.exe
```

## Features Demonstrated

Both demo programs showcase:

1. **Basic Usage** - Individual value updates
2. **Batch Processing** - Processing arrays of values efficiently
3. **State Management** - Save/restore EWMA state using JSON
4. **Alpha Parameter Changes** - Dynamically adjusting smoothing factor
5. **Performance Testing** - High-frequency processing capabilities

## Key Classes and Methods

### EWMAWrapper Class

#### Constructor
- `EWMAWrapper(double alpha)` - Creates new instance with smoothing factor

#### Core Methods
- `Update(double value)` - Updates EWMA with new value
- `GetValue()` - Gets current EWMA value
- `Reset()` - Resets to initial state
- `SetAlpha(double alpha)` - Changes smoothing factor

#### Batch Processing
- `ProcessBatch(double[] values)` - Returns all intermediate results
- `ProcessBatchFinal(double[] values)` - Returns only final result

#### State Management
- `GetStateAsJSON()` - Exports state as JSON string
- `SetStateFromJSON(string json)` - Imports state from JSON
- `Clone()` - Creates a copy of the instance

#### Resource Management
- `Dispose()` - Properly releases native resources
- Implements `IDisposable` pattern for automatic cleanup

## Notes

- Both wrapper classes provide identical functionality
- All memory management is handled automatically
- Thread-safety depends on usage pattern (create separate instances for concurrent access)
- The shared library must be in the same directory as your executable or in the system PATH
