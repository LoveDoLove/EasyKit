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

:: Verify GitHub CLI is installed and working
echo [INFO] Checking for GitHub CLI...
call :CheckSoftwareMethod gh
if %errorlevel% neq 0 (
    echo [ERROR] GitHub CLI (gh) is required for pull request operations but installation failed.
    echo [INFO] You may need to run this script as administrator to install GitHub CLI.
    echo [INFO] Alternatively, you can install GitHub CLI manually from: https://cli.github.com/
    pause
    goto Menu
)

:: Check if we're in a git repository
git rev-parse --is-inside-work-tree >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] Not inside a git repository.
    pause
    goto Menu
)

:: Get current branch
for /f "tokens=*" %%a in ('git branch --show-current') do set "current_branch=%%a"
echo Current branch status:
git status -sb

echo.
echo [INFO] Will create a pull request from branch: %current_branch%
echo.

:: Check for uncommitted changes
git diff-index --quiet HEAD -- >nul 2>&1
if %errorlevel% neq 0 (
    echo [WARN] You have uncommitted changes. Consider committing before creating a PR.
    choice /c YNC /m "Continue anyway? (Y)es/(N)o/(C)ommit changes first"
    if !errorlevel! equ 2 (
        echo Operation cancelled.
        pause
        goto Menu
    )
    if !errorlevel! equ 3 (
        goto Commit
    )
)

:: Push current branch if it's not already pushed
git ls-remote --exit-code origin %current_branch% >nul 2>&1
if %errorlevel% neq 0 (
    echo [INFO] Current branch is not yet pushed to remote.
    choice /c YN /m "Push branch %current_branch% to remote before creating PR? (Y/N)"
    if !errorlevel! equ 1 (
        echo Pushing branch to remote...
        git push -u origin %current_branch%
        if !errorlevel! neq 0 (
            echo [ERROR] Failed to push branch.
            pause
            goto Menu
        )
    ) else (
        echo [WARN] Creating PR without pushing may fail if the branch doesn't exist remotely.
    )
)

:: Create the PR
echo.
set /p title=Enter PR title: 
if "%title%"=="" (
    echo [WARN] Title cannot be empty.
    pause
    goto CreatePullRequest
)

set /p body=Enter PR description (optional): 
set /p base=Enter target branch (leave empty for default branch): 

if "%base%"=="" (
    if "%body%"=="" (
        gh pr create --title "%title%"
    ) else (
        gh pr create --title "%title%" --body "%body%"
    )
) else (
    if "%body%"=="" (
        gh pr create --title "%title%" --base "%base%"
    ) else (
        gh pr create --title "%title%" --body "%body%" --base "%base%"
    )
)

if %errorlevel% neq 0 (
    echo [ERROR] Failed to create pull request.
    pause
    goto Menu
)

echo.
echo [OK] Pull request created successfully!
pause
goto Menu

:ListPullRequests
echo.
echo Listing pull requests...
call :CheckSoftwareMethod git
if %errorlevel% neq 0 goto OperationFailed

:: Verify GitHub CLI is installed and working
echo [INFO] Checking for GitHub CLI...
call :CheckSoftwareMethod gh
if %errorlevel% neq 0 (
    echo [ERROR] GitHub CLI (gh) is required for pull request operations but installation failed.
    echo [INFO] You may need to run this script as administrator to install GitHub CLI.
    echo [INFO] Alternatively, you can install GitHub CLI manually from: https://cli.github.com/
    pause
    goto Menu
)

:: Check if we're in a git repository
git rev-parse --is-inside-work-tree >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] Not inside a git repository.
    pause
    goto Menu
)

echo.
echo Pull Request Options:
echo 1. List open pull requests
echo 2. List your pull requests
echo 3. View a specific pull request
echo 4. Check out a pull request branch
echo 5. Back to main menu
echo.

set /p proption=Choose an option (1-5): 
if "%proption%"=="" goto ListPullRequests

if "%proption%"=="1" (
    echo.
    echo Listing all open pull requests:
    gh pr list
) else if "%proption%"=="2" (
    echo.
    echo Listing your pull requests:
    gh pr list --author "@me"
) else if "%proption%"=="3" (
    echo.
    set /p prnumber=Enter PR number to view: 
    if "%prnumber%"=="" goto ListPullRequests
    gh pr view %prnumber%
    
    echo.
    choice /c YN /m "Add a comment to this PR? (Y/N)"
    if !errorlevel! equ 1 (
        set /p comment=Enter your comment: 
        gh pr comment %prnumber% --body "%comment%"
    )
) else if "%proption%"=="4" (
    echo.
    set /p prnumber=Enter PR number to check out: 
    if "%prnumber%"=="" goto ListPullRequests
    
    echo [WARN] This will switch your current branch to the PR branch.
    choice /c YN /m "Continue? (Y/N)"
    if !errorlevel! equ 2 (
        goto ListPullRequests
    )
    
    gh pr checkout %prnumber%
) else if "%proption%"=="5" (
    goto Menu
) else (
    echo Invalid option.
    pause
    goto ListPullRequests
)

pause
goto ListPullRequests

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