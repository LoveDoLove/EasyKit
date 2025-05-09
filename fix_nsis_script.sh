#!/bin/bash

# Script to check for NSIS build errors
echo "Checking for potential NSIS errors in EasyKit workflow"

# Create a backup of the original NSIS file if it exists
if [ -f "EasyKit.nsi" ]; then
  cp EasyKit.nsi EasyKit.nsi.backup
  echo "Created backup of original NSIS file"
else
  echo "Error: EasyKit.nsi not found!"
  exit 1
fi

# Check and fix potential syntax issues in the NSIS file
echo "Applying fixes to NSIS file..."

# Ensure proper empty parameters for shortcuts
sed -i 's/\(CreateShortcut "[^"]*" "[^"]*"\) /\1 "" /g' EasyKit.nsi

# Fix the desktop shortcut section if needed
sed -i 's/Section "Desktop Shortcut" SecDesktop/Section "Desktop Shortcut" SecDesktop/g' EasyKit.nsi

# Add missing SectionEnd if needed
if ! grep -q "SectionEnd.*Section \"Start Menu Shortcuts\"" EasyKit.nsi; then
  sed -i 's/Section "Start Menu Shortcuts" SecStartMenu/SectionEnd\n\nSection "Start Menu Shortcuts" SecStartMenu/g' EasyKit.nsi
fi

# Add missing SectionEnd if needed
if ! grep -q "SectionEnd.*Section \"Add to PATH\"" EasyKit.nsi; then
  sed -i 's/Section "Add to PATH" SecPath/SectionEnd\n\nSection "Add to PATH" SecPath/g' EasyKit.nsi
fi

echo "Fixes applied. Check EasyKit.nsi for proper syntax"

# Test build the script on development environments (comment out in CI)
# makensis -V4 EasyKit.nsi

echo "Script completed. You can now build the installer with: makensis -V4 EasyKit.nsi"
