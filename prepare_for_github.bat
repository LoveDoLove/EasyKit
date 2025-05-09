@echo off
REM Script to prepare the NSIS installer for GitHub Actions
REM This avoids issues with the EnVar plugin

echo Preparing NSIS installer for GitHub Actions...

REM Make backup of original NSIS script if it doesn't exist already
if not exist EasyKit.nsi.original (
  copy EasyKit.nsi EasyKit.nsi.original
  echo Created backup of original NSIS script
)

REM Copy the GitHub-specific version to use for the build
copy EasyKit.nsi.github EasyKit.nsi
echo Using GitHub-compatible NSIS script (without EnVar plugin)

echo NSIS script prepared for GitHub Actions. You can now run the workflow.
