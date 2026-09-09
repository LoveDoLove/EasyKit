using EasyKit.Models;
using EasyKit.Services;
using Xunit;

namespace EasyKit.Tests.UnitTests;

public class ProjectDetectorComprehensiveTests
{
    private readonly ProjectDetector _detector;

    public ProjectDetectorComprehensiveTests()
    {
        _detector = new ProjectDetector();
    }

    [Fact]
    public void Detect_MixedProject_DetectsAllTypes()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        // Create a mixed project with multiple types
        File.WriteAllText(Path.Combine(tempDir, "package.json"), "{}");
        File.WriteAllText(Path.Combine(tempDir, "pnpm-lock.yaml"), "");
        File.WriteAllText(Path.Combine(tempDir, "pyproject.toml"), "[project]\nname = \"test\"");
        File.WriteAllText(Path.Combine(tempDir, "uv.lock"), "# uv lockfile");
        File.WriteAllText(Path.Combine(tempDir, "composer.json"), "{}");
        File.WriteAllText(Path.Combine(tempDir, "artisan"), "#!/usr/bin/env php");
        File.WriteAllText(Path.Combine(tempDir, "TestProject.csproj"), @"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
</Project>");
        File.WriteAllText(Path.Combine(tempDir, "Dockerfile"), "FROM node:latest");
        Directory.CreateDirectory(Path.Combine(tempDir, ".git"));

        try
        {
            // Act
            var result = _detector.Detect(tempDir);

            // Assert
            Assert.Contains(ProjectType.Git, result.Types);
            Assert.Contains(ProjectType.NodeJs, result.Types);
            Assert.Contains(ProjectType.Pnpm, result.Types);
            Assert.Contains(ProjectType.Python, result.Types);
            Assert.Contains(ProjectType.Uv, result.Types);
            Assert.Contains(ProjectType.Php, result.Types);
            Assert.Contains(ProjectType.Composer, result.Types);
            Assert.Contains(ProjectType.Laravel, result.Types);
            Assert.Contains(ProjectType.DotNet, result.Types);
            Assert.Contains(ProjectType.Docker, result.Types);
            
            Assert.True(result.IsGitRepo);
            Assert.True(result.IsPnpmProject);
            Assert.True(result.IsUvProject);
            Assert.True(result.IsLaravelProject);
            Assert.True(result.IsDotNetProject);
        }
        finally
        {
            Directory.Delete(tempDir, true);
            _detector.ClearCache();
        }
    }

    [Fact]
    public void Detect_NpmProjectWithLockfile_DetectsNpm()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        File.WriteAllText(Path.Combine(tempDir, "package.json"), "{}");
        File.WriteAllText(Path.Combine(tempDir, "package-lock.json"), "{}");

        try
        {
            // Act
            var result = _detector.Detect(tempDir);

            // Assert
            Assert.Contains(ProjectType.NodeJs, result.Types);
            Assert.DoesNotContain(ProjectType.Pnpm, result.Types);
            Assert.Equal("npm", _detector.GetPrimaryPackageManager(tempDir));
        }
        finally
        {
            Directory.Delete(tempDir, true);
            _detector.ClearCache();
        }
    }

    [Fact]
    public void Detect_PnpmProjectWithWorkspace_DetectsPnpm()
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
            Assert.Equal("pnpm", _detector.GetPrimaryPackageManager(tempDir));
        }
        finally
        {
            Directory.Delete(tempDir, true);
            _detector.ClearCache();
        }
    }

    [Fact]
    public void Detect_PythonWithVenv_DetectsPythonAndUv()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        Directory.CreateDirectory(Path.Combine(tempDir, ".venv"));
        File.WriteAllText(Path.Combine(tempDir, ".python-version"), "3.12.0");

        try
        {
            // Act
            var result = _detector.Detect(tempDir);

            // Assert
            Assert.Contains(ProjectType.Python, result.Types);
            // Note: uv detection requires uv.lock or pyproject.toml with uv marker
            Assert.DoesNotContain(ProjectType.Uv, result.Types);
        }
        finally
        {
            Directory.Delete(tempDir, true);
            _detector.ClearCache();
        }
    }

    [Fact]
    public void Detect_ComposerWithoutLaravel_DetectsPhpAndComposer()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        File.WriteAllText(Path.Combine(tempDir, "composer.json"), "{}");
        // No artisan file

        try
        {
            // Act
            var result = _detector.Detect(tempDir);

            // Assert
            Assert.Contains(ProjectType.Php, result.Types);
            Assert.Contains(ProjectType.Composer, result.Types);
            Assert.DoesNotContain(ProjectType.Laravel, result.Types);
            Assert.False(result.IsLaravelProject);
        }
        finally
        {
            Directory.Delete(tempDir, true);
            _detector.ClearCache();
        }
    }

    [Fact]
    public void Detect_Dockerfile_DetectsDocker()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        File.WriteAllText(Path.Combine(tempDir, "Dockerfile"), "FROM ubuntu:22.04");

        try
        {
            // Act
            var result = _detector.Detect(tempDir);

            // Assert
            Assert.Contains(ProjectType.Docker, result.Types);
        }
        finally
        {
            Directory.Delete(tempDir, true);
            _detector.ClearCache();
        }
    }

    [Fact]
    public void Detect_DockerCompose_DetectsDocker()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        File.WriteAllText(Path.Combine(tempDir, "docker-compose.yml"), "version: '3'\nservices:\n  app:\n    image: node:latest");

        try
        {
            // Act
            var result = _detector.Detect(tempDir);

            // Assert
            Assert.Contains(ProjectType.Docker, result.Types);
        }
        finally
        {
            Directory.Delete(tempDir, true);
            _detector.ClearCache();
        }
    }

    [Fact]
    public void Detect_VbProject_DetectsDotNet()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        File.WriteAllText(Path.Combine(tempDir, "TestProject.vbproj"), @"
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
    public void Detect_CacheInvalidation_ClearsCache()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            // Act - First detection
            var result1 = _detector.Detect(tempDir);
            
            // Create a git repo
            Directory.CreateDirectory(Path.Combine(tempDir, ".git"));
            
            // Act - Second detection without clearing cache
            var result2 = _detector.Detect(tempDir);
            
            // Assert - Should return cached result
            Assert.Same(result1, result2);
            
            // Clear cache
            _detector.ClearCache();
            
            // Act - Third detection after clearing cache
            var result3 = _detector.Detect(tempDir);
            
            // Assert - Should be different result
            Assert.NotSame(result1, result3);
            Assert.Contains(ProjectType.Git, result3.Types);
        }
        finally
        {
            Directory.Delete(tempDir, true);
            _detector.ClearCache();
        }
    }

    [Fact]
    public void GetPrimaryPackageManager_UvProject_ReturnsUv()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        File.WriteAllText(Path.Combine(tempDir, "pyproject.toml"), "[project]\nname = \"test\"");
        File.WriteAllText(Path.Combine(tempDir, "uv.lock"), "# uv lockfile");

        try
        {
            // Act
            var result = _detector.GetPrimaryPackageManager(tempDir);

            // Assert
            Assert.Equal("uv", result);
        }
        finally
        {
            Directory.Delete(tempDir, true);
            _detector.ClearCache();
        }
    }

    [Fact]
    public void GetPrimaryPackageManager_ComposerProject_ReturnsComposer()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        File.WriteAllText(Path.Combine(tempDir, "composer.json"), "{}");

        try
        {
            // Act
            var result = _detector.GetPrimaryPackageManager(tempDir);

            // Assert
            Assert.Equal("composer", result);
        }
        finally
        {
            Directory.Delete(tempDir, true);
            _detector.ClearCache();
        }
    }

    [Fact]
    public void GetPrimaryPackageManager_NonProjectDirectory_ReturnsUnknown()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            // Act
            var result = _detector.GetPrimaryPackageManager(tempDir);

            // Assert
            Assert.Equal("unknown", result);
        }
        finally
        {
            Directory.Delete(tempDir, true);
            _detector.ClearCache();
        }
    }
}
