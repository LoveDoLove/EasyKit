@echo off
echo Creating fixed workflow file

echo name: Build and Upload EasyKit > .github\workflows\build-and-upload.yml
echo. >> .github\workflows\build-and-upload.yml
echo on: >> .github\workflows\build-and-upload.yml
echo   push: >> .github\workflows\build-and-upload.yml
echo     branches: [ main ] >> .github\workflows\build-and-upload.yml
echo     tags: [ 'v*' ] >> .github\workflows\build-and-upload.yml
echo   pull_request: >> .github\workflows\build-and-upload.yml
echo     branches: [ main ] >> .github\workflows\build-and-upload.yml
echo   workflow_dispatch: >> .github\workflows\build-and-upload.yml
echo. >> .github\workflows\build-and-upload.yml
echo jobs: >> .github\workflows\build-and-upload.yml
echo   build: >> .github\workflows\build-and-upload.yml
echo     name: Build EasyKit Packages >> .github\workflows\build-and-upload.yml
echo     runs-on: windows-latest >> .github\workflows\build-and-upload.yml
echo     permissions: >> .github\workflows\build-and-upload.yml
echo       contents: write >> .github\workflows\build-and-upload.yml
echo. >> .github\workflows\build-and-upload.yml
echo     steps: >> .github\workflows\build-and-upload.yml
echo       - name: Checkout code >> .github\workflows\build-and-upload.yml
echo         uses: actions/checkout@v4 >> .github\workflows\build-and-upload.yml
echo. >> .github\workflows\build-and-upload.yml
echo       - name: Create build directory >> .github\workflows\build-and-upload.yml
echo         run: mkdir -p build >> .github\workflows\build-and-upload.yml
echo. >> .github\workflows\build-and-upload.yml
echo       - name: Fix NSIS script >> .github\workflows\build-and-upload.yml
echo         shell: pwsh >> .github\workflows\build-and-upload.yml
echo         run: ^| >> .github\workflows\build-and-upload.yml
echo           # Create a modified version of the NSIS script without EnVar plugin usage >> .github\workflows\build-and-upload.yml
echo           $content = Get-Content -Path ./installer/EasyKit.nsi -Raw >> .github\workflows\build-and-upload.yml
echo           $content = $content -replace 'OutFile "dist\\EasyKit_Setup.exe"', 'OutFile "build\EasyKit_Setup.exe"' >> .github\workflows\build-and-upload.yml
echo. >> .github\workflows\build-and-upload.yml
echo           # Fix paths to image files (after restructuring) >> .github\workflows\build-and-upload.yml
echo           $content = $content -replace '!define MUI_ICON "images\\icon.ico"', '!define MUI_ICON "..\images\icon.ico"' >> .github\workflows\build-and-upload.yml
echo           $content = $content -replace '!define MUI_UNICON "images\\icon.ico"', '!define MUI_UNICON "..\images\icon.ico"' >> .github\workflows\build-and-upload.yml
echo           $content = $content -replace '!define MUI_WELCOMEFINISHPAGE_BITMAP "images\\installer-welcome.bmp"', '!define MUI_WELCOMEFINISHPAGE_BITMAP "..\images\installer-welcome.bmp"' >> .github\workflows\build-and-upload.yml
echo           $content = $content -replace '!define MUI_HEADERIMAGE_BITMAP "images\\installer-header.bmp"', '!define MUI_HEADERIMAGE_BITMAP "..\images\installer-header.bmp"' >> .github\workflows\build-and-upload.yml
echo. >> .github\workflows\build-and-upload.yml
echo           # Fix path to LICENSE file >> .github\workflows\build-and-upload.yml
echo           $content = $content -replace '!insertmacro MUI_PAGE_LICENSE "LICENSE"', '!insertmacro MUI_PAGE_LICENSE "..\LICENSE"' >> .github\workflows\build-and-upload.yml
echo. >> .github\workflows\build-and-upload.yml
echo           # Comment out the EnVar plugin section >> .github\workflows\build-and-upload.yml
echo           $pathSectionPattern = '(?s)Section "Add to PATH" SecPath.*?EndSection' >> .github\workflows\build-and-upload.yml
echo           $content = $content -replace $pathSectionPattern, '; Section "Add to PATH" - Removed for CI compatibility' >> .github\workflows\build-and-upload.yml
echo. >> .github\workflows\build-and-upload.yml
echo           # Remove the PATH section description >> .github\workflows\build-and-upload.yml
echo           $content = $content -replace '!insertmacro MUI_DESCRIPTION_TEXT \$\{SecPath\} ".*?"', '; PATH section description removed' >> .github\workflows\build-and-upload.yml
echo. >> .github\workflows\build-and-upload.yml
echo           # Remove EnVar from uninstaller section >> .github\workflows\build-and-upload.yml
echo           $content = $content -replace 'EnVar::DeleteValue "PATH" "\$INSTDIR"', '; EnVar::DeleteValue "PATH" "$INSTDIR" - Removed for CI compatibility' >> .github\workflows\build-and-upload.yml
echo. >> .github\workflows\build-and-upload.yml
echo           Set-Content -Path ./installer/EasyKit.nsi -Value $content >> .github\workflows\build-and-upload.yml
echo. >> .github\workflows\build-and-upload.yml
echo       - name: Setup NSIS >> .github\workflows\build-and-upload.yml
echo         uses: joncloud/makensis-action@v4 >> .github\workflows\build-and-upload.yml
echo         with: >> .github\workflows\build-and-upload.yml
echo           script-file: ./installer/EasyKit.nsi >> .github\workflows\build-and-upload.yml
echo. >> .github\workflows\build-and-upload.yml
echo       - name: Build installer >> .github\workflows\build-and-upload.yml
echo         shell: pwsh >> .github\workflows\build-and-upload.yml
echo         run: ^| >> .github\workflows\build-and-upload.yml
echo           makensis -V4 ./installer/EasyKit.nsi >> .github\workflows\build-and-upload.yml
echo           if ($LASTEXITCODE -ne 0) { >> .github\workflows\build-and-upload.yml
echo             Write-Host "NSIS build failed with exit code $LASTEXITCODE" >> .github\workflows\build-and-upload.yml
echo             exit $LASTEXITCODE >> .github\workflows\build-and-upload.yml
echo           } >> .github\workflows\build-and-upload.yml
echo. >> .github\workflows\build-and-upload.yml
echo       - name: Build ZIP package >> .github\workflows\build-and-upload.yml
echo         shell: pwsh >> .github\workflows\build-and-upload.yml
echo         run: ^| >> .github\workflows\build-and-upload.yml
echo           Compress-Archive -Path scripts/**/*.bat,run_eskit.bat,README.md,LICENSE,images/* -DestinationPath build\EasyKit.zip -Force >> .github\workflows\build-and-upload.yml
echo. >> .github\workflows\build-and-upload.yml
echo       - name: Verify build artifacts >> .github\workflows\build-and-upload.yml
echo         shell: pwsh >> .github\workflows\build-and-upload.yml
echo         run: ^| >> .github\workflows\build-and-upload.yml
echo           Get-ChildItem -Path "build" >> .github\workflows\build-and-upload.yml
echo. >> .github\workflows\build-and-upload.yml
echo       - name: Upload artifacts >> .github\workflows\build-and-upload.yml
echo         uses: actions/upload-artifact@v4 >> .github\workflows\build-and-upload.yml
echo         with: >> .github\workflows\build-and-upload.yml
echo           name: EasyKit-Packages >> .github\workflows\build-and-upload.yml
echo           path: ^| >> .github\workflows\build-and-upload.yml
echo             build/EasyKit_Setup.exe >> .github\workflows\build-and-upload.yml
echo             build/EasyKit.zip >> .github\workflows\build-and-upload.yml
echo           retention-days: 14 >> .github\workflows\build-and-upload.yml
echo. >> .github\workflows\build-and-upload.yml
echo   release: >> .github\workflows\build-and-upload.yml
echo     name: Create Release >> .github\workflows\build-and-upload.yml
echo     if: startsWith(github.ref, 'refs/tags/') >> .github\workflows\build-and-upload.yml
echo     needs: build >> .github\workflows\build-and-upload.yml
echo     runs-on: windows-latest >> .github\workflows\build-and-upload.yml
echo     permissions: >> .github\workflows\build-and-upload.yml
echo       contents: write >> .github\workflows\build-and-upload.yml
echo. >> .github\workflows\build-and-upload.yml
echo     steps: >> .github\workflows\build-and-upload.yml
echo       - name: Download artifacts >> .github\workflows\build-and-upload.yml
echo         uses: actions/download-artifact@v4 >> .github\workflows\build-and-upload.yml
echo         with: >> .github\workflows\build-and-upload.yml
echo           name: EasyKit-Packages >> .github\workflows\build-and-upload.yml
echo           path: release-artifacts >> .github\workflows\build-and-upload.yml
echo. >> .github\workflows\build-and-upload.yml
echo       - name: Create GitHub Release >> .github\workflows\build-and-upload.yml
echo         uses: softprops/action-gh-release@v1 >> .github\workflows\build-and-upload.yml
echo         with: >> .github\workflows\build-and-upload.yml
echo           files: ^| >> .github\workflows\build-and-upload.yml
echo             release-artifacts/EasyKit.zip >> .github\workflows\build-and-upload.yml
echo             release-artifacts/EasyKit_Setup.exe >> .github\workflows\build-and-upload.yml
echo           draft: false >> .github\workflows\build-and-upload.yml
echo           generate_release_notes: true >> .github\workflows\build-and-upload.yml

echo Workflow file created successfully
