<!-- PROJECT SHIELDS -->
[![Contributors](https://img.shields.io/github/contributors/LoveDoLove/EasyKit.svg?style=for-the-badge)](https://github.com/LoveDoLove/EasyKit/graphs/contributors)
[![Forks](https://img.shields.io/github/forks/LoveDoLove/EasyKit.svg?style=for-the-badge)](https://github.com/LoveDoLove/EasyKit/network/members)
[![Stargazers](https://img.shields.io/github/stars/LoveDoLove/EasyKit.svg?style=for-the-badge)](https://github.com/LoveDoLove/EasyKit/stargazers)
[![Issues](https://img.shields.io/github/issues/LoveDoLove/EasyKit.svg?style=for-the-badge)](https://github.com/LoveDoLove/EasyKit/issues)
[![MIT License](https://img.shields.io/github/license/LoveDoLove/EasyKit.svg?style=for-the-badge)](LICENSE)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-Profile-blue?style=for-the-badge&logo=linkedin)](https://www.linkedin.com/in/lovedolove)

<a id="readme-top"></a>

<!-- PROJECT LOGO -->
<br />
<div align="center">
  <h3 align="center">EasyKit</h3>
  <p align="center">A C#/.NET 8.0 Console Toolkit for Modern Development Workflows</p>
</div>

> **Note:** This project is supported by [ZMTO](https://www.zmto.com) as part of their open-source VPS program. Special thanks to ZMTO for empowering open-source innovation!

<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#about-the-project">About The Project</a></li>
    <li><a href="#built-with">Built With</a></li>
    <li><a href="#features">Features</a></li>
    <li><a href="#project-structure">Project Structure</a></li>
    <li><a href="#getting-started">Getting Started</a></li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>

## About The Project

EasyKit is a powerful C# console-based toolkit designed to streamline modern development workflows. It provides a suite of tools for managing various aspects of web and software development projects—including NPM, Laravel, Composer, and Git operations—through an intuitive menu interface.

## Built With

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js & npm](https://nodejs.org/)
- [PHP & Composer](https://getcomposer.org/)
- [Git](https://git-scm.com/)
- [Serilog](https://serilog.net/)
- [MailKit](https://github.com/jstedfast/MailKit)
- [QRCoder](https://github.com/codebude/QRCoder)
- [FluentValidation](https://fluentvalidation.net/)
- [EntityFrameworkCore](https://docs.microsoft.com/ef/core/)
- [Stripe](https://stripe.com/docs/api/dotnet)
- [MediateR](https://github.com/jbogard/MediatR)

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Features

- Easy-to-use console menu system with customizable themes
- Tools for managing NPM packages and operations
- Laravel project tools and utilities
- Composer package management
- Git workflow integration
- Customizable settings and configuration
- Built-in logging and console management
- Modular architecture: Controllers, Services, Models, and Views
- Extensible: add your own tools and menu items
- Planned: Docker integration, VS Code extensions management, project templates, and multi-language support

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Project Structure

```
EasyKit.sln
LICENSE
README.md
EasyKit/
  Controllers/         # Tool controllers (Npm, Laravel, Composer, Git, etc.)
  Models/              # Data models (Config, MenuTheme, Software, etc.)
  Services/            # Core services (Console, Logger, Process, etc.)
  www/images/          # App icons and images
  Program.cs           # Application entry point
CS_CommonUtilities/    # Shared utility library (see its README for details)
...
```

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or newer
- [Node.js and npm](https://nodejs.org/) for NPM features
- [PHP and Composer](https://getcomposer.org/) for Laravel/Composer features
- [Git](https://git-scm.com/) for Git integration

### Installation

1. Clone the repository
   ```sh
   git clone https://github.com/LoveDoLove/EasyKit.git
   ```
2. Navigate to the project directory
   ```sh
   cd EasyKit
   ```
3. Build the application
   ```sh
   dotnet build -c Release
   ```
4. Run the application
   ```sh
   dotnet run --project EasyKit/EasyKit.csproj
   ```

Alternatively, download the latest release from the [releases page](https://github.com/LoveDoLove/EasyKit/releases).

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Usage

EasyKit provides a menu-driven interface for various development tools:

- **NPM Tools**: Install/update npm packages, run npm scripts, manage package.json, view project info
- **Laravel Tools**: Create/manage Laravel projects, run artisan commands, handle migrations
- **Composer Tools**: Install/update Composer packages, manage composer.json, run Composer scripts
- **Git Integration**: Initialize repositories, manage branches, commit, push/pull
- **Settings**: Customize menu appearance, configure tool preferences, set up default paths, toggle themes

Start EasyKit and follow the on-screen menu to access these features.

### Example: Running a Tool

```sh
dotnet run --project EasyKit/EasyKit.csproj
# Use arrow keys to navigate the menu and select a tool (e.g., NPM, Git, Laravel)
```

_See more examples and advanced usage in the [Documentation](https://github.com/LoveDoLove/EasyKit/wiki)_

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Roadmap

- [x] Core menu system with customizable themes
- [x] NPM integration
- [x] Laravel tools
- [x] Composer integration
- [x] Git integration
- [x] Settings management
- [ ] Docker integration
- [ ] VS Code extensions management
- [ ] Project templates and scaffolding
- [ ] Multi-language support (Spanish, Chinese)

See the [open issues](https://github.com/LoveDoLove/EasyKit/issues) for a full list of proposed features and known issues.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".
Don't forget to give the project a star! Thanks again!

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

_Refer to [README_TEMPLATES.md](README_TEMPLATES.md) for style and structure guidelines._

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## License

Distributed under the MIT License. See `LICENSE` for more information.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Contact

LoveDoLove - [GitHub Profile](https://github.com/LoveDoLove)

Project Link: [https://github.com/LoveDoLove/EasyKit](https://github.com/LoveDoLove/EasyKit)

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Acknowledgments

- [Best-README-Template](https://github.com/othneildrew/Best-README-Template)
- [Shields.io](https://shields.io)
- [Choose an Open Source License](https://choosealicense.com)
- [.NET Documentation](https://docs.microsoft.com/dotnet/)
