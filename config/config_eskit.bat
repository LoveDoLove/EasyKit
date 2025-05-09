@echo off
REM ================================
REM  EasyKit Configuration File (Template for AppData)
REM This file provides default settings.
REM Active configuration is at %APPDATA%\EasyKit\config_eskit.bat
REM ================================

REM Base configuration
set "ESKIT_VERSION=2.0.0"
set "ESKIT_COLOR=0A"
set "ESKIT_TITLE_PREFIX=EasyKit"
set "ESKIT_LOG_PATH=%APPDATA%\EasyKit\logs"
set "ESKIT_ENABLE_LOGGING=true"
set "ESKIT_CHECK_UPDATES=true"
set "ESKIT_UPDATE_URL=https://github.com/LoveDoLove/EasyKit/releases"

REM UI Configuration
set "ESKIT_MENU_WIDTH=60"
set "ESKIT_SHOW_TIPS=true"
set "ESKIT_CONFIRM_EXIT=true"

REM ESKIT_ICON_PATH is set dynamically by run_eskit.bat
