# Makefile for EWMA Go Library

# Variables
LIBRARY_NAME = libewma
GO_FILES = ewma.go
TEST_FILES = ewma_test.go

# Build targets
.PHONY: all clean test build-windows build-linux build-darwin

all: build-windows build-linux

# Test the Go code
test:
	@echo "Running Go tests..."
	go test -v
	@echo "Running benchmarks..."
	go test -bench=.

# Build shared library for Windows (DLL)
build-windows:
	@echo "Building Windows DLL..."
	GOOS=windows GOARCH=amd64 CGO_ENABLED=1 CC=x86_64-w64-mingw32-gcc go build -buildmode=c-shared -o $(LIBRARY_NAME).dll $(GO_FILES)
	@echo "Windows DLL built: $(LIBRARY_NAME).dll"

# Build shared library for Linux (SO)
build-linux:
	@echo "Building Linux shared library..."
	GOOS=linux GOARCH=amd64 CGO_ENABLED=1 go build -buildmode=c-shared -o $(LIBRARY_NAME).so $(GO_FILES)
	@echo "Linux shared library built: $(LIBRARY_NAME).so"

# Build shared library for macOS (DYLIB)
build-darwin:
	@echo "Building macOS shared library..."
	GOOS=darwin GOARCH=amd64 CGO_ENABLED=1 go build -buildmode=c-shared -o $(LIBRARY_NAME).dylib $(GO_FILES)
	@echo "macOS shared library built: $(LIBRARY_NAME).dylib"

# Build static library
build-static:
	@echo "Building static library..."
	go build -buildmode=c-archive -o $(LIBRARY_NAME).a $(GO_FILES)
	@echo "Static library built: $(LIBRARY_NAME).a"

# Run the example
run-example:
	@echo "Running EWMA example..."
	go run $(GO_FILES)

# Clean build artifacts
clean:
	@echo "Cleaning build artifacts..."
	rm -f $(LIBRARY_NAME).dll $(LIBRARY_NAME).so $(LIBRARY_NAME).dylib $(LIBRARY_NAME).a $(LIBRARY_NAME).h
	@echo "Clean complete."

# Install dependencies (if any)
deps:
	@echo "Installing dependencies..."
	go mod tidy
	go mod download

# Format code
fmt:
	@echo "Formatting Go code..."
	go fmt ./...

# Lint code (requires golangci-lint)
lint:
	@echo "Linting Go code..."
	golangci-lint run

# Generate documentation
docs:
	@echo "Generating documentation..."
	go doc -all > GODOC.md

# Help
help:
	@echo "Available targets:"
	@echo "  all           - Build for Windows and Linux"
	@echo "  test          - Run tests and benchmarks"
	@echo "  build-windows - Build Windows DLL"
	@echo "  build-linux   - Build Linux shared library"
	@echo "  build-darwin  - Build macOS shared library"
	@echo "  build-static  - Build static library"
	@echo "  run-example   - Run the example program"
	@echo "  clean         - Clean build artifacts"
	@echo "  deps          - Install dependencies"
	@echo "  fmt           - Format code"
	@echo "  lint          - Lint code"
	@echo "  docs          - Generate documentation"
	@echo "  help          - Show this help message"
