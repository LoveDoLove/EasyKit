# PowerShell script to update GitHub Actions workflow file
Write-Host "Updating GitHub Actions workflow file..."

# Define the path to the workflow file
$workflowPath = ".github\workflows\build-and-upload.yml"

# Make a backup of the original workflow file
Copy-Item -Path $workflowPath -Destination "$workflowPath.bak" -Force
Write-Host "Created backup of original workflow file"

# Define the content for the new workflow file
$workflowContent = @"
name: Build and Upload EasyKit

on:
  push:
    branches: [ main ]
    tags: [ 'v*' ]
  pull_request:
    branches: [ main ]
  workflow_dispatch:

jobs:
  build:
    name: Build EasyKit Packages
    runs-on: windows-latest
    permissions:
      contents: write

    steps:
      - name: Checkout code
        uses: actions/checkout@v4

      - name: Create build directory
        run: mkdir -p build
        
      - name: Fix NSIS script
        shell: pwsh
        run: |
          # Create a modified version of the NSIS script without EnVar plugin usage
          $content = Get-Content -Path ./installer/EasyKit.nsi -Raw
          $content = $content -replace 'OutFile "dist\\EasyKit_Setup.exe"', 'OutFile "build\EasyKit_Setup.exe"'
          
          # Fix paths to image files (after restructuring)
          $content = $content -replace '!define MUI_ICON "images\\icon.ico"', '!define MUI_ICON "..\images\icon.ico"'
          $content = $content -replace '!define MUI_UNICON "images\\icon.ico"', '!define MUI_UNICON "..\images\icon.ico"'
          $content = $content -replace '!define MUI_WELCOMEFINISHPAGE_BITMAP "images\\installer-welcome.bmp"', '!define MUI_WELCOMEFINISHPAGE_BITMAP "..\images\installer-welcome.bmp"'
          $content = $content -replace '!define MUI_HEADERIMAGE_BITMAP "images\\installer-header.bmp"', '!define MUI_HEADERIMAGE_BITMAP "..\images\installer-header.bmp"'
          
          # Fix path to LICENSE file
          $content = $content -replace '!insertmacro MUI_PAGE_LICENSE "LICENSE"', '!insertmacro MUI_PAGE_LICENSE "..\LICENSE"'
          
          # Comment out the EnVar plugin section
          $pathSectionPattern = '(?s)Section "Add to PATH" SecPath.*?EndSection'
          $content = $content -replace $pathSectionPattern, '; Section "Add to PATH" - Removed for CI compatibility'
          
          # Remove the PATH section description
          $content = $content -replace '!insertmacro MUI_DESCRIPTION_TEXT \$\{SecPath\} ".*?"', '; PATH section description removed'
          
          # Remove EnVar from uninstaller section
          $content = $content -replace 'EnVar::DeleteValue "PATH" "\\$INSTDIR"', '; EnVar::DeleteValue "PATH" "$INSTDIR" - Removed for CI compatibility'
          
          Set-Content -Path ./installer/EasyKit.nsi -Value $content
          
      - name: Setup NSIS
        uses: joncloud/makensis-action@v4
        with:
          script-file: ./installer/EasyKit.nsi
        
      - name: Build installer
        shell: pwsh
        run: |
          makensis -V4 ./installer/EasyKit.nsi
          if ($LASTEXITCODE -ne 0) {
            Write-Host "NSIS build failed with exit code $LASTEXITCODE"
            exit $LASTEXITCODE
          }
          
      - name: Build ZIP package
        shell: pwsh
        run: |
          Compress-Archive -Path scripts/**/*.bat,run_eskit.bat,README.md,LICENSE,images/* -DestinationPath build\EasyKit.zip -Force
          
      - name: Verify build artifacts
        shell: pwsh
        run: |
          Get-ChildItem -Path "build"
          
      - name: Upload artifacts
        uses: actions/upload-artifact@v4
        with:
          name: EasyKit-Packages
          path: |
            build/EasyKit_Setup.exe
            build/EasyKit.zip
          retention-days: 14

  release:
    name: Create Release
    if: startsWith(github.ref, 'refs/tags/')
    needs: build
    runs-on: windows-latest
    permissions:
      contents: write
    
    steps:
      - name: Download artifacts
        uses: actions/download-artifact@v4
        with:
          name: EasyKit-Packages
          path: release-artifacts
      
      - name: Create GitHub Release
        uses: softprops/action-gh-release@v1
        with:
          files: |
            release-artifacts/EasyKit.zip
            release-artifacts/EasyKit_Setup.exe
          draft: false
          generate_release_notes: true
"@

# Write the content to the workflow file
Set-Content -Path $workflowPath -Value $workflowContent

Write-Host "GitHub Actions workflow file updated successfully"
Write-Host ""
Write-Host "You need to commit these changes and push them to your repository"
Write-Host "Then manually trigger the workflow to test the build process"
