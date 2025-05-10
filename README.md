# EasyKit 3.0.0

**Your All-in-One Python Toolkit for Streamlined Windows Web Development**

EasyKit is a modern Python utility designed to simplify and accelerate web development tasks on Windows. It provides a unified interface to manage tools and project configurations, boosting productivity and ensuring consistency across your workflow.

## Features

- **Windows Support:** Designed for Windows environments
- **Modern CLI:** Rich text, colors, progress bars, and interactive prompts
- **Python-Based Integrations:** Git, Composer, NPM, Laravel Artisan
- **Smart Config:** Platform-specific configuration with platformdirs
- **Auto Updates:** Built-in version checking and update system
- **Enhanced Security:** Proper permission handling and validation
- **User Configs:** JSON-based, user-specific settings
- **Organized Logging:** Log directories for Windows
- **Comprehensive Testing:** Reliable for Windows

## Getting Started

### Prerequisites
- Windows 10 or higher
- Python 3.8 or higher
- pip (Python package installer)

### Installation

Install from PyPI (recommended):
```cmd
pip install easykit
```

Or install from source:
```cmd
git clone https://github.com/YourUsername/EasyKit.git
cd EasyKit
pip install -e .
```

### Setup & Build

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
   pip install nuitka
   ```
4. **(Optional) Clean previous build artifacts**
   ```cmd
   rmdir /s /q build
   rmdir /s /q dist
   del run_easykit.build
   del run_easykit.dist
   ```
5. **Build the executable (console app) with Nuitka:**
   ```cmd
   python -m nuitka --onefile --windows-icon-from-ico=images/icon.ico --output-filename=EasyKit_v3.0.0.exe \
   --include-data-dir=docs=docs \
   --include-data-dir=images=images \
   --include-data-dir=windows=windows \
   run_easykit.py
   ```
   > Replace `3.0.0` with your version/tag. Output: `EasyKit_v3.0.0.exe`

> **Note:** Nuitka produces a true native executable. You do not need a .spec file. If you use interactive prompts, no special flags are needed.

### Usage

Run EasyKit from the command line:
```cmd
easykit
```

Features include:
- Manage development tools and dependencies
- Configure your environment
- Create/manage projects
- Update EasyKit and its components

## Configuration

EasyKit uses Windows config directories:
- **Global:** `%APPDATA%\EasyKit\config\settings.json`
- **User:** `%LOCALAPPDATA%\EasyKit\config\user_settings.json`

## Logging

Logs are stored in Windows locations:
- `%LOCALAPPDATA%\EasyKit\logs\easykit.log`

## Removed Features
- Batch file dependencies (all Python now)
- Legacy config methods (now JSON-based)

## Development & Testing

1. Clone the repository:
   ```cmd
   git clone https://github.com/yourusername/EasyKit.git
   cd EasyKit
   ```
2. Create and activate a virtual environment:
   ```cmd
   python -m venv venv
   venv\Scripts\activate
   ```
3. Install dependencies:
   ```cmd
   pip install -r requirements.txt
   ```

### Running Tests

Pytest is no longer used. See updated docs for testing methods.

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Make your changes
4. Run tests
5. Commit (`git commit -m 'Add some amazing feature'`)
6. Push (`git push origin feature/amazing-feature`)
7. Open a Pull Request

## License

EasyKit is released under the [MIT License](LICENSE).

---

Thank you for using EasyKit! We hope it makes your Windows development process smoother and more efficient.
