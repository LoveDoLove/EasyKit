@echo off
color 0A
title Git Menu

:Menu
cls
echo.
echo ================================
echo  Git Menu
echo ================================
echo.
echo 0. Back to Main Menu
echo 1. Initialize Repository
echo 2. Check Status
echo 3. Add All Changes
echo 4. Commit Changes
echo 5. Push to Origin
echo 6. Pull from Origin
echo 7. Create New Branch
echo 8. Switch Branch
echo 9. Merge Branch
echo.
echo Advanced:
echo 10. View Commit History
echo 11. Stash Changes
echo 12. Apply Stash
echo.
set /p choice=Choose an option: 
if %choice%==0 goto Exit
if %choice%==1 goto InitRepo
if %choice%==2 goto CheckStatus
if %choice%==3 goto AddAll
if %choice%==4 goto Commit
if %choice%==5 goto Push
if %choice%==6 goto Pull
if %choice%==7 goto CreateBranch
if %choice%==8 goto SwitchBranch
if %choice%==9 goto MergeBranch
if %choice%==10 goto ViewHistory
if %choice%==11 goto StashChanges
if %choice%==12 goto ApplyStash
goto Menu

:InitRepo
echo.
echo Initializing Git repository...
call :CheckSoftwareMethod git
if %errorlevel% neq 0 goto OperationFailed
git init
if %errorlevel% neq 0 goto OperationFailed
echo.
echo Git repository initialized successfully!
pause
goto Menu

:CheckStatus
echo.
echo Checking Git status...
call :CheckSoftwareMethod git
if %errorlevel% neq 0 goto OperationFailed
git status
echo.
pause
goto Menu

:AddAll
echo.
echo Adding all changes to staging...
call :CheckSoftwareMethod git
if %errorlevel% neq 0 goto OperationFailed
git add .
if %errorlevel% neq 0 goto OperationFailed
echo.
echo All changes added to staging!
pause
goto Menu

:Commit
echo.
call :CheckSoftwareMethod git
if %errorlevel% neq 0 goto OperationFailed
set /p message=Enter commit message: 
if "%message%"=="" (
    echo Commit message cannot be empty!
    pause
    goto Menu
)
git commit -m "%message%"
if %errorlevel% neq 0 goto OperationFailed
echo.
echo Changes committed successfully!
pause
goto Menu

:Push
echo.
echo Pushing to remote repository...
call :CheckSoftwareMethod git
if %errorlevel% neq 0 goto OperationFailed
set /p branch=Enter branch name (leave empty for current branch): 
if "%branch%"=="" (
    git push
) else (
    git push origin %branch%
)
if %errorlevel% neq 0 goto OperationFailed
echo.
echo Changes pushed successfully!
pause
goto Menu

:Pull
echo.
echo Pulling from remote repository...
call :CheckSoftwareMethod git
if %errorlevel% neq 0 goto OperationFailed
set /p branch=Enter branch name (leave empty for current branch): 
if "%branch%"=="" (
    git pull
) else (
    git pull origin %branch%
)
if %errorlevel% neq 0 goto OperationFailed
echo.
echo Changes pulled successfully!
pause
goto Menu

:CreateBranch
echo.
call :CheckSoftwareMethod git
if %errorlevel% neq 0 goto OperationFailed
set /p branch=Enter new branch name: 
if "%branch%"=="" (
    echo Branch name cannot be empty!
    pause
    goto Menu
)
git checkout -b %branch%
if %errorlevel% neq 0 goto OperationFailed
echo.
echo New branch '%branch%' created and switched to it!
pause
goto Menu

:SwitchBranch
echo.
call :CheckSoftwareMethod git
if %errorlevel% neq 0 goto OperationFailed
echo Available branches:
git branch
echo.
set /p branch=Enter branch name to switch to: 
if "%branch%"=="" (
    echo Branch name cannot be empty!
    pause
    goto Menu
)
git checkout %branch%
if %errorlevel% neq 0 goto OperationFailed
echo.
echo Switched to branch '%branch%'!
pause
goto Menu

:MergeBranch
echo.
call :CheckSoftwareMethod git
if %errorlevel% neq 0 goto OperationFailed
echo Available branches:
git branch
echo.
set /p branch=Enter branch name to merge into current branch: 
if "%branch%"=="" (
    echo Branch name cannot be empty!
    pause
    goto Menu
)
git merge %branch%
if %errorlevel% neq 0 goto OperationFailed
echo.
echo Branch '%branch%' merged successfully!
pause
goto Menu

:ViewHistory
echo.
echo Showing commit history...
call :CheckSoftwareMethod git
if %errorlevel% neq 0 goto OperationFailed
git --no-pager log --oneline --graph --decorate -n 10
echo.
pause
goto Menu

:StashChanges
echo.
echo Stashing changes...
call :CheckSoftwareMethod git
if %errorlevel% neq 0 goto OperationFailed

echo.
echo WARNING: Stashing will temporarily set aside all your uncommitted changes.
echo          You can recover them later using the "Apply Stash" option.
echo.
choice /c YN /m "Are you sure you want to stash your changes? (Y/N)"
if %errorlevel% equ 2 (
    echo Operation cancelled.
    pause
    goto Menu
)

set /p message=Enter stash message (optional): 
if "%message%"=="" (
    git stash push -m "Stashed on %date% at %time%"
) else (
    git stash push -m "%message%"
)
if %errorlevel% neq 0 goto OperationFailed
echo.
echo Changes stashed successfully!
echo.
echo Use the "Apply Stash" option to recover these changes.
pause
goto Menu

:ApplyStash
echo.
echo Available stashes:
call :CheckSoftwareMethod git
if %errorlevel% neq 0 goto OperationFailed

REM Check if there are any stashes
git stash list > nul 2>&1
if %errorlevel% neq 0 (
    echo No stashes found.
    pause
    goto Menu
)

git stash list
echo.
echo Options:
echo 1. Apply the most recent stash (keep it in the stash list)
echo 2. Pop the most recent stash (apply and remove it from stash list)
echo 3. Apply a specific stash by index
echo 4. Pop a specific stash by index
echo 5. View stash contents before applying
echo 6. Back to Menu
echo.

set /p stashopt=Choose option (1-6): 
if "%stashopt%"=="" goto ApplyStash

if "%stashopt%"=="1" (
    git stash apply
    if %errorlevel% neq 0 (
        echo Failed to apply stash.
        pause
        goto Menu
    )
    echo Stash applied successfully! (Still available in stash list)
) else if "%stashopt%"=="2" (
    git stash pop
    if %errorlevel% neq 0 (
        echo Failed to pop stash.
        pause
        goto Menu
    )
    echo Stash popped successfully! (Removed from stash list)
) else if "%stashopt%"=="3" (
    set /p stashnum=Enter stash index (e.g., 0 for stash@{0}): 
    git stash apply stash@{%stashnum%}
    if %errorlevel% neq 0 (
        echo Failed to apply stash.
        pause
        goto Menu
    )
    echo Stash applied successfully! (Still available in stash list)
) else if "%stashopt%"=="4" (
    set /p stashnum=Enter stash index (e.g., 0 for stash@{0}): 
    git stash pop stash@{%stashnum%}
    if %errorlevel% neq 0 (
        echo Failed to pop stash.
        pause
        goto Menu
    )
    echo Stash popped successfully! (Removed from stash list)
) else if "%stashopt%"=="5" (
    set /p stashnum=Enter stash index to view (leave empty for most recent): 
    if "%stashnum%"=="" (
        git stash show -p
    ) else (
        git stash show -p stash@{%stashnum%}
    )
    echo.
    pause
    goto ApplyStash
) else if "%stashopt%"=="6" (
    goto Menu
) else (
    echo Invalid option.
    pause
    goto ApplyStash
)

pause
goto Menu

:OperationFailed
echo.
echo Operation failed with errors!
pause
goto Menu

@REM Method
:CheckSoftwareMethod
call check_software_eskit.bat %1
exit /b

:Exit
exit /b 0