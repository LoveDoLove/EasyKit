# Testing Workflow

This document describes the testing workflow for EasyKit.

## Test Structure

```
tests/
└── EasyKit.Tests/
    ├── UnitTests/
    │   ├── ProjectDetectorTests.cs
    │   ├── SecureProcessRunnerTests.cs
    │   ├── ProjectDetectorComprehensiveTests.cs
    │   └── SecureProcessRunnerComprehensiveTests.cs
    └── EasyKit.Tests.csproj
```

## Running Tests

```powershell
# Run all tests
dotnet test tests/EasyKit.Tests/EasyKit.Tests.csproj

# Run with verbosity
dotnet test tests/EasyKit.Tests/EasyKit.Tests.csproj --verbosity normal

# Run specific test class
dotnet test tests/EasyKit.Tests/EasyKit.Tests.csproj --filter "FullyQualifiedName~SecureProcessRunnerTests"
```

## Test Requirements

1. **All existing tests must pass** - Currently 48 tests
2. **New features require tests** - Minimum coverage for new code
3. **Security tests are mandatory** - For any command execution changes

## Writing Tests

### Unit Test Pattern

```csharp
using EasyKit.Services;
using Xunit;

namespace EasyKit.Tests;

public class SecureProcessRunnerTests
{
    private readonly SecureProcessRunner _runner;

    public SecureProcessRunnerTests()
    {
        _runner = new SecureProcessRunner();
    }

    [Fact]
    public void RunProcess_AllowedCommand_Succeeds()
    {
        // Arrange
        var command = "where";
        var arguments = new[] { "notepad" };

        // Act
        var result = _runner.RunProcess(command, arguments);

        // Assert
        Assert.Equal(0, result.exitCode);
    }

    [Fact]
    public void RunProcess_DisallowedCommand_ThrowsSecurityException()
    {
        // Arrange
        var command = "powershell";
        var arguments = new[] { "-Command", "whoami" };

        // Act & Assert
        Assert.Throws<SecurityException>(() => _runner.RunProcess(command, arguments));
    }
}
```

### Test Categories

| Category | Description | Example |
|----------|-------------|---------|
| Happy Path | Normal operation | Command executes successfully |
| Security | Injection prevention | Dangerous chars rejected |
| Edge Cases | Boundary conditions | Empty args, null values |
| Error Handling | Failure scenarios | Invalid command, timeout |

## Test Coverage Goals

- **SecureProcessRunner**: 100% coverage (security-critical)
- **ProjectDetector**: 100% coverage (core feature)
- **Controllers**: Coverage for new features

## Continuous Integration

Tests run on:
- Every push to GitHub
- Every pull request
- Release builds

## Evidence

- 48 tests total
- All tests pass
- Security tests cover injection scenarios
