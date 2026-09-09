using System.Security;
using EasyKit.Services;
using Xunit;

namespace EasyKit.Tests.UnitTests;

public class SecureProcessRunnerComprehensiveTests
{
    private readonly SecureProcessRunner _runner;

    public SecureProcessRunnerComprehensiveTests()
    {
        _runner = new SecureProcessRunner();
    }

    [Theory]
    [InlineData("where", new[] { "notepad" }, 0)]
    [InlineData("where", new[] { "calc" }, 0)]
    [InlineData("where", new[] { "notepad", "calc" }, 0)]
    public void RunProcess_ValidCommand_Succeeds(string command, string[] arguments, int expectedExitCode)
    {
        // Act
        var result = _runner.RunProcess(command, arguments);

        // Assert
        Assert.Equal(expectedExitCode, result.exitCode);
    }

    [Theory]
    [InlineData("invalid_command_that_does_not_exist_12345", new[] { "arg" })]
    [InlineData("powershell", new[] { "-Command", "whoami" })]
    public void RunProcess_DisallowedCommand_ThrowsSecurityException(string command, string[] arguments)
    {
        // Act & Assert
        Assert.Throws<SecurityException>(() => _runner.RunProcess(command, arguments));
    }

    [Theory]
    [InlineData("where", new[] { "; rm -rf /" })]
    [InlineData("where", new[] { "| cat" })]
    [InlineData("where", new[] { "& dir" })]
    [InlineData("where", new[] { "` whoami" })]
    [InlineData("where", new[] { "$(whoami)" })]
    public void RunProcess_DangerousCharactersInArgument_ThrowsSecurityException(string command, string[] arguments)
    {
        // Act & Assert
        Assert.Throws<SecurityException>(() => _runner.RunProcess(command, arguments));
    }

    [Theory]
    [InlineData("where", new[] { "notepad" }, 0)]
    [InlineData("where", new[] { "calc" }, 0)]
    [InlineData("where", new[] { "cmd" }, 0)]
    public void RunProcess_SafeArguments_Succeeds(string command, string[] arguments, int expectedExitCode)
    {
        // Act
        var result = _runner.RunProcess(command, arguments);

        // Assert
        Assert.Equal(expectedExitCode, result.exitCode);
    }

    [Fact]
    public void RunProcess_EmptyArguments_ReturnsUsageError()
    {
        // Act
        var result = _runner.RunProcess("where", Array.Empty<string>());

        // Assert - where.exe returns 2 when no pattern is provided
        Assert.Equal(2, result.exitCode);
    }

    [Fact]
    public void RunProcess_NullArguments_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _runner.RunProcess("", null!));
    }

    [Fact]
    public void RunProcess_EmptyCommand_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _runner.RunProcess("", ["arg"]));
    }

    [Fact]
    public void RunProcess_PathTraversalInArgument_ThrowsSecurityException()
    {
        // Act & Assert
        Assert.Throws<SecurityException>(() => _runner.RunProcess("where", ["../../../etc/passwd"]));
    }

    [Fact]
    public void RunProcess_ValidRelativePath_Accepted()
    {
        // Act
        var result = _runner.RunProcess("where", ["EasyKit"]);

        // Assert - should succeed or fail due to path not existing, not due to security
        Assert.True(result.exitCode == 0 || result.exitCode == 1);
    }
}
