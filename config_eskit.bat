@echo off
REM ================================
REM  EasyKit Configuration File
REM ================================

REM Base configuration
set "ESKIT_VERSION=1.1.0"
set "ESKIT_COLOR=0A"
set "ESKIT_TITLE_PREFIX=EasyKit"
set "ESKIT_LOG_PATH=%~dp0logs"
set "ESKIT_ENABLE_LOGGING=true"
set "ESKIT_CHECK_UPDATES=true"
set "ESKIT_UPDATE_URL=https://github.com/LoveDoLove/EasyKit/releases"

REM UI Configuration
set "ESKIT_MENU_WIDTH=60"
set "ESKIT_SHOW_TIPS=true"
set "ESKIT_CONFIRM_EXIT=true"

REM Software paths
set "ESKIT_ICON_PATH=%~dp0images\icon.ico"

REM Customize this section for your environment
REM Example: set "ESKIT_DEFAULT_PROJECT_PATH=D:\Projects"

REM Don't modify below this line
REM ================================

REM Create logs directory if it doesn't exist
if not exist "%ESKIT_LOG_PATH%" (
    mkdir "%ESKIT_LOG_PATH%" >nul 2>&1
)

REM Function to log messages if logging is enabled
:log
if "%ESKIT_ENABLE_LOGGING%"=="true" (
    echo %DATE% %TIME% - %* >> "%ESKIT_LOG_PATH%\eskit.log"
)
goto :eof
