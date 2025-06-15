# Composer Features

Composer is the leading dependency manager for PHP, allowing you to manage project libraries, install packages, and automate autoloading. EasyKit integrates Composer with a user-friendly interface, making PHP project setup and maintenance simple—even for users unfamiliar with the command line.

## Introduction

Composer automates PHP dependency management by reading your `composer.json` file, resolving and installing required packages, and generating an autoloader. With EasyKit, you can perform common Composer operations directly from the application, streamlining your workflow and reducing manual steps.

## Key Features

- **Install:** Installs dependencies from `composer.json` (`composer install`).
- **Update:** Updates dependencies to the latest versions, respecting version constraints (`composer update`).
- **Autoload:** Regenerates the Composer autoloader (`composer dump-autoload`).
- **Require:** Adds a new dependency (`composer require vendor/package`).
- **Create Project:** Creates a new project from a package (`composer create-project vendor/package path version`).
- **Validate:** Validates your `composer.json` for errors (`composer validate`).
- **Clear Cache:** Clears the Composer cache (`composer clear-cache`).
- **Show Info:** Displays `composer.json` content and installed packages (`composer show`).
- **Diagnostics:** Checks for common configuration problems and platform requirements (`composer check-platform-reqs`).

## Usage

Access the Composer feature from the main menu in EasyKit. The Composer menu provides options for managing PHP dependencies, such as installing, updating, or adding packages. You can also:

- **Initialize a new project:**
  ```shell
  composer init
  ```
- **Add a dependency:**
  ```shell
  composer require foo/bar
  ```
- **Update a specific package:**
  ```shell
  composer update vendor/package
  ```
- **Regenerate autoload files:**
  ```shell
  composer dump-autoload
  ```
- **Validate configuration:**
  ```shell
  composer validate
  ```

For advanced configuration, you can edit `composer.json` directly or use EasyKit's interface to manage dependencies and scripts. For more details, see the [Composer documentation](https://getcomposer.org/doc/).
