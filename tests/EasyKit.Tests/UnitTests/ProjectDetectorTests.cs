using EasyKit.Models;
using EasyKit.Services;
using Xunit;

namespace EasyKit.Tests;

public class ProjectDetectorTests
{
    private readonly ProjectDetector _detector;

    public ProjectDetectorTests()
    {
        _detector = new ProjectDetector();
    }

    [Fact]
    public void Detect_NonExistentDirectory_ReturnsNone()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            // Act
            var result = _detector.Detect(tempDir);

            // Assert
            Assert.Equal(tempDir, result.Directory);
            Assert.Empty(result.Types);
            Assert.False(result.IsGitRepo);
            Assert.False(result.IsPnpmProject);
            Assert.False(result.IsUvProject);
            Assert.False(result.IsLaravelProject);
            Assert.False(result.IsDotNetProject);
        }
        finally
        {
            Directory.Delete(tempDir, true);
            _detector.ClearCache();
        }
    }

    [Fact]
    public void Detect_GitRepository_DetectsGit()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        Directory.CreateDirectory(Path.Combine(tempDir, ".git"));

        try
        {
            // Act
            var result = _detector.Detect(tempDir);

            // Assert
            Assert.Contains(ProjectType.Git, result.Types);
            Assert.True(result.IsGitRepo);
        }
        finally
        {
            Directory.Delete(tempDir, true);
            _detector.ClearCache();
        }
    }

    [Fact]
    public void Detect_NodeJsProject_DetectsNodeJs()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        File.WriteAllText(Path.Combine(tempDir, "package.json"), "{}");

        try
        {
            // Act
            var result = _detector.Detect(tempDir);

            // Assert
            Assert.Contains(ProjectType.NodeJs, result.Types);
        }
        finally
        {
            Directory.Delete(tempDir, true);
            _detector.ClearCache();
        }
    }

    [Fact]
    public void Detect_PnpmProject_DetectsPnpm()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        File.WriteAllText(Path.Combine(tempDir, "package.json"), "{}");
        File.WriteAllText(Path.Combine(tempDir, "pnpm-lock.yaml"), "");

        try
        {
            // Act
            var result = _detector.Detect(tempDir);

            // Assert
            Assert.Contains(ProjectType.NodeJs, result.Types);
            Assert.Contains(ProjectType.Pnpm, result.Types);
            Assert.True(result.IsPnpmProject);
        }
        finally
        {
            Directory.Delete(tempDir, true);
            _detector.ClearCache();
        }
    }

    [Fact]
    public void Detect_PnpmWorkspaceProject_DetectsPnpm()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        File.WriteAllText(Path.Combine(tempDir, "package.json"), "{}");
        File.WriteAllText(Path.Combine(tempDir, "pnpm-workspace.yaml"), "packages:\n  - 'packages/*'");

        try
        {
            // Act
            var result = _detector.Detect(tempDir);

            // Assert
            Assert.Contains(ProjectType.Pnpm, result.Types);
            Assert.True(result.IsPnpmProject);
        }
        finally
        {
            Directory.Delete(tempDir, true);
            _detector.ClearCache();
        }
    }

    [Fact]
    public void Detect_UvProject_DetectsUv()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        File.WriteAllText(Path.Combine(tempDir, "pyproject.toml"), "[project]\nname = \"test\"");
        File.WriteAllText(Path.Combine(tempDir, "uv.lock"), "# uv lockfile");

        try
        {
            // Act
            var result = _detector.Detect(tempDir);

            // Assert
            Assert.Contains(ProjectType.Python, result.Types);
            Assert.Contains(ProjectType.Uv, result.Types);
            Assert.True(result.IsUvProject);
        }
        finally
        {
            Directory.Delete(tempDir, true);
            _detector.ClearCache();
        }
    }

    [Fact]
    public void Detect_ComposerProject_DetectsPhpAndComposer()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        File.WriteAllText(Path.Combine(tempDir, "composer.json"), "{}");

        try
        {
            // Act
            var result = _detector.Detect(tempDir);

            // Assert
            Assert.Contains(ProjectType.Php, result.Types);
            Assert.Contains(ProjectType.Composer, result.Types);
        }
        finally
        {
            Directory.Delete(tempDir, true);
            _detector.ClearCache();
        }
    }

    [Fact]
    public void Detect_LaravelProject_DetectsLaravel()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        File.WriteAllText(Path.Combine(tempDir, "composer.json"), "{}");
        File.WriteAllText(Path.Combine(tempDir, "artisan"), "#!/usr/bin/env php");

        try
        {
            // Act
            var result = _detector.Detect(tempDir);

            // Assert
            Assert.Contains(ProjectType.Laravel, result.Types);
            Assert.True(result.IsLaravelProject);
        }
        finally
        {
            Directory.Delete(tempDir, true);
            _detector.ClearCache();
        }
    }

    [Fact]
    public void Detect_DotNetProject_DetectsDotNet()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        File.WriteAllText(Path.Combine(tempDir, "TestProject.csproj"), @"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
</Project>");

        try
        {
            // Act
            var result = _detector.Detect(tempDir);

            // Assert
            Assert.Contains(ProjectType.DotNet, result.Types);
            Assert.True(result.IsDotNetProject);
        }
        finally
        {
            Directory.Delete(tempDir, true);
            _detector.ClearCache();
        }
    }

    [Fact]
    public void Detect_CacheResult_IsCached()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            // Act
            var result1 = _detector.Detect(tempDir);
            var result2 = _detector.Detect(tempDir);

            // Assert - Should return cached result (same object reference)
            Assert.Same(result1, result2);
        }
        finally
        {
            Directory.Delete(tempDir, true);
            _detector.ClearCache();
        }
    }

    [Fact]
    public void GetPrimaryPackageManager_PnpmProject_ReturnsPnpm()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        File.WriteAllText(Path.Combine(tempDir, "package.json"), "{}");
        File.WriteAllText(Path.Combine(tempDir, "pnpm-lock.yaml"), "");

        try
        {
            // Act
            var result = _detector.GetPrimaryPackageManager(tempDir);

            // Assert
            Assert.Equal("pnpm", result);
        }
        finally
        {
            Directory.Delete(tempDir, true);
            _detector.ClearCache();
        }
    }

    [Fact]
    public void GetPrimaryPackageManager_NpmProject_ReturnsNpm()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        File.WriteAllText(Path.Combine(tempDir, "package.json"), "{}");
        File.WriteAllText(Path.Combine(tempDir, "package-lock.json"), "{}");

        try
        {
            // Act
            var result = _detector.GetPrimaryPackageManager(tempDir);

            // Assert
            Assert.Equal("npm", result);
        }
        finally
        {
            Directory.Delete(tempDir, true);
            _detector.ClearCache();
        }
    }
}
