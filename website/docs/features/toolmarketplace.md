# Tool Marketplace

The Tool Marketplace in EasyKit helps you identify, install, and manage essential project tools, ensuring your development environment is ready for modern workflows.

## Introduction

Tool Marketplace checks for the presence of key development tools (such as Node.js, npm, PHP, Composer, and Git) and provides download links for any missing components. This streamlines the setup process for new projects and helps maintain a consistent environment.

## Key Features

- **Check Installed Tools:** Scans your system for Node.js, npm, PHP, Composer, and Git.
- **Display Installed Tools:** Lists all tools currently detected on your system.
- **Display Missing Tools:** Shows missing tools and provides direct download links.
- **Install Tools:** Opens the download page for a missing tool, making installation easy.

## Usage

Access the Tool Marketplace from the main menu. You can:

- **View installed and missing tools:**
  - Instantly see which tools are available and which need to be installed.
- **Install missing tools:**
  - Select a missing tool to open its official download page in your browser.

## Code Overview

The `ToolMarketplaceController` manages this feature. Key methods include:
- `ShowMarketplace`
- `IsToolInstalled`
- `InstallTool`

For more information, refer to the EasyKit documentation or explore the Tool Marketplace in the application.