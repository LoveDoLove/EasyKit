@echo off
color 0A
title Git Menu
setlocal enabledelayedexpansion

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
echo 13. Create Pull Request
echo 14. List Pull Requests
echo.
set choice=
set /p choice=Choose an option: 
if not defined choice goto InvalidChoice
set "numbers=0123456789"
set "valid=true"
for /L %%i in (0,1,9) do if "!choice:~%%i,1!" NEQ "" (
    if "!numbers:!choice:~%%i,1!=!" EQU "%numbers%" set "valid=false"
)
if "!valid!" EQU "false" goto InvalidChoice
if %choice% LSS 0 goto InvalidChoice
if %choice% GTR 14 goto InvalidChoice

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
if %choice%==13 goto CreatePullRequest
if %choice%==14 goto ListPullRequests
goto Menu

:InvalidChoice
echo.
echo Invalid option. Please try again.
timeout /t 2 >nul
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
echo Staging all changes...
git add .
if %errorlevel% neq 0 goto OperationFailed
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
set /p message=Enter stash message (optional): 
if "%message%"=="" (
    git stash
) else (
    git stash save "%message%"
)
if %errorlevel% neq 0 goto OperationFailed
echo.
echo Changes stashed successfully!
pause
goto Menu

:ApplyStash
echo.
echo Available stashes:
call :CheckSoftwareMethod git
if %errorlevel% neq 0 goto OperationFailed
git stash list
echo.
set /p stashnum=Enter stash number to apply (leave empty for most recent): 
if "%stashnum%"=="" (
    git stash apply
) else (
    git stash apply stash@{%stashnum%}
)
if %errorlevel% neq 0 goto OperationFailed
echo.
echo Stash applied successfully!
pause
goto Menu

:CreatePullRequest
echo.
echo Creating a pull request...
call :CheckSoftwareMethod git
if %errorlevel% neq 0 goto OperationFailed

call :CheckSoftwareMethod gh
if %errorlevel% neq 0 goto OperationFailed

REM Check if we're in a git repository
git rev-parse --is-inside-work-tree >nul 2>&1
if %errorlevel% neq 0 (
    echo You must be inside a git repository to create a pull request.
    pause
    goto Menu
)

REM Push current branch if needed
echo Current branch status:
git status -sb
echo.
choice /c YN /m "Do you want to push your current branch before creating PR? (Y/N)"
if %errorlevel% equ 1 (
    git push --set-upstream origin HEAD
    if %errorlevel% neq 0 (
        echo Failed to push changes.
        pause
        goto Menu
    )
)

echo.
echo Creating pull request...
set /p title=Enter PR title: 
if "%title%"=="" (
    echo Title cannot be empty!
    pause
    goto Menu
)

set /p body=Enter PR description (optional): 

if "%body%"=="" (
    gh pr create --title "%title%"
) else (
    gh pr create --title "%title%" --body "%body%"
)

if %errorlevel% neq 0 (
    echo Failed to create pull request. 
    echo Make sure your branch is pushed to GitHub and the repository is configured correctly.
    pause
    goto Menu
)

echo.
echo Pull request created successfully!
pause
goto Menu

:ListPullRequests
echo.
echo Listing pull requests...
call :CheckSoftwareMethod git
if %errorlevel% neq 0 goto OperationFailed

call :CheckSoftwareMethod gh
if %errorlevel% neq 0 goto OperationFailed

REM Check if we're in a git repository
git rev-parse --is-inside-work-tree >nul 2>&1
if %errorlevel% neq 0 (
    echo You must be inside a git repository to list pull requests.
    pause
    goto Menu
)

echo.
echo Pull requests for current repository:
gh pr list
if %errorlevel% neq 0 (
    echo Failed to list pull requests.
    echo Make sure you're in a git repository connected to GitHub.
    pause
    goto Menu
)

echo.
set /p prnumber=Enter PR number to view details (leave empty to go back): 
if "%prnumber%"=="" goto Menu

echo.
echo Pull request details:
gh pr view %prnumber%
if %errorlevel% neq 0 (
    echo Failed to view pull request details.
    pause
    goto Menu
)
echo.

choice /c YNC /m "Do you want to check out this PR (Y), comment on it (C), or go back (N)? "
if errorlevel 3 (
    set /p comment=Enter your comment: 
    gh pr comment %prnumber% --body "%comment%"
    if %errorlevel% neq 0 (
        echo Failed to add comment.
    ) else (
        echo Comment added successfully.
    )
) else if errorlevel 2 (
    rem Do nothing and return
) else if errorlevel 1 (
    gh pr checkout %prnumber%
    if %errorlevel% neq 0 (
        echo Failed to checkout PR.
    ) else (
        echo PR %prnumber% checked out successfully.
    )
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