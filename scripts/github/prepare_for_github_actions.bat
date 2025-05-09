@echo off
REM Script to prepare the NSIS file for GitHub Actions by removing EnVar plugin usage

echo Preparing NSIS script for GitHub Actions...

REM Make a backup of the original file if it doesn't exist
if not exist "%~dp0..\..\installer\EasyKit.nsi.original" (
  copy "%~dp0..\..\installer\EasyKit.nsi" "%~dp0..\..\installer\EasyKit.nsi.original"
  echo Backup of original NSIS script created.
)

REM Ensure build directory exists
if not exist "%~dp0..\..\build" mkdir "%~dp0..\..\build"

REM Create a GitHub-compatible version without EnVar plugin usage
powershell -Command "& {
  $content = Get-Content -Path '%~dp0..\..\installer\EasyKit.nsi' -Raw
  
  # Update output path to build directory
  $content = $content -replace 'OutFile \"dist\\EasyKit_Setup.exe\"', 'OutFile \"build\EasyKit_Setup.exe\"'
  
  # Comment out the EnVar plugin section
  $pathSectionPattern = '(?s)Section \"Add to PATH\" SecPath.*?EndSection'
  $content = $content -replace $pathSectionPattern, '; Section \"Add to PATH\" - Removed for CI compatibility'
  
  # Remove the PATH section description
  $content = $content -replace '!insertmacro MUI_DESCRIPTION_TEXT \$\{SecPath\} \".*?\"', '; PATH section description removed'
  
  # Remove EnVar from uninstaller section
  $content = $content -replace 'EnVar::DeleteValue \"PATH\" \"\$INSTDIR\"', '; EnVar::DeleteValue \"PATH\" \"$INSTDIR\" - Removed for CI compatibility'
  
  # Write to the GitHub version
  Set-Content -Path '%~dp0..\..\installer\EasyKit.nsi.github' -Value $content
  Write-Host 'Created GitHub-compatible NSIS script in EasyKit.nsi.github'
}"

REM Copy the GitHub version to the main NSIS file for local testing
copy "%~dp0..\..\installer\EasyKit.nsi.github" "%~dp0..\..\installer\EasyKit.nsi"
echo NSIS script prepared for GitHub Actions

echo.
echo GitHub Actions workflow configuration:
echo -------------------------------------
echo The workflow file at .github/workflows/build-and-upload.yml has been configured to:
echo 1. Modify the NSIS script to remove EnVar plugin usage
echo 2. Build the installer using the NSIS compiler
echo 3. Create a ZIP package with the installer and other files
echo 4. Upload the artifacts as part of the build
echo 5. Create a GitHub release when a new tag is pushed
echo.

echo Done! GitHub Actions should now be able to build the installer without EnVar plugin errors.
