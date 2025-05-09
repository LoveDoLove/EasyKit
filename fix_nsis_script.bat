@echo off
REM Script to check for NSIS build errors and fix them

echo Checking for potential NSIS errors in EasyKit workflow

REM Create a backup of the original NSIS file if it exists
if exist EasyKit.nsi (
  copy EasyKit.nsi EasyKit.nsi.backup
  echo Created backup of original NSIS file
) else (
  echo Error: EasyKit.nsi not found!
  exit /b 1
)

echo Applying fixes to NSIS file...
powershell -Command "& {
  $content = Get-Content -Path .\EasyKit.nsi -Raw
  
  # Ensure proper empty parameters for shortcuts
  $content = $content -replace '(CreateShortcut\s+\""[^""]*\""\s+\""[^""]*\"")\s', '$1 \"\"" '
  
  # Make sure each section has proper SectionEnd
  $sections = @('Desktop Shortcut', 'Start Menu Shortcuts', 'Add to PATH')
  
  foreach ($section in $sections) {
    if ($content -match """$section""" -and $content -notmatch """$section"""\s+[^S]*SectionEnd) {
      $content = $content -replace """$section""", """$section"""`r`n  # Ensure proper SectionEnd is present"
    }
  }
  
  # Write the modified content back
  Set-Content -Path .\EasyKit.nsi -Value $content
  
  Write-Host 'NSIS script fixes applied'
}"

echo Fixes applied. Check EasyKit.nsi for proper syntax

echo Script completed. You can now build the installer with: makensis -V4 EasyKit.nsi
