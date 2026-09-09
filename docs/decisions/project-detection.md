# Project Detection

**Date:** 2025-09-09  
**Status:** Current

## Context

EasyKit needs to automatically detect the type of project in the current directory to provide relevant tool options.

## Decision

**Implement automatic project type detection based on file heuristics.**

## Implementation

### Detection Logic

```csharp
public class ProjectDetector
{
    public ProjectDetectionResult Detect(string directory)
    {
        var types = new List<string>();
        
        if (Directory.Exists(Path.Combine(directory, ".git")))
            types.Add("Git");
        if (File.Exists(Path.Combine(directory, "package.json")))
            types.Add("Node.js");
        if (File.Exists(Path.Combine(directory, "pnpm-lock.yaml")))
            types.Add("pnpm");
        if (File.Exists(Path.Combine(directory, "pyproject.toml")))
            types.Add("Python");
        if (File.Exists(Path.Combine(directory, "uv.lock")))
            types.Add("uv");
        if (File.Exists(Path.Combine(directory, "composer.json")))
            types.Add("PHP");
        if (Directory.GetFiles(directory, "*.csproj").Any())
            types.Add(".NET");
        if (File.Exists(Path.Combine(directory, "Dockerfile")))
            types.Add("Docker");
            
        return new ProjectDetectionResult(types);
    }
}
```

### Cache Strategy

- 5-minute cache to avoid repeated filesystem scans
- Cache key: directory path + modification time of detected files

## Display

The detected project types are shown in the main menu:
```
Project: Git + Node.js + pnpm
```

## Evidence

- `ProjectDetector.cs` - Implementation
- `ProjectDetectionResult.cs` - Data model
- `Program.cs` - Integration with main menu
- Tests: 24 unit tests for detection logic
