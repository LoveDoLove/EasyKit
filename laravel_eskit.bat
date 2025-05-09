@echo off
setlocal EnableDelayedExpansion
color 0A
title Laravel Menu

:Menu
cls
echo.
echo ================================
echo  Laravel Menu
echo ================================
echo.
echo 0. Back to Main Menu
echo 1. Quick Auto Setup
echo 2. Install Laravel Packages
echo 3. Update Laravel Packages
echo 4. Regenerate Composer AutoLoad Files
echo 5. Build To Production
echo 6. Run Development Server
echo 7. Create Storage Link
echo 8. Run Database Seeding
echo 9. Test Database Connection
echo 10. Laravel Sail Commands (Docker)
echo.
echo Maintenance:
echo 11. Check PHP Version
echo 12. Check Laravel Configuration
echo.
echo Dangerous:
echo 44. Reset All Cache
echo.
set /p choice=Choose an option: 
if "%choice%"=="0" goto Exit
if "%choice%"=="1" goto Setup
if "%choice%"=="2" goto Install
if "%choice%"=="3" goto UpdateLaravelPackages
if "%choice%"=="4" goto RegenerateComposerAutoload
if "%choice%"=="5" goto BuildToProduction
if "%choice%"=="6" goto RunServer
if "%choice%"=="7" goto CreateStorageLink
if "%choice%"=="8" goto RunDatabaseSeeding
if "%choice%"=="9" goto TestDatabaseConnection
if "%choice%"=="10" goto SailMenu
if "%choice%"=="11" goto CheckPHPVersion
if "%choice%"=="12" goto CheckConfiguration
if "%choice%"=="44" goto ResetAllCache
echo Invalid option. Please try again.
timeout /t 2 >nul
goto Menu

:Setup
echo.
echo Starting Laravel quick setup...

REM Check if Laravel is installed
if not exist "artisan" (
    echo ERROR: This doesn't appear to be a Laravel project directory.
    echo Make sure you're in the root directory of a Laravel project.
    goto OperationFailed
)

call :CheckPHPMethod
if !errorlevel! neq 0 goto OperationFailed

call :Install
if !errorlevel! neq 0 goto OperationFailed
call :UpdateLaravelPackages
if !errorlevel! neq 0 goto OperationFailed

REM Check if .env.example exists
if not exist ".env.example" (
    echo ERROR: .env.example file not found. Cannot create environment file.
    goto OperationFailed
)

echo Creating .env file...
if exist ".env" (
    echo .env file already exists.
    choice /c YN /m "Do you want to overwrite it? (Y/N)"
    if !errorlevel! equ 2 (
        echo .env creation skipped.
        goto :SkipEnvCreation
    )
)

call copy .env.example .env
if !errorlevel! neq 0 (
    echo Failed to create .env file!
    goto OperationFailed
)

:SkipEnvCreation
echo Generating application key...
call php artisan key:generate
if !errorlevel! neq 0 goto OperationFailed

REM Check for database configuration before running migrations
echo Checking database configuration...
echo "<?php try { \$db = DB::connection()->getPdo(); echo '1'; } catch (\Exception \$e) { echo '0'; }" > db_connection_test.php
for /f %%i in ('php db_connection_test.php') do set db_connection=%%i
del db_connection_test.php

if "!db_connection!"=="0" (
    echo WARNING: Database connection failed. Migrations will likely fail.
    choice /c YNS /m "Continue with migrations? (Y)es/(N)o/(S)kip"
    if !errorlevel! equ 2 (
        goto OperationFailed
    )
    if !errorlevel! equ 3 (
        goto SkipMigrations
    )
)

echo Running migrations...
call php artisan migrate
if !errorlevel! neq 0 (
    echo Migration failed!
    choice /c YN /m "Do you want to continue setup? (Y/N)"
    if !errorlevel! equ 2 goto OperationFailed
)

:SkipMigrations
echo.
echo Do you want to run database seeders?
choice /c YN /m "Run seeders now? (Y/N)"
if !errorlevel! equ 1 (
    call :RunDatabaseSeeding
)

echo.
echo Do you want to create a storage link?
choice /c YN /m "Create storage link? (Y/N)"
if !errorlevel! equ 1 (
    call :CreateStorageLink
)

echo.
echo Laravel setup completed!
pause
goto Menu

:Install
echo.
echo Installing Laravel packages...
call :CheckSoftwareMethod composer
if !errorlevel! neq 0 goto OperationFailed

echo Checking composer.json...
if not exist "composer.json" (
    echo ERROR: composer.json not found. Cannot install packages.
    exit /b 1
)

REM Check if vendor directory exists and if it's not empty
if exist "vendor" (
    echo Vendor directory already exists.
    choice /c YN /m "Force install/reinstall packages? (Y/N)"
    if !errorlevel! equ 2 (
        echo Skipping package installation.
        exit /b 0
    )
)

call composer install
if !errorlevel! neq 0 exit /b 1

echo.
echo Laravel packages installed successfully!
pause
exit /b 0

:UpdateLaravelPackages
echo.
echo Updating Laravel packages...
call :CheckSoftwareMethod composer
if !errorlevel! neq 0 goto OperationFailed

echo Checking composer.json...
if not exist "composer.json" (
    echo ERROR: composer.json not found. Cannot update packages.
    exit /b 1
)

REM Offer to backup composer.lock first
if exist "composer.lock" (
    choice /c YN /m "Backup composer.lock before updating? (Y/N)"
    if !errorlevel! equ 1 (
        echo Creating backup of composer.lock...
        copy "composer.lock" "composer.lock.backup"
        echo Backup created as composer.lock.backup
    )
)

call composer update
if !errorlevel! neq 0 exit /b 1

echo.
echo Laravel packages updated successfully!
pause
exit /b 0

:RegenerateComposerAutoload
echo.
echo Regenerating Composer autoload files...
call :CheckSoftwareMethod composer
if !errorlevel! neq 0 goto OperationFailed

echo Checking composer.json...
if not exist "composer.json" (
    echo ERROR: composer.json not found. Cannot regenerate autoload files.
    goto OperationFailed
)

call composer dump-autoload
if !errorlevel! neq 0 goto OperationFailed
echo.
echo Composer autoload files regenerated successfully!
pause
goto Menu

:BuildToProduction
echo.
echo Building for production...
call :CheckSoftwareMethod npm
if !errorlevel! neq 0 goto OperationFailed
call npm run build
if !errorlevel! neq 0 goto OperationFailed
echo.
echo Build completed successfully!
pause
goto Menu

:RunServer
echo.
echo Starting Laravel development server...
echo.
echo Press Ctrl+C to stop the server when finished.
echo.
timeout /t 3
call php artisan serve
pause
goto Menu

:CreateStorageLink
echo.
echo Creating storage link...

REM Check if Laravel is installed
if not exist "artisan" (
    echo ERROR: This doesn't appear to be a Laravel project directory.
    goto OperationFailed
)

REM Check if storage link already exists
if exist "public\storage" (
    echo Storage link already exists.
    choice /c YN /m "Do you want to recreate it? (Y/N)"
    if !errorlevel! equ 2 (
        echo Storage link creation skipped.
        goto Menu
    )
    echo Removing existing storage link...
    rmdir "public\storage"
)

call php artisan storage:link
if !errorlevel! neq 0 (
    echo Failed to create storage link!
    goto OperationFailed
)
echo.
echo Storage link created successfully!
pause
goto Menu

:RunDatabaseSeeding
echo.
echo Running database seeders...

REM Check if Laravel is installed
if not exist "artisan" (
    echo ERROR: This doesn't appear to be a Laravel project directory.
    goto OperationFailed
)

REM Check for database connection before seeding
echo Checking database connection...
echo "<?php try { \$db = DB::connection()->getPdo(); echo 'Connection successful!'; } catch (\Exception \$e) { echo 'Connection failed: ' . \$e->getMessage(); }" > db_test.php
call php db_test.php
if !errorlevel! neq 0 (
    del db_test.php
    echo WARNING: Database connection issue detected. Seeding may fail.
    choice /c YN /m "Continue with seeding anyway? (Y/N)"
    if !errorlevel! equ 2 (
        goto Menu
    )
) else (
    del db_test.php
)

REM Check if DatabaseSeeder class exists
echo Checking for seeders...
if not exist "database\seeders\DatabaseSeeder.php" (
    if not exist "database\seeds\DatabaseSeeder.php" (
        echo WARNING: DatabaseSeeder not found. Seeding may fail.
        choice /c YN /m "Continue with seeding anyway? (Y/N)"
        if !errorlevel! equ 2 (
            goto Menu
        )
    )
)

call php artisan db:seed
if !errorlevel! neq 0 (
    echo Failed to run database seeders!
    goto OperationFailed
)
echo.
echo Database seeding completed successfully!
pause
goto Menu

:TestDatabaseConnection
echo.
echo Testing database connection...

REM Check if Laravel is installed
if not exist "artisan" (
    echo ERROR: This doesn't appear to be a Laravel project directory.
    goto OperationFailed
)

REM Try db:monitor first (newer Laravel versions)
call php artisan db:monitor >nul 2>&1
if !errorlevel! equ 0 (
    call php artisan db:monitor
) else (
    REM Fallback to custom test for older Laravel versions
    echo.
    echo Creating custom database test...
    echo "<?php 
try { 
    \$db = DB::connection()->getPdo(); 
    echo 'Connection successful to database: ' . DB::connection()->getDatabaseName();
} catch (\Exception \$e) { 
    echo 'Connection failed: ' . \$e->getMessage(); 
}" > db_test.php
    call php db_test.php
    del db_test.php
)
echo.
pause
goto Menu

:CheckPHPVersion
echo.
echo Checking PHP version...
call php -v
echo.
echo Checking Laravel PHP requirements...
call php artisan --version
echo.
pause
goto Menu

:CheckConfiguration
echo.
echo Checking Laravel configuration...
echo.
echo Application Environment: 
call php artisan env
echo.
echo Application Status:
call php artisan status
echo.
pause
goto Menu

:SailMenu
echo.
echo Checking for Docker...
call docker --version >nul 2>&1
if !errorlevel! neq 0 (
    echo Docker not found. Laravel Sail requires Docker.
    echo Please install Docker Desktop first.
    echo.
    choice /c YN /m "Do you want help installing Docker? (Y/N)"
    if !errorlevel! equ 1 (
        start https://www.docker.com/products/docker-desktop/
    )
    pause
    goto Menu
)

cls
echo.
echo ================================
echo  Laravel Sail Menu
echo ================================
echo.
echo 0. Back to Laravel Menu
echo 1. Install Laravel Sail
echo 2. Start Sail Services
echo 3. Stop Sail Services
echo 4. Run Sail Command
echo.
set /p sailChoice=Choose an option: 
if %sailChoice%==0 goto Menu
if %sailChoice%==1 (
    echo.
    echo Installing Laravel Sail...
    call composer require laravel/sail --dev
    call php artisan sail:install
    echo.
    echo Laravel Sail installed successfully!
    pause
    goto SailMenu
)
if %sailChoice%==2 (
    echo.
    echo Starting Sail services...
    call ./vendor/bin/sail up -d
    echo.
    echo Sail services started!
    pause
    goto SailMenu
)
if %sailChoice%==3 (
    echo.
    echo Stopping Sail services...
    call ./vendor/bin/sail down
    echo.
    echo Sail services stopped!
    pause
    goto SailMenu
)
if %sailChoice%==4 (
    echo.
    set /p sailCmd=Enter Sail command (without './vendor/bin/sail'): 
    echo Running: ./vendor/bin/sail %sailCmd%
    call ./vendor/bin/sail %sailCmd%
    echo.
    pause
    goto SailMenu
)
goto SailMenu

@REM Dangerous
:ResetAllCache
echo.
echo WARNING: This will clear all Laravel cache!
choice /c YN /m "Are you sure you want to proceed? (Y/N)"
if !errorlevel! equ 2 goto Menu

REM Check if Laravel is installed
if not exist "artisan" (
    echo ERROR: This doesn't appear to be a Laravel project directory.
    goto OperationFailed
)

echo.
echo Clearing Laravel caches...
echo.

call php artisan cache:clear
echo Config cache...
call php artisan config:clear
echo Route cache...
call php artisan route:clear
echo View cache...
call php artisan view:clear
echo Events cache...
call php artisan event:clear >nul 2>&1
echo Compiled classes...
call php artisan clear-compiled
echo.
echo Optimizing...
call php artisan optimize
echo.
echo All Laravel caches cleared successfully!
pause
goto Menu

:OperationFailed
echo.
echo Operation failed with errors!
pause
goto Menu

:CheckPHPMethod
echo Checking for PHP...
where php >nul 2>&1
if !errorlevel! neq 0 (
    echo ERROR: PHP is not installed or not in your PATH.
    echo Please install PHP and make sure it's in your system PATH.
    exit /b 1
)
exit /b 0

:CheckSoftwareMethod
call check_software_eskit.bat %1
exit /b !errorlevel!

:Exit
endlocal
exit /b 0
