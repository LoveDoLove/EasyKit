@echo off
echo Creating fixed WiX workflow file for GitHub Actions
echo.

rem Create directories if they don't exist
mkdir .github\workflows 2>nul
mkdir scripts\build 2>nul

rem Create release_v1.2.6.bat if it doesn't exist
if not exist scripts\build\release_v1.2.6.bat (
  echo Creating release_v1.2.6.bat...
  echo @echo off > scripts\build\release_v1.2.6.bat
  echo echo EasyKit Release v1.2.6 >> scripts\build\release_v1.2.6.bat
  echo echo Build date: %%date%% %%time%% >> scripts\build\release_v1.2.6.bat
  echo echo. >> scripts\build\release_v1.2.6.bat
  echo echo This script is part of the EasyKit installation package. >> scripts\build\release_v1.2.6.bat
  echo echo For more information, visit: https://github.com/LoveDoLove/EasyKit >> scripts\build\release_v1.2.6.bat
  echo echo. >> scripts\build\release_v1.2.6.bat
  echo pause >> scripts\build\release_v1.2.6.bat
  echo File created.
) else (
  echo release_v1.2.6.bat already exists.
)

echo Creating/updating workflow file...
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
echo         run: mkdir build >> .github\workflows\build-and-upload.yml
echo         shell: cmd >> .github\workflows\build-and-upload.yml
echo. >> .github\workflows\build-and-upload.yml
echo       - name: Setup build environment >> .github\workflows\build-and-upload.yml
echo         shell: cmd >> .github\workflows\build-and-upload.yml
echo         run: ^| >> .github\workflows\build-and-upload.yml
echo           IF NOT EXIST scripts\build mkdir scripts\build >> .github\workflows\build-and-upload.yml
echo           IF NOT EXIST scripts\build\release_v1.2.6.bat ^( >> .github\workflows\build-and-upload.yml
echo             echo @echo off ^> scripts\build\release_v1.2.6.bat >> .github\workflows\build-and-upload.yml
echo             echo echo EasyKit Release v1.2.6 ^>^> scripts\build\release_v1.2.6.bat >> .github\workflows\build-and-upload.yml
echo             echo echo Build date: %%%%date%%%% %%%%time%%%% ^>^> scripts\build\release_v1.2.6.bat >> .github\workflows\build-and-upload.yml
echo             echo echo. ^>^> scripts\build\release_v1.2.6.bat >> .github\workflows\build-and-upload.yml
echo             echo echo This script is part of the EasyKit installation package. ^>^> scripts\build\release_v1.2.6.bat >> .github\workflows\build-and-upload.yml
echo             echo echo For more information, visit: https://github.com/LoveDoLove/EasyKit ^>^> scripts\build\release_v1.2.6.bat >> .github\workflows\build-and-upload.yml
echo             echo echo. ^>^> scripts\build\release_v1.2.6.bat >> .github\workflows\build-and-upload.yml
echo             echo pause ^>^> scripts\build\release_v1.2.6.bat >> .github\workflows\build-and-upload.yml
echo           ^) >> .github\workflows\build-and-upload.yml
echo. >> .github\workflows\build-and-upload.yml
echo       - name: Compile WiX source file >> .github\workflows\build-and-upload.yml
echo         shell: cmd >> .github\workflows\build-and-upload.yml
echo         run: candle.exe -dProjectDir="%%CD%%\\" -out build\EasyKit.wxs.obj installer\EasyKit.wxs -ext WixUIExtension >> .github\workflows\build-and-upload.yml
echo. >> .github\workflows\build-and-upload.yml
echo       - name: Link WiX object file to create MSI >> .github\workflows\build-and-upload.yml
echo         shell: cmd >> .github\workflows\build-and-upload.yml
echo         run: light.exe -out build\EasyKit_Setup.msi build\EasyKit.wxs.obj -ext WixUIExtension -cultures:en-us >> .github\workflows\build-and-upload.yml
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
echo             build/EasyKit_Setup.msi >> .github\workflows\build-and-upload.yml
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
echo             release-artifacts/EasyKit_Setup.msi >> .github\workflows\build-and-upload.yml
echo           draft: false >> .github\workflows\build-and-upload.yml
echo           generate_release_notes: true >> .github\workflows\build-and-upload.yml

echo.
echo Workflow file created/updated successfully.
echo Please verify the changes before committing.
echo.
echo Script completed.
