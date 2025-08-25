#!/bin/bash

# EWMA Library Build Script
# =========================

set -e

echo "EWMA Library Build Script"
echo "========================="

# Function to check if command exists
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Check prerequisites
echo "Checking prerequisites..."

if ! command_exists go; then
    echo "Error: Go is not installed or not in PATH"
    exit 1
fi

if ! command_exists gcc; then
    echo "Error: GCC is not installed or not in PATH"
    exit 1
fi

echo "✓ Go $(go version | cut -d' ' -f3)"
echo "✓ GCC $(gcc --version | head -n1)"

# Clean previous builds
echo ""
echo "Cleaning previous builds..."
rm -f libewma.so libewma.dll libewma.dylib libewma.a test_c

# Run tests
echo ""
echo "Running tests..."
CGO_ENABLED=1 go test -v

# Run benchmarks
echo ""
echo "Running benchmarks..."
CGO_ENABLED=1 go test -bench=.

# Build for current platform
echo ""
echo "Building for current platform..."
CGO_ENABLED=1 go build -buildmode=c-shared -o libewma.so ewma.go

# Build test program
echo ""
echo "Building test program..."
gcc -o test_c test_c.c -L. -lewma

# Run test
echo ""
echo "Running integration test..."
LD_LIBRARY_PATH=. ./test_c

echo ""
echo "Build completed successfully!"
echo ""
echo "Generated files:"
echo "  libewma.so  - Shared library"
echo "  libewma.h   - C header file"
echo "  test_c      - Test executable"
echo ""
echo "For .NET development:"
echo "  1. Copy libewma.so to your .NET project directory"
echo "  2. Use the wrapper classes in examples/ directory"
echo "  3. Update DLL name in DllImport attributes if needed"
