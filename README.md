# EasyKit 3.1.9

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
- [Miniconda (recommended for environment management)](https://docs.conda.io/en/latest/miniconda.html)
- Git (for source install)

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

---

## Setup & Build (with Miniconda)

### 1. Download and Install Miniconda
- Download Miniconda for Windows from: https://docs.conda.io/en/latest/miniconda.html
- Run the installer and follow the prompts (choose "Add Miniconda to my PATH" if you want to use `conda` from any terminal).

### 2. Create and Activate a Conda Environment
```cmd
conda create -n easykit python=3.11
conda activate easykit
```

### 3. Install Build Dependencies in the Conda Environment
```cmd
pip install --upgrade pip
pip install -r requirements.txt
pip install nuitka
```

### 4. (Optional) Clean Previous Build Artifacts
```cmd
rmdir /s /q build
rmdir /s /q dist
del run_easykit.build
del run_easykit.dist
```

### 5. Build the Executable with Nuitka (Windows standalone mode)
```cmd
python -m nuitka --standalone --windows-icon-from-ico=images/icon.ico --output-dir=dist --output-filename=EasyKit_v3.1.9.exe --include-data-dir=docs=docs --include-data-dir=images=images --include-data-dir=windows=windows run_easykit.py
```
> In Windows cmd.exe, run this as a single line (no backslashes). Replace `3.1.9` with your version/tag. Nuitka will create all files (EXE, DLLs, etc.) directly in the `dist` folder. **You must include all files from `dist` in your installer, not just the EXE.**

**Important:**
- Do **not** use `--onefile`! Only use `--standalone`.
- If you still see your app extracting to a temp folder at runtime, you are either running a onefile build or not running from the installed folder.
- Always run the EXE from the install directory (where all DLLs and files are present), not by copying the EXE elsewhere.
- If you see missing DLL errors, double-check that all files from `dist` are included in your installer and present after install.

**Note:** Nuitka produces a true native executable. No .spec file is needed.

### 6. Build a Windows Installer (Setup EXE)

#### a. Install Inno Setup (if not already installed)
```cmd
choco install innosetup -y
```

#### b. Build the Installer
```cmd
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" EasyKitInstaller.iss
```
This will generate `EasyKitSetup_3.1.9.exe` in your project directory.

#### c. Distribute the Installer
Share `EasyKitSetup_3.1.9.exe` with users. They can double-click to install EasyKit on their Windows system, including all required files and shortcuts.

---

## Usage

After installation, run EasyKit from the Start Menu or desktop shortcut, or from the command line:
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
2. Create and activate a conda environment:
   ```cmd
   conda create -n easykit python=3.11
   conda activate easykit
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
