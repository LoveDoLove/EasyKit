using EasyKit.Helpers.Console;
using EasyKit.Models;
using EasyKit.Services;
using EasyKit.UI.ConsoleUI;

namespace EasyKit.Controllers;

public class GitController
{
    private readonly ConfirmationHelper _confirmation;
    private readonly ConsoleService _console;
    private readonly NotificationView _notificationView;
    private readonly ProcessService _processService;
    private readonly PromptView _prompt;
    private readonly Software _software;

    public GitController(
        Software software,
        ConsoleService console,
        ConfirmationHelper confirmation,
        PromptView prompt,
        NotificationView notificationView)
    {
        _software = software;
        _console = console;
        _confirmation = confirmation;
        _prompt = prompt;
        _notificationView = notificationView;
        _processService = new ProcessService(console, console.Config);
    }

    // Helper to get the detected git path
    private string GetGitPath()
    {
        return _processService.FindExecutablePath("git") ?? "git";
    }

    /// <summary>
    ///     Waits for user input, used after most actions.
    /// </summary>
    private void WaitForUser()
    {
        _console.WriteInfo("Press Enter to continue...");
        Console.ReadLine();
    }

    public void RunDiagnostics()
    {
        _console.WriteInfo("===== GIT Configuration Diagnostics =====");
        _console.WriteInfo(
            "This will check your Git installation and repository status, and provide troubleshooting information.\n");

        // Step 1: Check if git is accessible in PATH
        _console.WriteInfo("Step 1: Checking if git is accessible in PATH");
        var (gitVersionOutput, gitVersionError, gitVersionExit) =
            _processService.RunProcessWithOutput(GetGitPath(), "--version", Environment.CurrentDirectory);
        if (gitVersionExit == 0 && !string.IsNullOrWhiteSpace(gitVersionOutput))
        {
            _console.WriteSuccess($"\u2713 Git is accessible. Version: {gitVersionOutput.Trim()}");
        }
        else
        {
            _console.WriteError("\u2717 Git is not accessible via PATH or detected path.");
            _console.WriteInfo("Please install Git from https://git-scm.com/downloads and ensure it is in your PATH.");
            _console.WriteInfo("You can also use the Tool Marketplace in EasyKit to open the download page.");
            _console.WriteInfo("\n===== End of GIT Configuration Diagnostics =====");
            WaitForUser();
            return;
        }

        // Step 2: Show detected git path
        var gitPath = GetGitPath();
        _console.WriteInfo($"Detected git path: {gitPath}");

        // Step 3: Check if current directory is a git repository
        _console.WriteInfo("\nStep 3: Checking if current directory is a git repository");
        if (File.Exists(".git/config"))
        {
            _console.WriteSuccess("\u2713 This is a git repository.");
        }
        else
        {
            _console.WriteError("\u2717 This is not a git repository.");
            _console.WriteInfo("You can initialize a repository with 'Init repository' in the menu or run 'git init'.");
            _console.WriteInfo("\n===== End of GIT Configuration Diagnostics =====");
            WaitForUser();
            return;
        }

        // Step 4: Run git status
        _console.WriteInfo("\nStep 4: Checking repository status (git status)");
        var (statusOutput, statusError, statusExit) = RunGitCommandWithOutput("status");
        if (!string.IsNullOrWhiteSpace(statusError))
            _console.WriteError(statusError.Trim());
        else if (!string.IsNullOrWhiteSpace(statusOutput))
            _console.WriteInfo(statusOutput.Trim());
        else
            _console.WriteInfo("No status output received from git.");

        // Step 5: Recommendations
        _console.WriteInfo("\nStep 5: Recommendations");
        _console.WriteInfo("If you encounter issues:");
        _console.WriteInfo("- Ensure Git is installed and in your PATH.");
        _console.WriteInfo("- Restart your terminal or EasyKit after installation.");
        _console.WriteInfo(
            "- For more help, visit https://git-scm.com/book/en/v2/Getting-Started-First-Time-Git-Setup");
        _console.WriteInfo(
            "- Use the Tool Marketplace in EasyKit to check installation status or open the download page.");

        _console.WriteInfo("\n===== End of GIT Configuration Diagnostics =====");
        WaitForUser();
    }

    public void ShowMenu()
    {
        // Get user settings
        int menuWidth = 100;

        // Try to get user preferences from config
        var menuWidthObj = _console.Config.Get("menu_width", 100);
        if (menuWidthObj is int mw)
            menuWidth = mw;

        // Create and configure the menu
        var menuView = new MenuView();
        menuView.CreateMenu("Git Tools", width: menuWidth)
            .AddOption("0", "Back to main menu", () =>
            {
                /* Return to main menu */
            })
            .AddOption("1", "Status", () => CheckStatus())
            .AddOption("2", "Init repository", () => InitRepo())
            .AddOption("3", "Add all changes", () => AddAll())
            .AddOption("4", "Commit staged changes", () => Commit())
            .AddOption("5", "Push to remote", () => Push())
            .AddOption("6", "Pull from remote", () => Pull())
            .AddOption("7", "View commit history", () => ViewHistory())
            .AddOption("8", "Create branch", () => CreateBranch())
            .AddOption("9", "Switch branch", () => SwitchBranch())
            .AddOption("10", "Merge branch", () => MergeBranch())
            .AddOption("11", "Stash changes", () => StashChanges())
            .AddOption("12", "Apply stash", () => ApplyStash())
            .AddOption("13", "Add submodule", () => AddSubmodule())
            .AddOption("14", "Update submodule", () => UpdateSubmodule())
            .AddOption("15", "Remove submodule", () => RemoveSubmodule())
            .AddOption("16", "Create pull request (info)", () => CreatePullRequest())
            .AddOption("17", "List pull requests (info)", () => ListPullRequests())
            .AddOption("18", "Run git diagnostics", () => RunDiagnostics())
            .WithColors(MenuTheme.ColorScheme.Dark.border, MenuTheme.ColorScheme.Dark.highlight,
                MenuTheme.ColorScheme.Dark.title, MenuTheme.ColorScheme.Dark.text, MenuTheme.ColorScheme.Dark.help)
            .WithHelpText("Select an option or press 0 to return to the main menu")
            .WithRoundedBorder()
            .Show();
    }

    private bool RunGitCommand(string args, bool showOutput = true)
    {
        if (!File.Exists(".git/config") && args != "init")
        {
            if (showOutput)
                _console.WriteError("This doesn't appear to be a git repository. Run 'git init' first.");
            return false;
        }

        return _processService.RunProcess(GetGitPath(), args, showOutput, Environment.CurrentDirectory);
    } // New helper to get output and error from git command

    private (string output, string error, int exitCode) RunGitCommandWithOutput(string args)
    {
        if (!File.Exists(".git/config") && args != "init")
            return ("", "This doesn't appear to be a git repository. Run 'git init' first.", 1);
        return _processService.RunProcessWithOutput(GetGitPath(), args, Environment.CurrentDirectory);
    }

    private void InitRepo()
    {
        _console.WriteInfo("Initializing git repository in current directory...");
        if (File.Exists(".git/config"))
        {
            _console.WriteInfo("Git repository already exists.");
            WaitForUser();
            return;
        }

        if (RunGitCommand("init"))
            _console.WriteSuccess("✓ Git repository initialized successfully!");
        else
            _console.WriteError("✗ Failed to initialize git repository.");
        WaitForUser();
    }

    private void CheckStatus()
    {
        _console.WriteInfo("Checking git status...");
        var (output, error, exitCode) = RunGitCommandWithOutput("status");

        if (!string.IsNullOrWhiteSpace(error))
        {
            _console.WriteError(error.Trim());
        }
        else if (string.IsNullOrWhiteSpace(output))
        {
            _console.WriteInfo("No status output received from git.");
        }
        else if (output.Contains("nothing to commit, working tree clean", StringComparison.OrdinalIgnoreCase))
        {
            _console.WriteSuccess("✓ Working tree clean. Nothing to commit.");
        }
        else
        {
            // Prevent invisible output: check for black-on-black
            var textColorObj = _console.Config?.Get("text_color", "");
            var bgColorObj = _console.Config?.Get("background_color", "");
            if (textColorObj?.ToString()?.ToLower() == "black" && bgColorObj?.ToString()?.ToLower() == "black")
                _console.WriteInfo(
                    "[Warning] Both text and background color are set to black. Please adjust your settings for visibility.");
            _console.WriteInfo(output?.Trim() ?? "");
        }

        WaitForUser();
    }

    private void AddAll()
    {
        _console.WriteInfo("Adding all changes to staging area...");
        if (RunGitCommand("add ."))
            _console.WriteSuccess("✓ Changes added to staging area!");
        else
            _console.WriteError("✗ Failed to add changes.");
        WaitForUser();
    }

    private void Commit()
    {
        var message = _prompt.Prompt("Enter commit message: ") ?? "";
        if (string.IsNullOrWhiteSpace(message))
        {
            _console.WriteError("Commit message cannot be empty.");
            WaitForUser();
            return;
        }

        _console.WriteInfo("Committing changes...");
        if (RunGitCommand($"commit -m \"{message}\""))
            _console.WriteSuccess("✓ Changes committed successfully!");
        else
            _console.WriteError("✗ Failed to commit changes.");
        WaitForUser();
    }

    private void Push()
    {
        string remoteBranch = _prompt.Prompt("Enter remote and branch (e.g. 'origin main'): ") ?? "origin main";
        if (string.IsNullOrWhiteSpace(remoteBranch))
            remoteBranch = "origin main";

        _console.WriteInfo($"Pushing to {remoteBranch}...");
        var parts = remoteBranch.Split(' ');
        string remote = parts.Length > 0 ? parts[0] : "origin";
        string branch = parts.Length > 1 ? parts[1] : "main";
        if (RunGitCommand($"push {remote} {branch}"))
            _console.WriteSuccess($"✓ Changes pushed to {remote}/{branch} successfully!");
        else
            _console.WriteError($"✗ Failed to push changes to {remote}/{branch}.");
        WaitForUser();
    }

    private void Pull()
    {
        string remoteBranch = _prompt.Prompt("Enter remote and branch (e.g. 'origin main'): ") ?? "origin main";
        if (string.IsNullOrWhiteSpace(remoteBranch))
            remoteBranch = "origin main";

        _console.WriteInfo($"Pulling from {remoteBranch}...");
        var parts = remoteBranch.Split(' ');
        string remote = parts.Length > 0 ? parts[0] : "origin";
        string branch = parts.Length > 1 ? parts[1] : "main";
        if (RunGitCommand($"pull {remote} {branch}"))
            _console.WriteSuccess($"✓ Changes pulled from {remote}/{branch} successfully!");
        else
            _console.WriteError($"✗ Failed to pull changes from {remote}/{branch}.");
        WaitForUser();
    }

    private void CreateBranch()
    {
        var branchName = _prompt.Prompt("Enter new branch name: ") ?? "";
        if (string.IsNullOrWhiteSpace(branchName))
        {
            _console.WriteError("Branch name cannot be empty.");
            Console.ReadLine();
            return;
        }

        _console.WriteInfo($"Creating branch '{branchName}'...");
        if (RunGitCommand($"checkout -b {branchName}"))
            _console.WriteSuccess($"✓ Branch '{branchName}' created and checked out successfully!");
        else
            _console.WriteError($"✗ Failed to create branch '{branchName}'.");
        WaitForUser();
    }

    private void SwitchBranch()
    {
        _console.WriteInfo("Available branches:");
        RunGitCommand("branch");
        var branchName = _prompt.Prompt("Enter branch name to switch to: ") ?? "";
        if (string.IsNullOrWhiteSpace(branchName))
        {
            _console.WriteError("Branch name cannot be empty.");
            Console.ReadLine();
            return;
        }

        _console.WriteInfo($"Switching to branch '{branchName}'...");
        if (RunGitCommand($"checkout {branchName}"))
            _console.WriteSuccess($"✓ Switched to branch '{branchName}' successfully!");
        else
            _console.WriteError($"✗ Failed to switch to branch '{branchName}'.");
        WaitForUser();
    }

    private void MergeBranch()
    {
        _console.WriteInfo("Available branches:");
        RunGitCommand("branch");
        var branchName = _prompt.Prompt("Enter branch name to merge into current branch: ") ?? "";
        if (string.IsNullOrWhiteSpace(branchName))
        {
            _console.WriteError("Branch name cannot be empty.");
            Console.ReadLine();
            return;
        }

        _console.WriteInfo($"Merging branch '{branchName}' into current branch...");
        if (RunGitCommand($"merge {branchName}"))
            _console.WriteSuccess($"✓ Branch '{branchName}' merged successfully!");
        else
            _console.WriteError($"✗ Failed to merge branch '{branchName}'.");
        WaitForUser();
    }

    private void ViewHistory()
    {
        _console.WriteInfo("Viewing commit history...");
        RunGitCommand("log --pretty=format:'%h %ad | %s%d [%an]' --graph --date=short");
        WaitForUser();
    }

    private void StashChanges()
    {
        var message = _prompt.Prompt("Enter stash message (optional): ") ?? "";
        var command = string.IsNullOrWhiteSpace(message) ? "stash" : $"stash push -m \"{message}\"";
        _console.WriteInfo("Stashing changes...");
        if (RunGitCommand(command))
            _console.WriteSuccess("✓ Changes stashed successfully!");
        else
            _console.WriteError("✗ Failed to stash changes.");
        WaitForUser();
    }

    private void ApplyStash()
    {
        _console.WriteInfo("Available stashes:");
        RunGitCommand("stash list");
        var stashId = _prompt.Prompt("Enter stash to apply (e.g. 'stash@{0}', leave empty for latest): ") ?? "";
        var command = string.IsNullOrWhiteSpace(stashId) ? "stash apply" : $"stash apply {stashId}";
        _console.WriteInfo("Applying stash...");
        if (RunGitCommand(command))
            _console.WriteSuccess("✓ Stash applied successfully!");
        else
            _console.WriteError("✗ Failed to apply stash.");
        WaitForUser();
    }

    private void CreatePullRequest()
    {
        _console.WriteInfo("Creating a pull request is typically done through the web interface of your Git hosting " +
                           "provider (GitHub, GitLab, Bitbucket, etc.).\n\n" +
                           "General steps:\n" +
                           "1. Push your branch to the remote repository\n" +
                           "2. Visit your Git provider's website\n" +
                           "3. Navigate to your repository\n" +
                           "4. Look for a 'New Pull Request' or 'Create Pull Request' button\n" +
                           "5. Select the base branch and your feature branch\n" +
                           "6. Fill in the title and description\n" +
                           "7. Create the pull request\n\n" +
                           "For GitHub, you can also use the GitHub CLI tool if installed:\n" +
                           "gh pr create --title \"Your PR title\" --body \"Description of changes\"\n\n" +
                           "Press Enter to continue.");
        WaitForUser();
    }

    private void ListPullRequests()
    {
        _console.WriteInfo("Listing pull requests is typically done through the web interface of your Git hosting " +
                           "provider (GitHub, GitLab, Bitbucket, etc.).\n\n" +
                           "General steps:\n" +
                           "1. Visit your Git provider's website\n" +
                           "2. Navigate to your repository\n" +
                           "3. Look for a 'Pull Requests' or 'Merge Requests' tab\n\n" +
                           "For GitHub, you can also use the GitHub CLI tool if installed:\n" +
                           "gh pr list\n\n" +
                           "Press Enter to continue.");
        WaitForUser();
    }

    private void AddSubmodule()
    {
        // Get the submodule URL
        var url = _prompt.Prompt("Enter submodule URL: ") ?? "";
        if (string.IsNullOrWhiteSpace(url))
        {
            _console.WriteError("Submodule URL cannot be empty.");
            WaitForUser();
            return;
        }

        // Get the submodule path
        var path = _prompt.Prompt("Enter submodule path: ") ?? "";
        if (string.IsNullOrWhiteSpace(path))
        {
            _console.WriteError("Submodule path cannot be empty.");
            WaitForUser();
            return;
        }        // Check if the path already exists as a directory
        if (Directory.Exists(path))
        {
            if (!_confirmation.ConfirmAction($"Directory '{path}' already exists. Continue anyway?"))
            {
                _console.WriteInfo("Submodule addition cancelled.");
                WaitForUser();
                return;
            }
        }

        // Check if the path already exists in .gitmodules
        if (File.Exists(".gitmodules"))
        {
            try
            {
                string gitmodulesContent = File.ReadAllText(".gitmodules");
                if (gitmodulesContent.Contains($"path = {path}"))
                {
                    _console.WriteError($"A submodule already exists at path '{path}'. Use 'Update submodule' to update it.");
                    WaitForUser();
                    return;
                }
            }
            catch (Exception ex)
            {
                _console.WriteError($"Error checking .gitmodules file: {ex.Message}");
                // Continue with the operation, as the check is precautionary
            }
        }

        _console.WriteInfo($"Adding submodule from '{url}' to '{path}'...");

        // Run the git submodule add command and capture detailed output
        var (output, error, exitCode) = RunGitCommandWithOutput($"submodule add {url} {path}");

        if (exitCode == 0)
        {
            _console.WriteSuccess("✓ Submodule added successfully!");

            // Additional information about the submodule
            _console.WriteInfo("\nSubmodule Information:");
            RunGitCommand("submodule status", true);

            // Inform user about initialization and updating
            _console.WriteInfo("\nYou can update this submodule later using 'Update submodule' option.");
        }
        else
        {
            _console.WriteError($"✗ Failed to add submodule. Error details:");
            if (!string.IsNullOrWhiteSpace(error))
            {
                _console.WriteError(error);

                // Provide helpful guidance based on common error patterns
                if (error.Contains("already exists") && error.Contains("is not a valid repository"))
                {
                    _console.WriteInfo("Suggestion: The directory may already exist but is not a valid repository.");
                    _console.WriteInfo("Try removing the directory first or choosing a different path.");
                }
                else if (error.Contains("remote: Repository not found"))
                {
                    _console.WriteInfo("Suggestion: The repository URL may be incorrect or you might not have access to it.");
                    _console.WriteInfo("Verify the URL and ensure you have proper access permissions.");
                }
                else if (error.Contains("Permission denied"))
                {
                    _console.WriteInfo("Suggestion: You may not have permission to access this repository.");
                    _console.WriteInfo("Check your credentials or try using HTTPS instead of SSH or vice versa.");
                }
            }
            else if (!string.IsNullOrWhiteSpace(output))
            {
                _console.WriteInfo(output);
            }
        }

        WaitForUser();
    }

    private void UpdateSubmodule()
    {
        // Check if there are any submodules first
        var (statusOutput, statusError, statusExitCode) = RunGitCommandWithOutput("submodule status");
        if (statusExitCode != 0 || string.IsNullOrWhiteSpace(statusOutput))
        {
            _console.WriteError("No submodules found in this repository.");
            if (!string.IsNullOrWhiteSpace(statusError))
            {
                _console.WriteError("Error details: " + statusError);
            }
            WaitForUser();
            return;
        }

        // Show available submodules
        _console.WriteInfo("Available submodules:");
        RunGitCommand("submodule status");

        // Ask if user wants to update a specific submodule or all submodules
        var specificPath = _prompt.Prompt("Enter submodule path to update (leave empty to update all): ") ?? "";
        
        string updateCommand;
        string operationDescription;
        
        if (string.IsNullOrWhiteSpace(specificPath))
        {
            // Update all submodules
            updateCommand = "submodule update --remote --init --recursive";
            operationDescription = "all submodules";
        }
        else
        {
            // Check if the specified submodule exists
            bool submoduleExists = false;
            var submodules = statusOutput.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var submodule in submodules)
            {
                if (submodule.Contains(specificPath))
                {
                    submoduleExists = true;
                    break;
                }
            }
            
            if (!submoduleExists)
            {
                _console.WriteError($"No submodule found at path '{specificPath}'.");
                WaitForUser();
                return;
            }
            
            // Update specific submodule
            updateCommand = $"submodule update --remote --init --recursive {specificPath}";
            operationDescription = $"submodule at '{specificPath}'";
        }
        
        _console.WriteInfo($"Updating {operationDescription} to the latest remote commits...");

        // Run the update command and capture detailed output
        var (output, error, exitCode) = RunGitCommandWithOutput(updateCommand);
        
        if (exitCode == 0)
        {
            _console.WriteSuccess($"✓ {operationDescription} updated successfully!");
            
            // Show updated submodule status
            _console.WriteInfo("\nCurrent submodule status:");
            RunGitCommand("submodule status");
            
            // Inform user about additional steps if needed
            _console.WriteInfo("\nNote: The updates have been fetched, but not yet committed.");
            _console.WriteInfo("If you want to commit these changes, use 'Commit staged changes' option.");
        }
        else
        {
            _console.WriteError($"✗ Failed to update {operationDescription}. Error details:");
            
            if (!string.IsNullOrWhiteSpace(error))
            {
                _console.WriteError(error);
                
                // Provide helpful guidance based on common error patterns
                if (error.Contains("Please make sure you have the correct access rights"))
                {
                    _console.WriteInfo("Suggestion: Access rights issue. Check your credentials for the submodule repository.");
                }
                else if (error.Contains("did not match any file(s) known to git"))
                {
                    _console.WriteInfo("Suggestion: The submodule path may be incorrect or not initialized.");
                    _console.WriteInfo("Try using the 'Init repository' option first, or check the path name.");
                }
                else if (error.Contains("fatal: Unable to update"))
                {
                    _console.WriteInfo("Suggestion: There may be uncommitted changes in the submodule.");
                    _console.WriteInfo("Commit or stash changes in the submodule before updating.");
                }
            }
            else if (!string.IsNullOrWhiteSpace(output))
            {
                _console.WriteInfo(output);
            }
        }
        
        WaitForUser();
    }

    private void RemoveSubmodule()
    {
        // Check if there are any submodules first
        var (statusOutput, statusError, statusExitCode) = RunGitCommandWithOutput("submodule status");
        if (statusExitCode != 0 || string.IsNullOrWhiteSpace(statusOutput))
        {
            _console.WriteError("No submodules found in this repository.");
            if (!string.IsNullOrWhiteSpace(statusError))
            {
                _console.WriteError("Error details: " + statusError);
            }
            WaitForUser();
            return;
        }

        // Show available submodules
        _console.WriteInfo("Available submodules:");
        RunGitCommand("submodule status");

        // Get the submodule path to remove
        var path = _prompt.Prompt("Enter submodule path to remove: ") ?? "";
        if (string.IsNullOrWhiteSpace(path))
        {
            _console.WriteError("Submodule path cannot be empty.");
            WaitForUser();
            return;
        }

        // Check if specified submodule exists
        bool submoduleExists = false;
        var submodules = statusOutput.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        foreach (var submodule in submodules)
        {
            if (submodule.Contains(path))
            {
                submoduleExists = true;
                break;
            }
        }
        
        if (!submoduleExists)
        {
            _console.WriteError($"No submodule found at path '{path}'.");
            WaitForUser();
            return;
        }

        // Confirm removal
        if (!_confirmation.ConfirmAction($"Are you sure you want to remove submodule at '{path}'?", false))
        {
            _console.WriteInfo("Submodule removal cancelled.");
            WaitForUser();
            return;
        }

        _console.WriteInfo($"Removing submodule '{path}'...");

        bool success = true;
        string errorDetails = "";

        // 1. Deinitialize the submodule
        _console.WriteInfo("Step 1/4: Deinitializing submodule...");
        var (output1, error1, exitCode1) = RunGitCommandWithOutput($"submodule deinit -f {path}");
        if (exitCode1 != 0)
        {
            success = false;
            errorDetails += "Error deinitializing submodule:\n" + error1 + "\n";
        }

        // 2. Remove submodule from index (tracked files in .git)
        _console.WriteInfo("Step 2/4: Removing from Git index...");
        var (output2, error2, exitCode2) = RunGitCommandWithOutput($"rm -f {path}");
        if (exitCode2 != 0)
        {
            success = false;
            errorDetails += "Error removing from Git index:\n" + error2 + "\n";
        }

        // 3. Remove the submodule directory from .git/modules
        _console.WriteInfo("Step 3/4: Cleaning up .git/modules...");
        // Use cross-platform approach to remove directory
        string gitModulesPath = Path.Combine(".git", "modules", path);
        try
        {
            if (Directory.Exists(gitModulesPath))
            {
                Directory.Delete(gitModulesPath, true); // true for recursive
            }
        }
        catch (Exception ex)
        {
            success = false;
            errorDetails += $"Error removing .git/modules/{path}: {ex.Message}\n";
        }

        // 4. Commit the changes
        if (success)
        {
            _console.WriteInfo("Step 4/4: Committing changes...");
            var commitMessage = $"Remove submodule {path}";
            var (output3, error3, exitCode3) = RunGitCommandWithOutput($"commit -m \"{commitMessage}\"");
            if (exitCode3 != 0)
            {
                // This might happen if there's nothing staged - don't treat as complete failure
                _console.WriteInfo("Note: No changes were committed. This is normal if the submodule was already removed.");
                _console.WriteInfo("You may need to manually commit these changes.");
            }
        }

        // Output results
        if (success)
        {
            _console.WriteSuccess($"✓ Submodule '{path}' removed successfully!");
            
            // Show updated submodule status
            _console.WriteInfo("\nCurrent submodule status:");
            RunGitCommand("submodule status");
        }
        else
        {
            _console.WriteError($"✗ There were errors removing submodule '{path}':");
            _console.WriteError(errorDetails);
            _console.WriteInfo("\nYou may need to manually complete the removal process.");
            
            // Provide recovery guidance
            _console.WriteInfo("Recovery steps:");
            _console.WriteInfo("1. Check if the submodule still exists: git submodule status");
            _console.WriteInfo("2. If needed, manually edit the .gitmodules file to remove the entry");
            _console.WriteInfo("3. Run: git add .gitmodules");
            _console.WriteInfo("4. Commit the changes: git commit -m \"Remove submodule " + path + "\"");
        }
        
        WaitForUser();
    }
}