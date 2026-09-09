# EasyKit 全面工程审计报告

## 执行摘要

EasyKit 是一个基于 .NET 8 的 Windows 开发工具箱，提供 Git、NPM、Composer、Laravel 等开发工具的统一控制台界面。本次审计发现：

- **P0 严重问题**: 3 个（Shell 注入漏洞、无输入验证、PATH 劫持风险）
- **P1 高优先级**: 7 个（零测试覆盖、uv 缺失、项目检测缺失等）
- **P2 中优先级**: 5 个（代码重复、硬编码字符串等）
- **P3 低优先级**: 3 个（文档过时等）

---

## A. Repository Audit - 当前架构和实现总结

### 1. 解决方案结构

```
EasyKit/
├── EasyKit.sln                    # .NET 8 解决方案
├── EasyKit/                       # 主控制台应用
│   ├── Controllers/               # 控制器层 (7 个)
│   │   ├── GitController.cs       # Git 操作 (1069 行)
│   │   ├── NpmController.cs       # NPM 操作 (252 行)
│   │   ├── PnpmController.cs      # pnpm 操作 (385 行)
│   │   ├── ComposerController.cs  # Composer 操作 (238 行)
│   │   ├── LaravelController.cs   # Laravel 操作 (520 行)
│   │   ├── SettingsController.cs  # 设置 (191 行)
│   │   └── ToolMarketplaceController.cs  # 工具市场 (108 行)
│   ├── Services/
│   │   ├── CmdService.cs          # 进程执行服务 (64 行)
│   │   └── ConsoleService.cs      # 控制台服务 (111 行)
│   ├── Models/
│   │   ├── Config.cs              # 配置管理 (191 行)
│   │   ├── Software.cs            # 工具检测 (138 行)
│   │   ├── MenuTheme.cs           # 菜单主题 (75 行)
│   │   └── ReadLineModel.cs       # 输入模型 (49 行)
│   ├── UI/ConsoleUI/              # UI 组件
│   │   ├── HeaderView.cs
│   │   ├── HelpView.cs
│   │   ├── KeyboardShortcut.cs
│   │   ├── MenuBuilder.cs
│   │   ├── MenuView.cs
│   │   ├── NotificationView.cs
│   │   ├── ProgressView.cs
│   │   └── PromptView.cs
│   ├── Helpers/Console/
│   │   └── ConfirmationHelper.cs  # 确认助手 (59 行)
│   ├── Utilities/
│   │   └── ConsoleUtilities.cs    # 控制台工具 (189 行)
│   └── Program.cs                 # 入口点 (245 行)
├── EasyKit-Gui/                   # WPF GUI 应用
│   ├── Views/                     # 视图
│   └── Utilities/                 # 工具
├── ISS/                           # Inno Setup 安装程序
├── .github/workflows/             # CI/CD
└── README.md                      # 文档
```

### 2. 已实现功能

| 功能 | 状态 | 文件位置 | 行数 |
|------|------|----------|------|
| Git 操作 | ✅ 完整 | GitController.cs | 1069 |
| NPM 操作 | ✅ 基本 | NpmController.cs | 252 |
| pnpm 操作 | ✅ 基本 | PnpmController.cs | 385 |
| Composer 操作 | ✅ 基本 | ComposerController.cs | 238 |
| Laravel 操作 | ✅ 基本 | LaravelController.cs | 520 |
| 设置 | ✅ 基本 | SettingsController.cs | 191 |
| 工具市场 | ✅ 基本 | ToolMarketplaceController.cs | 108 |
| Windows 右键菜单 | ✅ 基本 | ISS/ContextMenu*.reg | 15 |
| CI/CD | ✅ 跨平台构建 | .github/workflows/ | 99 |
| 日志 | ✅ | Config.cs | - |

### 3. 已缺失功能

| 功能 | 状态 |
|------|------|
| uv (Python 包管理器) | ❌ 完全缺失 |
| 项目类型自动检测 | ❌ 完全缺失 |
| EasyKit Doctor | ❌ 完全缺失 |
| 测试覆盖 | ❌ 零测试 |
| 命令执行安全加固 | ❌ 存在注入风险 |
| 进程取消/超时 | ⚠️ 部分实现 |
| 动态菜单（按项目类型） | ❌ 静态菜单 |
| 历史记录 | ❌ 缺失 |
| 命令预览 | ❌ 缺失 |

---

## B. Findings - 按优先级排序

### P0 - Critical (安全/崩溃/数据丢失)

#### 1. Shell 注入漏洞 🔴 Critical

**位置**: 多个 Controller

**问题描述**: 用户输入直接拼接到命令字符串，未进行任何验证或转义。

**具体漏洞**:

```csharp
// GitController.cs:307 - 提交信息注入
_processService.RunProcess("git", $"commit -m \"{message}\"", ...);

// GitController.cs:368 - 分支名注入
_processService.RunProcess("git", $"checkout -b {branchName}", ...);

// GitController.cs:505 - stash 消息注入
var command = string.IsNullOrWhiteSpace(message) ? "stash" : $"stash push -m \"{message}\"";

// GitController.cs:677 - submodule URL 注入
string addCommand = url.StartsWith("--force") ? $"submodule add {url} {path}" : $"submodule add {url} {path}";

// GitController.cs:996 - submodule 路径注入
_processService.RunProcess("git", $"rm --cached {match}", ...);

// GitController.cs:1048 - commit 消息注入
_processService.RunProcess("git", $"commit -am \"Remove submodule {match}\"", ...);

// ComposerController.cs:109 - 包名注入
_processService.RunProcess("composer", $"require --dev {package}", ...);

// ComposerController.cs:122 - 项目创建注入
_processService.RunProcess("composer", $"create-project {package} {directory}", ...);
```

**修复方案**:
```csharp
// 使用 ProcessStartInfo.ArgumentList 代替字符串拼接
var psi = new ProcessStartInfo
{
    FileName = "git",
    Arguments = string.Empty,
    UseShellExecute = false,
    CreateNoWindow = true
};
psi.ArgumentList.Add("commit");
psi.ArgumentList.Add("-m");
psi.ArgumentList.Add(message); // 参数会自动转义
```

**风险评估**: 攻击者可以通过构造恶意分支名或提交信息执行任意命令。

---

#### 2. 无输入验证 🔴 High

**位置**: 所有 Controller

**问题描述**: 用户输入（分支名、包名、提交信息等）未进行任何验证。

**示例**:
```csharp
// 分支名可以是任意字符串，包括命令注入字符
var branchName = _prompt.Prompt("Enter new branch name: ") ?? "";
_processService.RunProcess("git", $"checkout -b {branchName}", ...);
```

**修复方案**:
- 添加输入验证正则表达式
- 对特殊字符进行转义
- 使用白名单验证

---

#### 3. PATH 劫持风险 🔴 Medium

**位置**: CmdService, Software.cs

**问题描述**: 应用程序依赖系统 PATH 查找可执行文件，未验证来源。

**风险**: 如果攻击者能够修改 PATH 或在 PATH 中插入恶意可执行文件，可以劫持命令执行。

**修复方案**:
- 验证可执行文件路径
- 使用完整路径执行命令
- 添加哈希校验

---

### P1 - High (主要功能缺失/UX/可靠性)

#### 1. 零测试覆盖 🔴 Critical

**问题**: 整个项目没有任何测试（单元测试、集成测试）。

**影响**: 
- 无法保证重构安全性
- 无法验证新功能
- CI/CD 无法阻止回归

**建议**: 创建测试项目，至少覆盖：
- 项目检测逻辑
- 工具检测逻辑
- 命令构造
- 错误处理

---

#### 2. uv (Python 包管理器) 支持完全缺失 🟡 High

**问题**: Python 开发工具链完全缺失。

**需求**:
- 检测 pyproject.toml, uv.lock, .python-version, .venv
- 支持 uv init, add, remove, sync, lock, run
- 支持 uv tool install/uninstall/list/upgrade
- 支持 uvx
- 支持 uv python install/list/pin
- 支持 uv venv
- 支持 pip 兼容命令

---

#### 3. 项目类型自动检测缺失 🟡 High

**问题**: EasyKit 不根据当前目录自动识别项目类型。

**影响**:
- 用户必须手动选择工具
- 无法显示项目特定的菜单
- 无法智能推荐命令

**建议实现**:
```csharp
public enum ProjectType
{
    None,
    Git,
    NodeJs,
    Pnpm,
    Python,
    Uv,
    Php,
    Composer,
    Laravel,
    DotNet,
    Docker
}

public class ProjectDetector
{
    public List<ProjectType> Detect(string directory) { ... }
}
```

---

#### 4. 重复代码 🟡 Medium

**问题**: 每个 Controller 复制相似的逻辑。

**重复模式**:
- `EnsureXxxInstalled()` 方法
- 菜单创建逻辑
- 诊断方法
- 错误处理

**建议**: 创建基类或共享服务。

---

#### 5. 无依赖注入 🟡 Medium

**问题**: 每个 Controller 自己 `new CmdService()`。

**影响**:
- 无法替换为 mock 进行测试
- 无法共享进程执行器实例

**建议**: 使用构造函数注入。

---

#### 6. 错误处理不统一 🟡 Medium

**问题**: 错误信息缺乏 actionable guidance。

**当前**:
```csharp
_console.WriteError("Command failed.");
```

**应该**:
```csharp
_console.WriteError($"Command '{command}' failed with exit code {exitCode}.");
_console.WriteError($"Working directory: {workingDir}");
_console.WriteError($"Tool '{toolName}' not found in PATH.");
_console.WriteInfo("To fix: Install {toolName} from https://...");
```

---

#### 7. 无进程取消/超时 🟡 Medium

**问题**: 长命令可能卡死 UI。

**当前**: 部分地方有 CancellationToken，但不统一。

**建议**: 统一添加超时机制（默认 5 分钟）。

---

### P2 - Medium (有价值的改进)

#### 1. 控制台输出重复 🟢 Low

**问题**: ConsoleUtilities 和 ConsoleService 功能重叠。

**建议**: 合并或明确分工。

---

#### 2. 硬编码字符串 🟢 Low

**问题**: 命令名、路径等硬编码。

**建议**: 移到配置或常量。

---

#### 3. Windows 特定假设 🟢 Low

**问题**: 大量 Windows API 调用。

**建议**: 抽象出平台特定代码。

---

#### 4. 无缓存 🟢 Low

**问题**: 每次启动重新检测工具。

**建议**: 缓存检测结果 5 分钟。

---

#### 5. 静态菜单 🟢 Low

**问题**: 菜单不根据项目类型动态调整。

**建议**: 实现动态菜单。

---

### P3 - Low (锦上添花)

#### 1. README 过时 🟢 Low

**问题**: 未提及 pnpm 支持细节。

---

#### 2. 无 API 文档 🟢 Low

**问题**: 公共方法缺少 XML 文档。

---

#### 3. 无历史记录 🟢 Low

**问题**: 不跟踪最近项目/命令。

---

## C. pnpm 实现方案

### 当前状态

- ✅ 已有 `PnpmController.cs`
- ✅ 基础菜单（install, update, build, dev, audit, cache）
- ⚠️ 功能有限，缺少：
  - `add`, `add -D`, `remove`
  - `exec`, `dlx`
  - `list`, `why`, `outdated`
  - workspace/monorepo 支持
  - Corepack 支持
  - npm/pnpm 冲突检测

### 需要实现的命令

```text
pnpm install
pnpm add <package>
pnpm add -D <package>
pnpm remove <package>
pnpm update
pnpm outdated
pnpm run <script>
pnpm exec <command>
pnpm dlx <package>
pnpm prune
pnpm store prune
pnpm audit
pnpm why <package>
pnpm list
pnpm version
pnpm config set <key> <value>
```

### workspace/monorepo 支持

```csharp
public bool IsPnpmWorkspace(string directory)
{
    return File.Exists(Path.Combine(directory, "pnpm-workspace.yaml"));
}

public List<string> GetWorkspacePackages(string directory)
{
    // Parse pnpm-workspace.yaml
    // Return list of workspace paths
}
```

### Corepack 支持

```csharp
public bool IsCorepackEnabled()
{
    var (output, error, exitCode) = _processService.RunProcess("corepack", "--version");
    return exitCode == 0;
}

public bool IsPackageMangerSet(string packageManager)
{
    // Check package.json for packageManager field
}
```

### npm/pnpm 冲突检测

```csharp
public bool HasPackageManagerConflict(string directory)
{
    if (!File.Exists(Path.Combine(directory, "package.json")))
        return false;
    
    var json = JsonDocument.Parse(File.ReadAllText(Path.Combine(directory, "package.json")));
    if (json.RootElement.TryGetProperty("packageManager", out var pm))
    {
        var pmValue = pm.GetString();
        if (pmValue.StartsWith("pnpm"))
            return File.Exists(Path.Combine(directory, "package-lock.json"));
    }
    return false;
}
```

---

## D. uv 实现方案

### 当前状态

- ❌ 完全缺失，需要从零开始

### 项目检测

```csharp
public class UvProjectDetector
{
    public bool IsUvProject(string directory)
    {
        return File.Exists(Path.Combine(directory, "pyproject.toml"))
            || File.Exists(Path.Combine(directory, "uv.lock"))
            || File.Exists(Path.Combine(directory, ".python-version"))
            || Directory.Exists(Path.Combine(directory, ".venv"))
            || File.Exists(Path.Combine(directory, "requirements.txt"));
    }
    
    public ProjectType GetProjectType(string directory)
    {
        if (IsUvProject(directory))
            return ProjectType.Uv;
        if (File.Exists(Path.Combine(directory, "pyproject.toml")))
            return ProjectType.Python;
        return ProjectType.None;
    }
}
```

### 支持的命令

```csharp
// Project management
uv init                    // Initialize new project
uv add <package>          // Add dependency
uv add -D <package>       // Add dev dependency
uv remove <package>       // Remove dependency
uv sync                   // Sync dependencies
uv lock                   // Generate lockfile
uv run <script>           // Run script
uv run python -m pytest   // Run Python command

// Tool management
uv tool install <tool>    // Install global tool
uv tool uninstall <tool>  // Uninstall global tool
uv tool list              // List installed tools
uv tool upgrade           // Upgrade all tools
uvx <tool>                // Run tool without installing

// Python management
uv python install <version>  // Install Python version
uv python list               // List installed versions
uv python pin <version>      // Pin Python version
uv venv                      // Create virtual environment

// pip compatibility
uv pip install <package>     // pip-compatible install
uv pip uninstall <package>   // pip-compatible uninstall
uv pip compile               // Compile requirements.txt
uv pip sync                  // Sync requirements.txt
uv tree                      // Show dependency tree
uv lock --check              // Check lockfile
```

### 工作流程区分

```csharp
public enum UvWorkflow
{
    ProjectDependencies,  // uv add/remove/sync/lock
    VirtualEnvironment,   // uv venv/python
    GlobalTools,          // uv tool install/list
    TemporaryRun,         // uvx/run
    PipCompatible         // uv pip install/uninstall
}
```

---

## E. 统一项目检测器

```csharp
public enum ProjectType
{
    None,
    Git,
    NodeJs,
    Pnpm,
    Python,
    Uv,
    Php,
    Composer,
    Laravel,
    DotNet,
    Docker
}

public class ProjectDetector
{
    private readonly Dictionary<string, List<ProjectType>> _cache = new();
    private readonly TimeSpan _cacheTimeout = TimeSpan.FromMinutes(5);
    
    public List<ProjectType> Detect(string directory)
    {
        var cached = GetCachedResult(directory);
        if (cached != null)
            return cached;
        
        var types = new List<ProjectType>();
        
        // Git
        if (Directory.Exists(Path.Combine(directory, ".git")))
            types.Add(ProjectType.Git);
        
        // Node.js / pnpm
        var packageJson = Path.Combine(directory, "package.json");
        if (File.Exists(packageJson))
        {
            types.Add(ProjectType.NodeJs);
            if (IsPnpmProject(directory))
                types.Add(ProjectType.Pnpm);
        }
        
        // Python / uv
        if (File.Exists(Path.Combine(directory, "pyproject.toml"))
            || File.Exists(Path.Combine(directory, "uv.lock"))
            || File.Exists(Path.Combine(directory, ".python-version"))
            || Directory.Exists(Path.Combine(directory, ".venv"))
            || File.Exists(Path.Combine(directory, "requirements.txt")))
        {
            types.Add(ProjectType.Python);
            if (File.Exists(Path.Combine(directory, "uv.lock")))
                types.Add(ProjectType.Uv);
        }
        
        // PHP / Composer / Laravel
        if (File.Exists(Path.Combine(directory, "composer.json")))
        {
            types.Add(ProjectType.Php);
            types.Add(ProjectType.Composer);
            if (File.Exists(Path.Combine(directory, "artisan")))
                types.Add(ProjectType.Laravel);
        }
        
        // .NET
        if (Directory.GetFiles(directory, "*.csproj").Any()
            || Directory.GetFiles(directory, "*.vbproj").Any())
            types.Add(ProjectType.DotNet);
        
        // Docker
        if (File.Exists(Path.Combine(directory, "Dockerfile"))
            || Directory.GetFiles(directory, "docker-compose*.yml").Any())
            types.Add(ProjectType.Docker);
        
        CacheResult(directory, types);
        return types;
    }
    
    private bool IsPnpmProject(string directory)
    {
        return File.Exists(Path.Combine(directory, "pnpm-lock.yaml"))
            || File.Exists(Path.Combine(directory, "pnpm-workspace.yaml"))
            || (File.Exists(Path.Combine(directory, "package.json"))
                && ReadPackagePackageManager(directory) == "pnpm");
    }
    
    private string? ReadPackagePackageManager(string dir)
    {
        var path = Path.Combine(dir, "package.json");
        if (!File.Exists(path)) return null;
        try
        {
            var json = JsonDocument.Parse(File.ReadAllText(path));
            if (json.RootElement.TryGetProperty("packageManager", out var pm))
                return pm.GetString();
        }
        catch { }
        return null;
    }
    
    private List<ProjectType>? GetCachedResult(string directory)
    {
        // Check cache with timeout
        return null;
    }
    
    private void CacheResult(string directory, List<ProjectType> types)
    {
        // Store cache with timestamp
    }
}
```

---

## F. 安全进程执行基础设施

### 当前问题

```csharp
// CmdService.cs - 委托给 CommonUtilities
public (string output, string error, int exitCode) RunProcess(string command, string args, string? workingDirectory = null)
{
    return CmdHelper.RunProcessWithCmd(command, args, workingDirectory);
}
```

### 需要实现

```csharp
public class SecureProcessRunner
{
    private readonly TimeSpan _defaultTimeout = TimeSpan.FromMinutes(5);
    private readonly HashSet<string> _allowedCommands = new(StringComparer.OrdinalIgnoreCase)
    {
        "git", "npm", "pnpm", "composer", "php", "laravel",
        "uv", "python", "pip", "dotnet", "docker"
    };
    
    public async Task<ProcessResult> RunAsync(
        string command,
        IEnumerable<string> arguments,
        string? workingDirectory = null,
        CancellationToken cancellationToken = default,
        TimeSpan? timeout = null)
    {
        // Validate command
        ValidateCommand(command);
        
        // Validate arguments (no shell metacharacters)
        ValidateArguments(arguments);
        
        var psi = new ProcessStartInfo
        {
            FileName = command,
            Arguments = string.Join(" ", arguments),
            WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        
        using var process = Process.Start(psi);
        if (process == null)
            throw new InvalidOperationException($"Failed to start process: {command}");
        
        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();
        
        var completed = await Task.WhenAny(
            process.WaitForExitAsync(cancellationToken),
            Task.Delay(timeout ?? _defaultTimeout)
        );
        
        if (completed != process.WaitForExitAsync(cancellationToken))
        {
            process.Kill();
            return new ProcessResult(null, null, -1, true);
        }
        
        await Task.WhenAll(outputTask, errorTask);
        
        return new ProcessResult(
            outputTask.Result,
            errorTask.Result,
            process.ExitCode,
            false
        );
    }
    
    private void ValidateCommand(string command)
    {
        // Only allow known safe commands
        if (!_allowedCommands.Contains(Path.GetFileNameWithoutExtension(command)))
            throw new SecurityException($"Command '{command}' is not allowed");
        
        // Check for shell metacharacters in command path
        if (command.Any(c => "`|;&$(){}[]!#%'^<>".Contains(c)))
            throw new SecurityException($"Invalid characters in command: {command}");
    }
    
    private void ValidateArguments(IEnumerable<string> arguments)
    {
        foreach (var arg in arguments)
        {
            // Reject shell metacharacters
            if (arg.Any(c => "`|;&$(){}[]!#%'^<>".Contains(c)))
                throw new SecurityException($"Invalid character in argument: {arg}");
            
            // Reject path traversal
            if (arg.Contains("..") || arg.StartsWith("/"))
                throw new SecurityException($"Suspicious argument: {arg}");
        }
    }
}

public record ProcessResult(string? Output, string? Error, int ExitCode, bool TimedOut);
```

---

## G. 实施计划

### 阶段 1: 安全修复 (立即实施)

1. **修复 Shell 注入漏洞**
   - [ ] 修改所有 Controller 使用 `ArgumentList`
   - [ ] 添加输入验证
   - [ ] 添加命令白名单

2. **增强进程执行安全**
   - [ ] 实现 `SecureProcessRunner`
   - [ ] 添加 PATH 验证
   - [ ] 实现进程树终止

### 阶段 2: 基础设施 (P1)

1. **重构 CmdService**
   - [ ] 添加 CancellationToken 支持
   - [ ] 添加超时机制
   - [ ] 实现流式输出

2. **统一错误处理**
   - [ ] 创建错误响应模型
   - [ ] 添加诊断信息
   - [ ] 提供修复建议

### 阶段 3: 检测系统 (P1)

1. **实现 ProjectDetector**
   - [ ] 检测所有项目类型
   - [ ] 实现缓存
   - [ ] 集成到 UI

2. **实现 ToolDetector**
   - [ ] 检测所有工具
   - [ ] 版本检查
   - [ ] 状态分类 (Installed/Not Installed/Outdated/Broken)

### 阶段 4: pnpm 完善 (P1)

1. **扩展 PnpmController**
   - [ ] 添加缺失命令
   - [ ] 实现 workspace 支持
   - [ ] 添加 Corepack 支持
   - [ ] 冲突检测

### 阶段 5: uv 实现 (P1)

1. **创建 UvController**
   - [ ] 所有列出的命令
   - [ ] 工作流程区分
   - [ ] 项目检测

2. **创建 UvProjectDetector**
   - [ ] 检测 uv 项目
   - [ ] 检测 Python 项目

### 阶段 6: 包管理器抽象 (P2)

1. **创建 PackageManager 接口**
   - [ ] 定义统一接口
   - [ ] 实现共享逻辑
   - [ ] 各 Manager 继承

### 阶段 7: EasyKit Doctor (P2)

1. **创建 DoctorController**
   - [ ] 检查所有工具
   - [ ] 显示状态和版本
   - [ ] 提供修复建议

### 阶段 8: UX 改进 (P2)

1. **动态菜单**
   - [ ] 根据项目类型显示
   - [ ] 面包屑导航
   - [ ] 命令预览

2. **历史记录**
   - [ ] 最近项目
   - [ ] 最近命令

### 阶段 9: 测试 (P1)

1. **单元测试**
   - [ ] 项目检测
   - [ ] 工具检测
   - [ ] 命令构造
   - [ ] 错误解析

2. **集成测试**
   - [ ] 使用 mock 进程执行器
   - [ ] 避免依赖外部工具

### 阶段 10: 文档 (P3)

1. **更新 README**
2. **添加 pnpm 文档**
3. **添加 uv 文档**
4. **添加贡献指南**

---

## H. 结论

EasyKit 是一个功能基本的开发工具箱，但在安全性、测试覆盖和现代化功能方面存在显著不足。建议优先修复安全漏洞，然后逐步添加 pnpm 增强、uv 支持和项目检测功能。

### 关键行动项

1. **立即**: 修复 Shell 注入漏洞
2. **短期**: 添加测试框架和基础测试
3. **中期**: 实现 uv 支持和项目检测
4. **长期**: 重构架构，实现包管理器抽象

---

*报告生成时间: 2025*
*审计范围: EasyKit v4.2.1*
