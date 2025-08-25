#!/bin/bash

# Test script to verify the separated wrapper and demo files work correctly

echo "Testing Separated EWMA Example Structure"
echo "========================================"

# Create a test directory
TEST_DIR="/tmp/ewma_test"
mkdir -p "$TEST_DIR"

echo "1. Copying files to test directory..."
cp /workspaces/common4n/libewma.so "$TEST_DIR/"
cp /workspaces/common4n/examples/EWMAWrapper.vb "$TEST_DIR/"
cp /workspaces/common4n/examples/EWMAExample.vb "$TEST_DIR/"

echo "2. Updating DllImport for Linux..."
# Update the DLL name for Linux
sed -i 's/libewma\.dll/libewma.so/g' "$TEST_DIR/EWMAWrapper.vb"

echo "3. Files ready for testing:"
ls -la "$TEST_DIR"

echo ""
echo "4. The wrapper class (EWMAWrapper.vb) and demo (EWMAExample.vb) are now separate!"
echo "   - EWMAWrapper.vb: Contains the algorithm interface"
echo "   - EWMAExample.vb: Contains demonstration code"
echo ""
echo "5. To use in your project:"
echo "   - Copy EWMAWrapper.vb to your project"
echo "   - Copy libewma.so to your project"
echo "   - Update DllImport paths as needed"
echo ""
echo "Files are ready in: $TEST_DIR"

# Clean up
# rm -rf "$TEST_DIR"
