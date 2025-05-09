# EasyKit 2.0.0

**Your All-in-One Toolkit for Streamlined Web Development**

EasyKit is a comprehensive command-line utility designed to simplify and accelerate common web development tasks. It provides a unified interface to manage various tools and project configurations, boosting your productivity and ensuring consistency across your development workflow.

## Overview

EasyKit 2.0.0 marks a significant update, focusing on enhanced performance, broader tool integration, and a more intuitive user experience. Whether you're managing dependencies, version control, or project-specific commands, EasyKit provides the necessary scripts and configurations to get you up and running quickly.

## Features

*   **Simplified Tool Access:** Quickly access and run tools like Git, Composer, NPM, and Laravel Artisan commands through dedicated EasyKit scripts.
*   **Centralized Configuration:** Manage global and user-specific settings via `config/config_eskit.bat` and `config/user_config_eskit.bat`.
*   **Automated Setup:** Easy installation process using `scripts/core/install_eskit.bat`.
*   **Update Management:** Keep EasyKit up-to-date with `scripts/core/update_eskit.bat`.
*   **Project Initialization:** Streamlined project setup with `run_eskit.bat`.
*   **Customizable:** Extend EasyKit with your own scripts and configurations.

## Getting Started

### Prerequisites

Ensure you have the necessary underlying software installed (e.g., PHP, Node.js, Git) that EasyKit helps manage. The `scripts/core/check_software_eskit.bat` script can help verify your environment.

### Installation

1.  Clone or download the EasyKit repository to your desired location.
2.  Navigate to the `scripts/core` directory.
3.  Run `install_eskit.bat` to set up EasyKit and create necessary environment configurations or shortcuts.

## Usage

Once installed, you can primarily interact with EasyKit using `run_eskit.bat` from your project's root directory or by directly invoking the scripts in the `scripts/tools` directory.

*   **Main Interface:**
    ```batch
    run_eskit.bat
    ```
    This will typically present you with a menu or options based on your project type and EasyKit configuration.

*   **Tool-Specific Scripts:**
    *   `scripts/tools/git_eskit.bat [git commands]`
    *   `scripts/tools/composer_eskit.bat [composer commands]`
    *   `scripts/tools/npm_eskit.bat [npm commands]`
    *   `scripts/tools/laravel_eskit.bat [artisan commands]`

## Configuration

EasyKit's behavior can be customized through configuration files located in the `config` directory:

*   `config_eskit.bat`: Contains the main configuration for EasyKit, including the current version (`ESKIT_VERSION=2.0.0`).
*   `user_config_eskit.bat`: For user-specific overrides and custom settings. This file is typically not version-controlled.

## Logging

EasyKit maintains logs in the `logs/eskit.log` file (or `config/logs/eskit.log`), which can be helpful for troubleshooting.

## Contributing

(Details on how to contribute can be added here if the project is open to contributions.)

## License

EasyKit is released under the [MIT License](LICENSE).

---

Thank you for using EasyKit! We hope it makes your development process smoother and more efficient.
