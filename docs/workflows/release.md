# Release Workflow

This document describes the release workflow for EasyKit.

## Versioning

EasyKit follows [Semantic Versioning](https://semver.org/):
- **MAJOR**: Breaking changes
- **MINOR**: New features (backward compatible)
- **PATCH**: Bug fixes (backward compatible)

Current version: 4.2.2.0

## Release Steps

### 1. Update Version

Update version in:
- `EasyKit/EasyKit.csproj`
- `EasyKit-Gui/EasyKit-Gui.csproj`
- `README.md`
- `CHANGELOG.md`

### 2. Update CHANGELOG

Add entries for:
- Added features
- Changed behavior
- Fixed bugs
- Security fixes

### 3. Run Full Test Suite

```powershell
dotnet test tests/EasyKit.Tests/EasyKit.Tests.csproj
dotnet build EasyKit.sln
```

### 4. Create Release Branch

```powershell
git checkout -b release/4.2.2
git push origin release/4.2.2
```

### 5. Build Release Artifacts

```powershell
# Build for Windows x64
dotnet publish EasyKit/EasyKit.csproj -c Release -r win-x64 --self-contained true -o publish/win-x64

# Build for Windows x86
dotnet publish EasyKit/EasyKit.csproj -c Release -r win-x86 --self-contained true -o publish/win-x86
```

### 6. Create Installer

Use Inno Setup to create installer:
```powershell
iscc EasyKit.iss
```

### 7. Tag and Release

```powershell
git tag v4.2.2
git push origin v4.2.2
```

### 8. Publish to GitHub Releases

Upload:
- `EasyKit-4.2.2-x64.exe`
- `EasyKit-4.2.2-x86.exe`
- Source code archive

### 9. Update Main Branch

```powershell
git checkout main
git merge release/4.2.2
git push origin main
```

## Release Checklist

- [ ] Version updated in all files
- [ ] CHANGELOG.md updated
- [ ] All tests pass
- [ ] Release branch created
- [ ] Artifacts built
- [ ] Installer created
- [ ] Tag pushed
- [ ] GitHub release published
- [ ] Main branch updated

## Evidence

- GitHub Actions CI/CD pipeline
- Inno Setup installer configuration
- CHANGELOG.md tracks all releases
