using System.Security;
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

    [Fact]
    public void RunProcess_DangerousCharactersInArgument_ThrowsSecurityException()
    {
        // Arrange
        var command = "where";
        var arguments = new[] { "; rm -rf /" };

        // Act & Assert
        Assert.Throws<SecurityException>(() => _runner.RunProcess(command, arguments));
    }

    [Fact]
    public void RunProcess_ValidArguments_Succeeds()
    {
        // Arrange
        var command = "where";
        var arguments = new[] { "notepad", "calc" };

        // Act
        var result = _runner.RunProcess(command, arguments);

        // Assert
        Assert.Equal(0, result.exitCode);
    }

    [Fact]
    public void RunProcess_EmptyArguments_ReturnsUsageError()
    {
        // Arrange
        var command = "where";
        var arguments = Array.Empty<string>();

        // Act
        var result = _runner.RunProcess(command, arguments);

        // Assert - where.exe returns 2 when no pattern is provided
        Assert.Equal(2, result.exitCode);
    }

    [Fact]
    public void RunProcess_InvalidCommand_ThrowsArgumentException()
    {
        // Arrange
        var command = "";
        var arguments = Array.Empty<string>();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _runner.RunProcess(command, arguments));
    }
}
