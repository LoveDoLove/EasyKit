# Laravel Features

Laravel is a powerful PHP web application framework with expressive, elegant syntax. EasyKit integrates Laravel tools to simplify development, automate common tasks, and streamline project setup.

## Introduction

Laravel provides a robust foundation for building modern web applications, offering features like routing, database migrations, Eloquent ORM, and a rich ecosystem of packages. EasyKit brings essential Laravel commands and utilities into a user-friendly interface, making it easier to manage your Laravel projects.

## Key Features

- **Quick Setup:** Creates `.env` (if missing), installs dependencies, generates app key, clears config/cache.
- **Install:** Installs Composer packages (`composer install`).
- **Update:** Updates Composer packages (`composer update`).
- **Autoload:** Regenerates autoload files (`composer dump-autoload`).
- **Production Build:** Optimizes for production (installs production dependencies, caches config/routes/views) using `php artisan optimize` and related commands.
- **Serve:** Starts the development server (`php artisan serve`).
- **Storage Link:** Creates a symbolic link for storage (`php artisan storage:link`).
- **Database Seeding:** Runs migrations and seeders (`php artisan migrate --seed`).
- **Test DB Connection:** Tests the database connection.
- **Check PHP Version:** Checks the PHP version (`php -v`).
- **Check Laravel Config:** Checks the Laravel configuration (`php artisan config:cache`).
- **Reset Cache:** Clears all Laravel caches (`php artisan cache:clear`, `php artisan config:clear`, etc.).
- **Route List:** Displays defined routes (`php artisan route:list`).
- **PHP Diagnostics:** Checks PHP environment for compatibility.

## Usage

Access the Laravel menu in EasyKit and select the desired option. You can also use the following commands in your terminal for common tasks:

- **Run migrations:**
  ```shell
  php artisan migrate
  ```
- **Seed the database:**
  ```shell
  php artisan db:seed
  ```
- **List routes:**
  ```shell
  php artisan route:list
  ```
- **Optimize for production:**
  ```shell
  php artisan optimize
  ```
- **Start development server:**
  ```shell
  php artisan serve
  ```

For advanced features, refer to the [official Laravel documentation](https://laravel.com/docs).