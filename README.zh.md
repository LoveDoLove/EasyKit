<!-- Improved compatibility of back to top link: See: https://github.com/othneildrew/Best-README-Template/pull/73 -->

<a id="readme-top"></a>

<!-- PROJECT SHIELDS -->

[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]

<!-- PROJECT LOGO -->
<br />
<div align="center">
  <img src="images/icon.jpg" alt="EasyKit Logo" width="80" height="80">
  <h3 align="center">EasyKit</h3>
  <p align="center">
    一款面向 Web 开发者的 .NET 工具包，提供统一的控制台界面管理 Git、NPM、Composer、Laravel 和 Python 工具
    <br />
    <a href="https://github.com/LoveDoLove/EasyKit"><strong>探索文档 »</strong></a>
    <br />
    <br />
    <a href="https://github.com/LoveDoLove/EasyKit/releases">下载最新版</a>
    &middot;
    <a href="https://github.com/LoveDoLove/EasyKit/issues/new?labels=bug&template=bug-report---.md">报告 Bug</a>
    &middot;
    <a href="https://github.com/LoveDoLove/EasyKit/issues/new?labels=enhancement&template=feature-request---.md">请求功能</a>
  </p>
</div>

<details>
  <summary>目录</summary>
  <ol>
    <li><a href="#关于项目">关于项目</a>
      <ul>
        <li><a href="#功能特性">功能特性</a></li>
        <li><a href="#技术栈">技术栈</a></li>
      </ul>
    </li>
    <li><a href="#快速开始">快速开始</a>
      <ul>
        <li><a href="#前置要求">前置要求</a></li>
        <li><a href="#安装">安装</a></li>
      </ul>
    </li>
    <li><a href="#使用方法">使用方法</a></li>
    <li><a href="#路线图">路线图</a></li>
    <li><a href="#贡献指南">贡献指南</a></li>
    <li><a href="#许可证">许可证</a></li>
    <li><a href="#联系方式">联系方式</a></li>
    <li><a href="#致谢">致谢</a></li>
  </ol>
</details>

<!-- 关于项目 -->

## 关于项目

EasyKit 是一款专为 Web 开发者设计的 Windows 工具包。它提供了一个统一的控制台界面，集成了多种开发工具，包括 Git、NPM、Composer、Laravel Artisan 和 uv（Python），使在 Windows 系统上管理 Web 开发工作流更加轻松。

主要功能包括：

- **统一界面**：单一控制台应用管理所有开发工具
- **Git 集成**：完整的 Git 工作流管理，支持安全命令执行
- **NPM 支持**：Node.js 包管理
- **pnpm 支持**：一流的 pnpm 支持，带工作区/单仓库检测
- **uv 支持**：Python 包/项目/工具管理
- **Composer 集成**：PHP 依赖管理
- **Laravel 工具**：Artisan 命令执行
- **工具市场**：检测和安装缺失的开发工具
- **EasyKit Doctor**：全面的环境诊断工具
- **项目检测**：自动检测项目类型（Git、Node.js、pnpm、Python、uv、PHP、.NET、Docker）
- **Windows 优化**：专为 Windows 10/11 设计
- **现代 .NET**：基于 .NET 8.0 构建，性能可靠
- **安全性**：安全的进程执行，防止 Shell 注入
- **右键菜单支持**：从 Windows 资源管理器快速访问
- **彩色输出**：增强的控制台体验和通知
- **全面测试**：48+ 单元测试覆盖安全性和功能

<p align="right">(<a href="#readme-top">返回顶部</a>)</p>

### 技术栈

- [![.NET][.NET-badge]][.NET-url] - .NET 8.0 框架
- [![C#][C#-badge]][C#-url] - 主要编程语言
- [![Windows][Windows-badge]][Windows-url] - 目标平台
- [CommonUtilities](https://github.com/LoveDoLove/CS_CommonUtilities) - 共享工具库

<p align="right">(<a href="#readme-top">返回顶部</a>)</p>

<!-- 快速开始 -->

## 快速开始

EasyKit 专为 Windows 系统设计，提供简单的 Web 开发工具入门方式。

### 前置要求

运行 EasyKit 前，请确保已安装以下内容：

- **Windows 10 或 11**
- **.NET 8.0 Runtime** - 从 [Microsoft .NET](https://dotnet.microsoft.com/download/dotnet/8.0) 下载
- **可选开发工具**：
  - Git（用于 Git 操作）
  - Node.js 和 NPM（用于 JavaScript/TypeScript 项目）
  - PHP 和 Composer（用于 PHP/Laravel 项目）

### 安装

#### 方式一：下载预构建版本（推荐）

1. 访问 [Releases](https://github.com/LoveDoLove/EasyKit/releases) 页面
2. 根据您的系统下载最新的 `EasyKit-x.x.x-x64.exe` 或 `EasyKit-x.x.x-x86.exe`
3. 运行安装程序并按照安装向导操作

#### 方式二：从源代码构建

1. 克隆仓库：
   ```cmd
   git clone https://github.com/LoveDoLove/EasyKit.git
   ```
2. 在 Visual Studio 2022+ 或 JetBrains Rider 中打开 `EasyKit.sln`
3. 构建解决方案（Release 或 Debug）
4. 运行 EasyKit 项目（建议使用管理员权限）

<p align="right">(<a href="#readme-top">返回顶部</a>)</p>

<!-- 使用方法 -->

## 使用方法

安装后，从开始菜单运行 EasyKit 或执行已安装的应用程序。控制台界面将为您提供可用工具菜单：

| 按键 | 功能 | 描述 |
|-----|---------|-------------|
| 0 | 退出 | 退出应用程序 |
| 1 | Git 工具 | 完整的 Git 工作流管理 |
| 2 | NPM 工具 | Node.js 包管理 |
| 3 | pnpm 工具 | 带工作区支持的 pnpm 包管理 |
| 4 | Composer 工具 | PHP 依赖管理 |
| 5 | Laravel 工具 | Laravel Artisan 命令执行 |
| 6 | uv 工具 | Python 包/项目/工具管理 |
| 7 | 设置 | 应用程序设置 |
| 8 | 诊断 | EasyKit Doctor - 环境诊断 |
| T | 工具市场 | 检测和管理开发工具 |
| Q | 退出 | 退出应用程序 |

### 项目检测

EasyKit 会根据以下文件自动检测您的项目类型：

- `.git/` → Git 仓库
- `package.json` → Node.js 项目
- `pnpm-lock.yaml` / `pnpm-workspace.yaml` → pnpm 项目
- `pyproject.toml` / `uv.lock` → Python/uv 项目
- `composer.json` → PHP/Composer 项目
- `artisan` → Laravel 项目
- `*.csproj` → .NET 项目
- `Dockerfile` / `docker-compose*.yml` → Docker 项目

当前项目类型将显示在主菜单中。

使用键盘导航菜单，访问 EasyKit 集成的各种开发工具。

更多使用说明，请参阅应用内帮助或源代码注释。

<p align="right">(<a href="#readme-top">返回顶部</a>)</p>

## 路线图

EasyKit 自诞生以来已经经历了显著的发展：

- **v3.x 系列**：纯 Python 实现，专注于 Windows
- **v4.0.6**：重大过渡到 .NET 框架
- **v4.1.x 系列**：增强的 .NET 实现，性能提升
- **当前 v4.2.3**：最新稳定版本，包含安全修复、pnpm/uv 支持、Doctor 诊断和项目检测

### 最新版本新增功能 (v4.2.3)

- ✅ GitHub Actions CI/CD 工作流实现自动化发布
- ✅ Inno Setup 安装程序打包（x64/x86）
- ✅ pnpm 一流支持，带工作区检测
- ✅ uv Python 包管理器支持
- ✅ EasyKit Doctor 诊断工具
- ✅ 自动项目类型检测
- ✅ 安全的进程执行（防止 Shell 注入）
- ✅ 全面的单元测试（48+ 测试）

### 未来计划

- [ ] 改进 UI/UX，添加项目感知菜单
- [ ] 添加更多 Git 功能
- [ ] 添加 Docker 支持
- [ ] 添加集成测试
- [ ] 跨平台支持

查看 [open issues](https://github.com/LoveDoLove/EasyKit/issues) 获取完整的功能提案和问题列表。

<p align="right">(<a href="#readme-top">返回顶部</a>)</p>

<!-- 贡献指南 -->

## 贡献指南

贡献是让开源社区变得如此精彩的原因。您所做的任何贡献都**非常感谢**。

要参与贡献：

1. Fork 项目
2. 创建您的特性分支（`git checkout -b feature/AmazingFeature`）
3. 提交您的更改（`git commit -m 'Add some AmazingFeature'`）
4. 推送到分支（`git push origin feature/AmazingFeature`）
5. 打开 Pull Request

请遵循 .NET Foundation 行为准则和最佳实践。

<p align="right">(<a href="#readme-top">返回顶部</a>)</p>

<!-- 许可证 -->

## 许可证

根据 MIT 许可证分发。有关更多信息，请参阅 `LICENSE` 文件。

<p align="right">(<a href="#readme-top">返回顶部</a>)</p>

<!-- 联系方式 -->

## 联系方式

LoveDoLove - [GitHub](https://github.com/LoveDoLove)

项目链接：[https://github.com/LoveDoLove/EasyKit](https://github.com/LoveDoLove/EasyKit)

<p align="right">(<a href="#readme-top">返回顶部</a>)</p>

## 赞助支持

本项目由 [ZMTO](https://www.zmto.com) 作为其开源 VPS 计划的一部分 Proudly 支持。我们向 ZMTO 表示诚挚的感谢，感谢他们宝贵的资源和对推动开源创新的承诺。

<p align="right">(<a href="#readme-top">返回顶部</a>)</p>

<!-- 致谢 -->

## 致谢

- [Best-README-Template](https://github.com/othneildrew/Best-README-Template)
- [Choose an Open Source License](https://choosealicense.com)
- [.NET Foundation](https://dotnetfoundation.org/)
- [CommonUtilities](https://github.com/LoveDoLove/CS_CommonUtilities)

<p align="right">(<a href="#readme-top">返回顶部</a>)</p>

<!-- MARKDOWN LINKS & IMAGES -->

[contributors-shield]: https://img.shields.io/github/contributors/LoveDoLove/EasyKit.svg?style=for-the-badge
[contributors-url]: https://github.com/LoveDoLove/EasyKit/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/LoveDoLove/EasyKit.svg?style=for-the-badge
[forks-url]: https://github.com/LoveDoLove/EasyKit/network/members
[stars-shield]: https://img.shields.io/github/stars/LoveDoLove/EasyKit.svg?style=for-the-badge
[stars-url]: https://github.com/LoveDoLove/EasyKit/stargazers
[issues-shield]: https://img.shields.io/github/issues/LoveDoLove/EasyKit.svg?style=for-the-badge
[issues-url]: https://github.com/LoveDoLove/EasyKit/issues
[license-shield]: https://img.shields.io/github/license/LoveDoLove/EasyKit.svg?style=for-the-badge
[license-url]: https://github.com/LoveDoLove/EasyKit/blob/main/LICENSE
[.NET-badge]: https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white
[.NET-url]: https://dotnet.microsoft.com/
[C#-badge]: https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white
[C#-url]: https://docs.microsoft.com/en-us/dotnet/csharp/
[Windows-badge]: https://img.shields.io/badge/Windows-0078D4?style=for-the-badge&logo=windows&logoColor=white
[Windows-url]: https://www.microsoft.com/windows
