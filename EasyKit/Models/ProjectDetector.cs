// MIT License
// 
// Copyright (c) 2025 LoveDoLove
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Text.Json;
using EasyKit.Services;

namespace EasyKit.Models;

/// <summary>
///     Detects project types based on files and directories present.
///     Results are cached for performance.
/// </summary>
public class ProjectDetector
{
    private readonly SecureProcessRunner _processRunner;
    private readonly Dictionary<string, ProjectDetectionResult> _cache = new();
    private readonly Dictionary<string, DateTime> _cacheTimestamps = new();
    private readonly TimeSpan _cacheTimeout = TimeSpan.FromMinutes(5);

    public ProjectDetector(SecureProcessRunner? processRunner = null)
    {
        _processRunner = processRunner ?? new SecureProcessRunner();
    }

    /// <summary>
    ///     Detects project types in the specified directory.
    /// </summary>
    public ProjectDetectionResult Detect(string directory)
    {
        // Check cache first
        if (_cache.TryGetValue(directory, out var cached) &&
            _cacheTimestamps.TryGetValue(directory, out var timestamp) &&
            DateTime.Now - timestamp < _cacheTimeout)
        {
            return cached;
        }

        var result = DetectInternal(directory);
        _cache[directory] = result;
        _cacheTimestamps[directory] = DateTime.Now;
        return result;
    }

    private ProjectDetectionResult DetectInternal(string directory)
    {
        var types = new List<ProjectType>();
        var absPath = Path.GetFullPath(directory);

        // Git
        if (Directory.Exists(Path.Combine(absPath, ".git")))
            types.Add(ProjectType.Git);

        // Node.js / pnpm
        var packageJson = Path.Combine(absPath, "package.json");
        var pnpmLock = Path.Combine(absPath, "pnpm-lock.yaml");
        var pnpmWorkspace = Path.Combine(absPath, "pnpm-workspace.yaml");
        
        if (File.Exists(packageJson))
        {
            types.Add(ProjectType.NodeJs);
            
            // Check for pnpm
            bool isPnpm = File.Exists(pnpmLock) || 
                         File.Exists(pnpmWorkspace) ||
                         ReadPackagePackageManager(absPath) == "pnpm";
            if (isPnpm)
                types.Add(ProjectType.Pnpm);
        }

        // Python / uv
        var pyprojectToml = Path.Combine(absPath, "pyproject.toml");
        var uvLock = Path.Combine(absPath, "uv.lock");
        var pythonVersion = Path.Combine(absPath, ".python-version");
        var venvDir = Path.Combine(absPath, ".venv");
        var requirementsTxt = Path.Combine(absPath, "requirements.txt");

        bool hasPythonFiles = File.Exists(pyprojectToml) || 
                             File.Exists(uvLock) || 
                             File.Exists(pythonVersion) ||
                             Directory.Exists(venvDir) ||
                             File.Exists(requirementsTxt);
        
        if (hasPythonFiles)
        {
            types.Add(ProjectType.Python);
            if (File.Exists(uvLock) || (File.Exists(pyprojectToml) && IsUvProject(absPath)))
                types.Add(ProjectType.Uv);
        }

        // PHP / Composer / Laravel
        var composerJson = Path.Combine(absPath, "composer.json");
        var artisan = Path.Combine(absPath, "artisan");

        if (File.Exists(composerJson))
        {
            types.Add(ProjectType.Php);
            types.Add(ProjectType.Composer);
            if (File.Exists(artisan))
                types.Add(ProjectType.Laravel);
        }

        // .NET
        var csprojFiles = Directory.GetFiles(absPath, "*.csproj", SearchOption.TopDirectoryOnly);
        var vbprojFiles = Directory.GetFiles(absPath, "*.vbproj", SearchOption.TopDirectoryOnly);
        if (csprojFiles.Length > 0 || vbprojFiles.Length > 0)
            types.Add(ProjectType.DotNet);

        // Docker
        var dockerfile = Path.Combine(absPath, "Dockerfile");
        var dockerComposeFiles = Directory.GetFiles(absPath, "docker-compose*.yml", SearchOption.TopDirectoryOnly);
        if (File.Exists(dockerfile) || dockerComposeFiles.Length > 0)
            types.Add(ProjectType.Docker);

        bool isPnpmProject = types.Contains(ProjectType.Pnpm);
        bool isUvProject = types.Contains(ProjectType.Uv);
        bool isGitRepo = types.Contains(ProjectType.Git);
        bool isLaravelProject = types.Contains(ProjectType.Laravel);
        bool isDotNetProject = types.Contains(ProjectType.DotNet);

        return new ProjectDetectionResult(
            absPath,
            types,
            isPnpmProject,
            isUvProject,
            isGitRepo,
            isLaravelProject,
            isDotNetProject);
    }

    private string? ReadPackagePackageManager(string directory)
    {
        var path = Path.Combine(directory, "package.json");
        if (!File.Exists(path)) return null;

        try
        {
            var json = File.ReadAllText(path);
            var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("packageManager", out var pm))
                return pm.GetString();
        }
        catch
        {
            // Ignore parse errors
        }
        return null;
    }

    private bool IsUvProject(string directory)
    {
        var pyprojectPath = Path.Combine(directory, "pyproject.toml");
        if (!File.Exists(pyprojectPath)) return false;

        try
        {
            var content = File.ReadAllText(pyprojectPath);
            return content.Contains("uv") || content.Contains("[tool.uv]");
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    ///     Clears the detection cache.
    /// </summary>
    public void ClearCache()
    {
        _cache.Clear();
        _cacheTimestamps.Clear();
    }

    /// <summary>
    ///     Gets the primary package manager for a directory.
    /// </summary>
    public string GetPrimaryPackageManager(string directory)
    {
        var detection = Detect(directory);
        
        if (detection.IsPnpmProject)
            return "pnpm";
        if (detection.IsUvProject)
            return "uv";
        if (detection.Types.Contains(ProjectType.NodeJs))
            return "npm";
        if (detection.Types.Contains(ProjectType.Composer))
            return "composer";
        if (detection.Types.Contains(ProjectType.Python))
            return "python";
        
        return "unknown";
    }
}
