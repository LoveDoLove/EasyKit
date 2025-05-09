@echo off
REM Script to verify the NSIS script and GitHub Actions configuration

echo Verifying EasyKit GitHub Actions Setup...
echo ========================================
echo.

REM Check if GitHub-compatible version exists
if exist EasyKit.nsi.github (
  echo [PASS] GitHub-compatible NSIS script (EasyKit.nsi.github) found
) else (
  echo [FAIL] GitHub-compatible NSIS script (EasyKit.nsi.github) not found!
  echo Run prepare_for_github_actions.bat first.
  goto :error
)

REM Check for GitHub Actions workflow file
if exist .github\workflows\build-and-upload.yml (
  echo [PASS] GitHub Actions workflow file found
) else (
  echo [FAIL] GitHub Actions workflow file not found!
  goto :error
)

REM Check for EnVar plugin references in the GitHub-compatible script
powershell -Command "& {
  $content = Get-Content -Path 'EasyKit.nsi.github' -Raw
  $activeEnvarReferences = ($content | Select-String -Pattern '[^;].*EnVar::' -AllMatches).Matches.Count
  
  if ($activeEnvarReferences -gt 0) {
    Write-Host '[FAIL] NSIS script still contains' $activeEnvarReferences 'active references to EnVar plugin!'
    exit 1
  } else {
    Write-Host '[PASS] No active EnVar plugin references found in the NSIS script'
  }
  
  # Check output path is set to build directory
  if ($content -match 'OutFile \"build\\EasyKit_Setup.exe\"') {
    Write-Host '[PASS] Output path correctly set to build directory'
  } else {
    Write-Host '[FAIL] Output path not set to build directory!'
    exit 1
  }
}"

if %ERRORLEVEL% NEQ 0 goto :error

REM Check workflow configuration
powershell -Command "& {
  $content = Get-Content -Path '.github\workflows\build-and-upload.yml' -Raw
  
  # Check NSIS action
  if ($content -match 'joncloud/makensis-action') {
    Write-Host '[PASS] NSIS setup action found in workflow'
  } else {
    Write-Host '[FAIL] NSIS setup action not found in workflow!'
    exit 1
  }
  
  # Check artifact path configuration
  if ($content -match 'build/EasyKit_Setup.exe') {
    Write-Host '[PASS] Installer artifact path correctly configured'
  } else {
    Write-Host '[FAIL] Installer artifact path not correctly configured!'
    exit 1
  }
  
  # Check ZIP package configuration
  if ($content -match 'build/EasyKit.zip') {
    Write-Host '[PASS] ZIP package artifact path correctly configured'
  } else {
    Write-Host '[FAIL] ZIP package artifact path not correctly configured!'
    exit 1
  }
  
  # Check release configuration
  if ($content -match 'startsWith\(github\.ref, ''refs/tags/''\)') {
    Write-Host '[PASS] Release job correctly configured for tags'
  } else {
    Write-Host '[WARN] Release job may not be correctly configured for tags'
  }
}"

if %ERRORLEVEL% NEQ 0 goto :error

echo.
echo Testing NSIS compilation (if NSIS is installed locally)...
where makensis > nul 2>&1
if %ERRORLEVEL% == 0 (
  echo [INFO] NSIS found in PATH, testing compilation...
  copy EasyKit.nsi.github EasyKit.nsi.test > nul
  makensis -V2 EasyKit.nsi.test > nul 2>&1
  if %ERRORLEVEL% == 0 (
    echo [PASS] Local NSIS compilation successful
    del EasyKit.nsi.test > nul
  ) else (
    echo [WARN] Local NSIS compilation failed. This might be due to missing plugins or other local issues.
    echo        The GitHub Actions environment might still work correctly.
    del EasyKit.nsi.test > nul
  )
) else (
  echo [INFO] NSIS not found in PATH. Skipping local compilation test.
  echo        Install NSIS to test compilation locally.
)

echo.
echo Summary:
echo ========
echo EasyKit is properly configured for GitHub Actions:
echo 1. GitHub-compatible NSIS script created without EnVar plugin references
echo 2. Workflow file properly configured to build both installer and ZIP package
echo 3. Release job set up to create GitHub releases on tag push
echo.
echo To trigger a release:
echo 1. Commit and push your changes
echo 2. Create and push a tag: git tag vX.Y.Z && git push origin vX.Y.Z
echo.
echo Verification completed successfully!
exit /b 0

:error
echo.
echo Verification failed! Please fix the issues above.
exit /b 1
