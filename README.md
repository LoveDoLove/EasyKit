# EasyKit 3.0.0

**Your All-in-One Python Toolkit for Streamlined Cross-Platform Web Development**

EasyKit is a modern, cross-platform Python utility designed to simplify and accelerate common web development tasks. It provides a unified interface to manage various tools and project configurations, boosting your productivity and ensuring consistency across your development workflow.

## Overview

EasyKit 3.0.0 represents a complete transformation to a pure Python implementation, focusing on cross-platform compatibility, enhanced performance, and a modern command-line experience. Whether you're managing dependencies, version control, or project-specific commands, EasyKit provides a streamlined Python-based interface to get you up and running quickly.

## Features

*   **Cross-Platform Support:** Works seamlessly on Windows, macOS, and Linux.
*   **Modern CLI Interface:** Rich text interface with colors, progress bars, and interactive prompts.
*   **Python-Based Tools:** Native Python implementations of Git, Composer, NPM, and Laravel Artisan integrations.
*   **Smart Configuration:** Automatic platform-specific configuration management using platformdirs.
*   **Automated Updates:** Built-in update system with version checking and automatic downloads.
*   **Enhanced Security:** Improved security with proper permission handling and validation.
*   **Extensive Testing:** Comprehensive test suite ensuring reliability across platforms.

## Additional Features

- **User-Specific Configurations:** Customize settings with JSON files for personalized behavior.
- **Enhanced Logging:** Logs stored in platform-specific directories for better organization.

## Getting Started

### Prerequisites

- Python 3.8 or higher
- pip (Python package installer)

### Installation

1. Install from PyPI (recommended):
   ```bash
   pip install easykit
   ```

2. Or install from source:
   ```bash
   git clone https://github.com/YourUsername/EasyKit.git
   cd EasyKit
   pip install -e . 
   ```

### Creating a Virtual Environment, Installing Dependencies, and Compiling

It is recommended to use a virtual environment to avoid dependency conflicts and ensure a clean build.

1. **Create a virtual environment**
   ```cmd
   python -m venv venv
   ```

2. **Activate the virtual environment**
   ```cmd
   venv\Scripts\activate
   ```

3. **Install dependencies**
   ```cmd
   pip install --upgrade pip
   pip install -r requirements.txt
   pip install pyinstaller
   ```

4. **(Optional) Clean previous build artifacts**
   ```cmd
   rmdir /s /q build
   rmdir /s /q dist
   del run_easykit.spec
   ```

5. **Compile to an executable with PyInstaller**
   - For console applications (recommended if your tool uses input):
     ```cmd
     pyinstaller --onefile --icon=images/icon.ico run_easykit.py
     ```
   - For GUI applications (no console window, input() will NOT work):
     ```cmd
     pyinstaller --onefile --windowed --icon=images/icon.ico run_easykit.py
     ```

> **Note:** If your tool uses `input()` or interactive prompts, do NOT use the `--windowed` flag, as it will cause errors like `RuntimeError: input(): lost sys.stdin`.

### Usage

Run EasyKit from the command line:
```bash
easykit
```

This will start the interactive interface where you can:
- Manage development tools and dependencies
- Configure your development environment
- Create and manage projects
- Update EasyKit and its components

## Configuration

EasyKit's behavior can be customized through its configuration system:

*   **Global Configuration:** Located in the platform-specific config directory
    - Windows: `%APPDATA%\EasyKit\config\settings.json`
    - macOS: `~/Library/Application Support/EasyKit/config/settings.json`
    - Linux: `~/.config/EasyKit/config/settings.json`
*   **User Configuration:** For personal settings and overrides
    - Windows: `%LOCALAPPDATA%\EasyKit\config\user_settings.json`
    - macOS: `~/Library/Application Support/EasyKit/config/user_settings.json`
    - Linux: `~/.local/share/EasyKit/config/user_settings.json`

## Logging

EasyKit maintains logs in platform-specific locations:
- Windows: `%LOCALAPPDATA%\EasyKit\logs\easykit.log`
- macOS: `~/Library/Logs/EasyKit/easykit.log`
- Linux: `~/.local/state/EasyKit/logs/EasyKit.log`

## Removed Features

- Batch file dependencies: All functionality has been migrated to Python.
- Legacy configuration methods: Replaced with JSON-based user-specific configurations.

### Updates

- Removed references to batch files and older configuration methods in the documentation.

## Development and Testing

### Setting Up Development Environment

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/EasyKit.git
   cd EasyKit
   ```

2. Create and activate a virtual environment:
   ```bash
   python -m venv venv
   # On Windows:
   venv\Scripts\activate
   # On Unix/Linux:
   source venv/bin/activate
   ```

3. Install dependencies:
   ```bash
   pip install -r requirements.txt
   ```

### Running Tests

The project no longer uses pytest for testing. Please refer to the updated testing documentation for alternative methods.

### Code Coverage

The code coverage section has been removed as pytest-cov is no longer used.

## Contributing

1. Fork the repository
2. Create a new branch for your feature (`git checkout -b feature/amazing-feature`)
3. Make your changes
4. Run the tests to ensure nothing is broken
5. Commit your changes (`git commit -m 'Add some amazing feature'`)
6. Push to your branch (`git push origin feature/amazing-feature`)
7. Open a Pull Request

## License

EasyKit is released under the [MIT License](LICENSE).

---

Thank you for using EasyKit! We hope it makes your development process smoother and more efficient.
