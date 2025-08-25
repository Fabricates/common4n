@echo off
REM EWMA Library Build Script for Windows
REM =====================================

echo EWMA Library Build Script for Windows
echo ====================================

REM Check if Go is installed
where go >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo Error: Go is not installed or not in PATH
    exit /b 1
)

REM Check if GCC is installed (MinGW)
where gcc >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo Error: GCC is not installed or not in PATH
    echo Please install MinGW-w64 or TDM-GCC
    exit /b 1
)

echo ✓ Go found
echo ✓ GCC found

REM Clean previous builds
echo.
echo Cleaning previous builds...
del /Q libewma.* test_c.exe 2>nul

REM Run tests
echo.
echo Running tests...
set CGO_ENABLED=1
go test -v
if %ERRORLEVEL% NEQ 0 (
    echo Tests failed!
    exit /b 1
)

REM Run benchmarks
echo.
echo Running benchmarks...
go test -bench=.

REM Build for Windows
echo.
echo Building for Windows...
go build -buildmode=c-shared -o libewma.dll ewma.go
if %ERRORLEVEL% NEQ 0 (
    echo Build failed!
    exit /b 1
)

echo.
echo Build completed successfully!
echo.
echo Generated files:
echo   libewma.dll - Windows DLL
echo   libewma.h   - C header file
echo.
echo For .NET development:
echo   1. Copy libewma.dll to your .NET project directory
echo   2. Use the wrapper classes in examples/ directory
echo   3. Make sure DllImport uses "libewma.dll"
echo.
pause
