// MIT License
// 
// Copyright (c) 2025 LoveDoLove
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Text;
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
        var url = _prompt.Prompt("Enter submodule URL: ") ?? "";
        if (string.IsNullOrWhiteSpace(url))
        {
            _console.WriteError("Submodule URL cannot be empty.");
            WaitForUser();
            return;
        }

        var path = _prompt.Prompt("Enter submodule path: ") ?? "";
        if (string.IsNullOrWhiteSpace(path))
        {
            _console.WriteError("Submodule path cannot be empty.");
            WaitForUser();
            return;
        }

        // Check if the directory exists before attempting to add the submodule
        bool directoryExists = Directory.Exists(path);
        bool isGitRepo = false;
        bool isEmpty = true;

        if (directoryExists)
        {
            // Check if the directory is a git repository
            isGitRepo = Directory.Exists(Path.Combine(path, ".git"));

            // Check if the directory is empty
            isEmpty = !Directory.EnumerateFileSystemEntries(path).Any();

            if (isGitRepo)
            {
                _console.WriteInfo($"[Warning] Directory '{path}' already exists and contains a Git repository.");
                _console.WriteInfo("Options:");
                _console.WriteInfo("1. Remove the directory and create submodule");
                _console.WriteInfo("2. Force add submodule (--force option)");
                _console.WriteInfo("3. Cancel operation");

                var choice = _prompt.Prompt("Enter your choice (1-3): ");

                switch (choice)
                {
                    case "1":
                        try
                        {
                            _console.WriteInfo($"Removing directory '{path}'...");
                            Directory.Delete(path, true);
                            directoryExists = false;
                            _console.WriteSuccess($"Directory '{path}' removed successfully.");
                        }
                        catch (Exception ex)
                        {
                            _console.WriteError($"Failed to remove directory: {ex.Message}");
                            _console.WriteInfo("Please remove the directory manually and try again.");
                            WaitForUser();
                            return;
                        }

                        break;
                    case "2":
                        _console.WriteInfo(
                            "[Warning] Proceeding with force add. This may still fail if there are conflicts.");
                        url = "--force " + url; // Add the force flag to the command
                        break;
                    default:
                        _console.WriteInfo("Submodule addition cancelled.");
                        WaitForUser();
                        return;
                }
            }
            else if (!isEmpty)
            {
                _console.WriteInfo($"[Warning] Directory '{path}' already exists and contains files.");
                _console.WriteInfo("Options:");
                _console.WriteInfo("1. Remove the directory and create submodule");
                _console.WriteInfo("2. Force add submodule (may fail if directory is not empty)");
                _console.WriteInfo("3. Cancel operation");

                var choice = _prompt.Prompt("Enter your choice (1-3): ");

                switch (choice)
                {
                    case "1":
                        try
                        {
                            _console.WriteInfo($"Removing directory '{path}'...");
                            Directory.Delete(path, true);
                            directoryExists = false;
                            _console.WriteSuccess($"Directory '{path}' removed successfully.");
                        }
                        catch (Exception ex)
                        {
                            _console.WriteError($"Failed to remove directory: {ex.Message}");
                            _console.WriteInfo("Please remove the directory manually and try again.");
                            WaitForUser();
                            return;
                        }

                        break;
                    case "2":
                        _console.WriteInfo(
                            "[Warning] Proceeding with force add. This may fail if the directory is not empty.");
                        break;
                    default:
                        _console.WriteInfo("Submodule addition cancelled.");
                        WaitForUser();
                        return;
                }
            }
            else if (!_confirmation.ConfirmAction($"Directory '{path}' already exists but is empty. Continue anyway?"))
            {
                _console.WriteInfo("Submodule addition cancelled.");
                WaitForUser();
                return;
            }
        }

        _console.WriteInfo($"Adding submodule from '{url}' to '{path}'...");

        // Run the git submodule add command and capture detailed output
        string addCommand = url.StartsWith("--force") ? $"submodule add {url} {path}" : $"submodule add {url} {path}";
        var (output, error, exitCode) = RunGitCommandWithOutput(addCommand);

        if (exitCode == 0)
        {
            _console.WriteSuccess("✓ Submodule added successfully!");

            // Additional information about the submodule
            _console.WriteInfo("\nSubmodule Information:");
            RunGitCommand("submodule status");

            // Inform user about initialization and updating
            _console.WriteInfo("\nYou can update this submodule later using 'Update submodule' option.");
        }
        else
        {
            _console.WriteError("✗ Failed to add submodule. Error details:");
            if (!string.IsNullOrWhiteSpace(error))
            {
                _console.WriteError(error);

                // Provide helpful guidance based on common error patterns
                if (error.Contains("already exists") && error.Contains("is not a valid repository"))
                {
                    _console.WriteInfo("Suggestion: The directory exists but is not a valid repository.");

                    if (_confirmation.ConfirmAction("Do you want to remove the directory and try again?"))
                        try
                        {
                            _console.WriteInfo($"Removing directory '{path}'...");
                            Directory.Delete(path, true);
                            _console.WriteSuccess($"Directory '{path}' removed successfully.");

                            // Try adding the submodule again
                            _console.WriteInfo($"Retrying: Adding submodule from '{url}' to '{path}'...");
                            var (retryOutput, retryError, retryExitCode) =
                                RunGitCommandWithOutput($"submodule add {url} {path}");

                            if (retryExitCode == 0)
                            {
                                _console.WriteSuccess("✓ Submodule added successfully on retry!");

                                // Additional information about the submodule
                                _console.WriteInfo("\nSubmodule Information:");
                                RunGitCommand("submodule status");

                                // Inform user about initialization and updating
                                _console.WriteInfo(
                                    "\nYou can update this submodule later using 'Update submodule' option.");
                                WaitForUser();
                                return;
                            }

                            _console.WriteError("✗ Failed to add submodule on retry. Error details:");
                            _console.WriteError(retryError);
                        }
                        catch (Exception ex)
                        {
                            _console.WriteError($"Failed to remove directory: {ex.Message}");
                            _console.WriteInfo("Please remove the directory manually and try again.");
                        }
                    else
                        _console.WriteInfo("You can manually remove the directory and try again.");
                }
                else if (error.Contains("remote: Repository not found"))
                {
                    _console.WriteInfo(
                        "Suggestion: The repository URL may be incorrect or you might not have access to it.");
                    _console.WriteInfo("Verify the URL and ensure you have proper access permissions.");
                }
                else if (error.Contains("Permission denied"))
                {
                    _console.WriteInfo("Suggestion: You may not have permission to access this repository.");
                    _console.WriteInfo("Check your credentials or try using HTTPS instead of SSH or vice versa.");
                }
                else if (error.Contains("already exists in the index"))
                {
                    _console.WriteInfo("Suggestion: Git is tracking this path already.");
                    _console.WriteInfo(
                        "If you want to replace it with a submodule, first remove it from git tracking with 'git rm -r --cached " +
                        path + "'");
                }
                else if (error.Contains("A git directory for") && error.Contains("is found locally"))
                {
                    _console.WriteInfo("Suggestion: There is an existing git repository at this location.");
                    _console.WriteInfo("You can use the '--force' option to reuse this local git directory.");

                    // Give option to try with force
                    if (_confirmation.ConfirmAction("Would you like to retry with the --force option?"))
                    {
                        _console.WriteInfo("Retrying with --force option...");
                        var (forceOutput, forceError, forceExitCode) =
                            RunGitCommandWithOutput($"submodule add --force {url} {path}");

                        if (forceExitCode == 0)
                        {
                            _console.WriteSuccess("✓ Submodule added successfully with --force option!");
                            _console.WriteInfo("\nSubmodule Information:");
                            RunGitCommand("submodule status");
                            _console.WriteInfo(
                                "\nYou can update this submodule later using 'Update submodule' option.");
                        }
                        else
                        {
                            _console.WriteError("✗ Failed to add submodule even with --force. Error details:");
                            _console.WriteError(forceError);
                        }
                    }
                }
                else if (error.Contains("not a git repository"))
                {
                    _console.WriteInfo("Suggestion: The directory '.git/modules/' is not a valid git repository.");
                    _console.WriteInfo("This could be due to a previous failed submodule operation.");

                    if (_confirmation.ConfirmAction("Would you like to try cleaning up and retrying?"))
                        try
                        {
                            string gitModulesPath = Path.Combine(".git", "modules", path);
                            if (Directory.Exists(gitModulesPath))
                            {
                                _console.WriteInfo($"Removing directory '{gitModulesPath}'...");
                                Directory.Delete(gitModulesPath, true);
                                _console.WriteSuccess($"Directory '{gitModulesPath}' removed.");
                            }

                            // Try adding the submodule again
                            _console.WriteInfo($"Retrying: Adding submodule from '{url}' to '{path}'...");
                            var (retryOutput, retryError, retryExitCode) =
                                RunGitCommandWithOutput($"submodule add --force {url} {path}");

                            if (retryExitCode == 0)
                            {
                                _console.WriteSuccess("✓ Submodule added successfully on retry!");
                                _console.WriteInfo("\nSubmodule Information:");
                                RunGitCommand("submodule status");
                                _console.WriteInfo(
                                    "\nYou can update this submodule later using 'Update submodule' option.");
                                WaitForUser();
                                return;
                            }

                            _console.WriteError("✗ Failed to add submodule on retry. Error details:");
                            _console.WriteError(retryError);
                        }
                        catch (Exception ex)
                        {
                            _console.WriteError($"Error during cleanup: {ex.Message}");
                        }
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
            if (!string.IsNullOrWhiteSpace(statusError)) _console.WriteError("Error details: " + statusError);
            WaitForUser();
            return;
        }

        // Show available submodules
        _console.WriteInfo("Available submodules:");
        RunGitCommand("submodule status");

        // Ask if user wants to update a specific submodule or all submodules
        var specificPath = _prompt.Prompt("Enter submodule path to update (leave empty to update all): ") ?? "";

        // Ask for update options
        _console.WriteInfo("Update options:");
        _console.WriteInfo("1. Standard update (updates to the commit specified in the superproject)");
        _console.WriteInfo("2. Remote update (fetches and updates to the latest commit on the remote branch)");
        _console.WriteInfo("3. Init and update (initializes first if needed, then updates)");
        _console.WriteInfo("4. Recursive update (updates nested submodules too)");
        _console.WriteInfo("5. Force update (discards local changes)");

        string updateOption = _prompt.Prompt("Choose update option (1-5) [1]: ") ?? "1";

        string updateCommand;
        string operationDescription;
        string updateFlags = "";

        // Build the update command based on the option chosen
        switch (updateOption)
        {
            case "2":
                updateFlags = "--remote";
                operationDescription = "to the latest remote commits";
                break;
            case "3":
                updateFlags = "--init";
                operationDescription = "with initialization";
                break;
            case "4":
                updateFlags = "--recursive";
                operationDescription = "recursively (including nested submodules)";
                break;
            case "5":
                updateFlags = "--force";
                operationDescription = "forcefully (discarding local changes)";
                break;
            default: // Case "1" or invalid input
                updateFlags = "";
                operationDescription = "to the version specified in the superproject";
                break;
        }

        if (string.IsNullOrWhiteSpace(specificPath))
        {
            // Update all submodules
            updateCommand = $"submodule update {updateFlags}".Trim();

            // For recursive updates, always add --init for better user experience
            if (updateOption == "4")
            {
                updateCommand = "submodule update --recursive --init";
                operationDescription = "recursively with initialization";
            }

            _console.WriteInfo($"Updating all submodules {operationDescription}...");
        }
        else
        {
            // Check if the specified submodule exists
            bool submoduleExists = false;
            var submodules = statusOutput.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var submodule in submodules)
                if (submodule.Contains(specificPath))
                {
                    submoduleExists = true;
                    break;
                }

            if (!submoduleExists)
            {
                _console.WriteError($"No submodule found at path '{specificPath}'.");
                _console.WriteInfo("Available submodules: ");
                foreach (var submodule in submodules)
                {
                    string submodulePath = submodule.Trim();
                    if (submodulePath.Contains(" ")) submodulePath = submodulePath.Split(' ').Last();
                    _console.WriteInfo($"- {submodulePath}");
                }

                WaitForUser();
                return;
            }

            // Update specific submodule
            updateCommand = $"submodule update {updateFlags} {specificPath}".Trim();

            // For recursive updates, always add --init for better user experience
            if (updateOption == "4")
            {
                updateCommand = $"submodule update --recursive --init {specificPath}";
                operationDescription = "recursively with initialization";
            }

            _console.WriteInfo($"Updating submodule '{specificPath}' {operationDescription}...");
        }

        _console.WriteInfo($"Running command: git {updateCommand}");

        // Run the update command and capture detailed output
        var (output, error, exitCode) = RunGitCommandWithOutput(updateCommand);

        if (exitCode == 0)
        {
            if (string.IsNullOrWhiteSpace(specificPath))
                _console.WriteSuccess($"✓ All submodules updated successfully {operationDescription}!");
            else
                _console.WriteSuccess($"✓ Submodule '{specificPath}' updated successfully {operationDescription}!");

            // Show updated submodule status
            _console.WriteInfo("\nCurrent submodule status:");
            RunGitCommand("submodule status");

            // Inform user about additional steps if needed
            if (updateOption == "2") // Remote update
            {
                _console.WriteInfo("\nNote: The submodule(s) have been updated to the latest remote commits.");
                _console.WriteInfo("These changes are not automatically committed to your repository.");
                _console.WriteInfo("To make these updates permanent:");
                _console.WriteInfo("1. Add the submodule changes: git add <submodule-path>");
                _console.WriteInfo("2. Commit the changes: git commit -m \"Update submodule to latest commit\"");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(output))
                    _console.WriteInfo("No changes were needed to update the submodule(s).");
            }
        }
        else
        {
            if (string.IsNullOrWhiteSpace(specificPath))
                _console.WriteError($"✗ Failed to update submodules {operationDescription}. Error details:");
            else
                _console.WriteError(
                    $"✗ Failed to update submodule '{specificPath}' {operationDescription}. Error details:");

            if (!string.IsNullOrWhiteSpace(error))
            {
                _console.WriteError(error);

                // Provide helpful guidance based on common error patterns
                if (error.Contains("Please make sure you have the correct access rights"))
                {
                    _console.WriteInfo(
                        "\nSuggestion: Access rights issue. Check your credentials for the submodule repository.");
                    _console.WriteInfo("For SSH repositories, verify your SSH keys are set up correctly.");
                    _console.WriteInfo("For HTTPS repositories, you may need to provide credentials.");

                    // Option to try with HTTPS instead of SSH or vice versa
                    if (error.Contains("ssh") &&
                        _confirmation.ConfirmAction(
                            "Would you like to view instructions for switching from SSH to HTTPS?"))
                    {
                        _console.WriteInfo("\nTo switch from SSH to HTTPS:");
                        _console.WriteInfo("1. Edit .gitmodules file and change URL from:");
                        _console.WriteInfo("   ssh://git@github.com/... to https://github.com/...");
                        _console.WriteInfo("2. Run: git submodule sync");
                        _console.WriteInfo("3. Run: git submodule update");
                    }
                }
                else if (error.Contains("did not match any file(s) known to git"))
                {
                    _console.WriteInfo("\nSuggestion: The submodule path may be incorrect or not initialized.");
                    _console.WriteInfo("Try using 'git submodule init' first, then 'git submodule update'.");

                    if (_confirmation.ConfirmAction("Would you like to try initializing the submodules first?"))
                    {
                        _console.WriteInfo("\nInitializing submodules...");
                        var (initOutput, initError, initExitCode) = RunGitCommandWithOutput("submodule init");

                        if (initExitCode == 0)
                        {
                            _console.WriteSuccess("✓ Submodules initialized successfully!");
                            _console.WriteInfo("\nNow updating submodules...");

                            // Try the update again
                            var (retryOutput, retryError, retryExitCode) = RunGitCommandWithOutput(updateCommand);

                            if (retryExitCode == 0)
                            {
                                _console.WriteSuccess("✓ Submodules updated successfully on retry!");
                                _console.WriteInfo("\nCurrent submodule status:");
                                RunGitCommand("submodule status");
                            }
                            else
                            {
                                _console.WriteError("✗ Failed to update submodules on retry. Error details:");
                                _console.WriteError(retryError);
                            }
                        }
                        else
                        {
                            _console.WriteError("✗ Failed to initialize submodules. Error details:");
                            _console.WriteError(initError);
                        }
                    }
                }
                else if (error.Contains("pathspec") && error.Contains("did not match any file"))
                {
                    _console.WriteInfo("\nSuggestion: The submodule may not be initialized or the path is incorrect.");

                    // Show available submodules again
                    _console.WriteInfo("\nAvailable submodules:");
                    RunGitCommand("submodule status");

                    _console.WriteInfo(
                        "\nTry updating with the correct path or use the option to initialize first (option 3).");
                }
                else if (error.Contains("Submodule path") && error.Contains("not initialized"))
                {
                    _console.WriteInfo("\nSuggestion: The submodule is not initialized.");

                    if (_confirmation.ConfirmAction("Would you like to initialize and update the submodule?"))
                    {
                        string initCommand = string.IsNullOrWhiteSpace(specificPath)
                            ? "submodule update --init"
                            : $"submodule update --init {specificPath}";

                        _console.WriteInfo($"\nRunning: git {initCommand}");
                        var (initOutput, initError, initExitCode) = RunGitCommandWithOutput(initCommand);

                        if (initExitCode == 0)
                        {
                            _console.WriteSuccess("✓ Submodule initialized and updated successfully!");
                            _console.WriteInfo("\nCurrent submodule status:");
                            RunGitCommand("submodule status");
                        }
                        else
                        {
                            _console.WriteError("✗ Failed to initialize and update submodule. Error details:");
                            _console.WriteError(initError);
                        }
                    }
                }
                else if (error.Contains("local modifications") || error.Contains("would be overwritten"))
                {
                    _console.WriteInfo(
                        "\nSuggestion: There are local changes in the submodule that would be overwritten.");
                    _console.WriteInfo("Options:");
                    _console.WriteInfo("1. Commit or stash your changes in the submodule first");
                    _console.WriteInfo("2. Use --force to discard local changes (option 5)");

                    if (_confirmation.ConfirmAction("Would you like to retry with --force to discard local changes?"))
                    {
                        string forceCommand = string.IsNullOrWhiteSpace(specificPath)
                            ? "submodule update --force"
                            : $"submodule update --force {specificPath}";

                        _console.WriteInfo($"\nRunning: git {forceCommand}");
                        var (forceOutput, forceError, forceExitCode) = RunGitCommandWithOutput(forceCommand);

                        if (forceExitCode == 0)
                        {
                            _console.WriteSuccess("✓ Submodule updated successfully with --force!");
                            _console.WriteInfo("\nCurrent submodule status:");
                            RunGitCommand("submodule status");
                        }
                        else
                        {
                            _console.WriteError("✗ Failed to update submodule with --force. Error details:");
                            _console.WriteError(forceError);
                        }
                    }
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
            if (!string.IsNullOrWhiteSpace(statusError)) _console.WriteError("Error details: " + statusError);
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
            string submodulePath = submodule.Trim();
            if (submodulePath.Contains(" ")) submodulePath = submodulePath.Split(' ').Last();

            if (submodulePath.Equals(path, StringComparison.OrdinalIgnoreCase))
            {
                submoduleExists = true;
                break;
            }
        }

        if (!submoduleExists)
        {
            _console.WriteError($"No submodule found at path '{path}'.");
            _console.WriteInfo("Available submodules: ");
            foreach (var submodule in submodules)
            {
                string submodulePath = submodule.Trim();
                if (submodulePath.Contains(" ")) submodulePath = submodulePath.Split(' ').Last();
                _console.WriteInfo($"- {submodulePath}");
            }

            WaitForUser();
            return;
        }

        // Check for pending changes in the submodule
        if (Directory.Exists(path))
        {
            var (subStatusOutput, subStatusError, subStatusExitCode) =
                _processService.RunProcessWithOutput(GetGitPath(), "status --short", Path.GetFullPath(path));

            if (subStatusExitCode == 0 && !string.IsNullOrWhiteSpace(subStatusOutput))
            {
                _console.WriteInfo($"[Warning] The submodule at '{path}' has uncommitted changes:");
                _console.WriteInfo(subStatusOutput);

                if (!_confirmation.ConfirmAction("These changes will be lost when removing the submodule. Continue?",
                        false))
                {
                    _console.WriteInfo("Submodule removal cancelled.");
                    WaitForUser();
                    return;
                }
            }
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
        StringBuilder errorDetails = new StringBuilder();

        // The proper Git submodule removal process involves multiple steps

        // Step 1: Deinitialize the submodule
        _console.WriteInfo("Step 1/4: Deinitializing submodule...");
        var (output1, error1, exitCode1) = RunGitCommandWithOutput($"submodule deinit -f {path}");
        if (exitCode1 != 0)
        {
            if (error1.Contains("error: pathspec") || error1.Contains("did not match any file"))
            {
                _console.WriteInfo(
                    "[Warning] Submodule might be partially removed already. Continuing with next steps...");
            }
            else
            {
                success = false;
                errorDetails.AppendLine("Error deinitializing submodule:");
                errorDetails.AppendLine(error1);
            }
        }
        else
        {
            _console.WriteSuccess("✓ Submodule deinitialized successfully.");
        }

        // Step 2: Remove submodule from index and working tree
        _console.WriteInfo("Step 2/4: Removing from Git index...");
        var (output2, error2, exitCode2) = RunGitCommandWithOutput($"rm -f {path}");
        if (exitCode2 != 0)
        {
            if (error2.Contains("pathspec") || error2.Contains("did not match any file"))
            {
                _console.WriteInfo("[Warning] Path might be removed already. Continuing with next steps...");
            }
            else
            {
                success = false;
                errorDetails.AppendLine("Error removing from Git index:");
                errorDetails.AppendLine(error2);
            }
        }
        else
        {
            _console.WriteSuccess("✓ Submodule removed from Git index successfully.");
        }

        // Step 3: Remove the submodule directory from .git/modules
        _console.WriteInfo("Step 3/4: Cleaning up .git/modules...");
        // Use cross-platform approach to remove directory
        string gitModulesPath = Path.Combine(".git", "modules", path);
        if (Directory.Exists(gitModulesPath))
            try
            {
                Directory.Delete(gitModulesPath, true); // true for recursive
                _console.WriteSuccess($"✓ Removed .git/modules/{path} successfully.");
            }
            catch (Exception ex)
            {
                success = false;
                errorDetails.AppendLine($"Error removing .git/modules/{path}: {ex.Message}");
            }
        else
            _console.WriteInfo($"No .git/modules/{path} directory found (might be already removed).");

        // Step 4: Check if we need to remove entry from .gitmodules
        bool gitmodulesEntryRemoved = false;
        if (File.Exists(".gitmodules"))
            try
            {
                string gitmodulesContent = File.ReadAllText(".gitmodules");
                if (gitmodulesContent.Contains($"path = {path}"))
                {
                    _console.WriteInfo("Step 4/4: Removing submodule entry from .gitmodules file...");
                    // Use git directly to modify the .gitmodules file
                    var (output3, error3, exitCode3) =
                        RunGitCommandWithOutput($"config --file=.gitmodules --remove-section submodule.{path}");
                    if (exitCode3 == 0)
                    {
                        gitmodulesEntryRemoved = true;
                        RunGitCommand("add .gitmodules", false); // Stage the changes
                        _console.WriteSuccess("✓ Removed submodule entry from .gitmodules file successfully.");
                    }
                    else
                    {
                        _console.WriteInfo("[Warning] Could not automatically remove entry from .gitmodules file.");
                        _console.WriteInfo("You might need to manually edit .gitmodules file to complete removal.");

                        // Read .gitmodules content and suggest manual edit
                        if (!string.IsNullOrWhiteSpace(gitmodulesContent))
                        {
                            _console.WriteInfo("\nCurrent .gitmodules content:");
                            _console.WriteInfo(gitmodulesContent);
                            _console.WriteInfo("\nManually remove the [submodule \"" + path +
                                               "\"] section and all its properties.");
                        }
                    }
                }
                else
                {
                    _console.WriteInfo("No entry for this submodule found in .gitmodules file.");
                }
            }
            catch (Exception ex)
            {
                _console.WriteInfo($"[Warning] Error processing .gitmodules file: {ex.Message}");
            }
        else
            _console.WriteInfo("No .gitmodules file found (might be already removed).");

        // Step 5: Commit the changes
        if (success && gitmodulesEntryRemoved)
        {
            _console.WriteInfo("Step 5/4: Committing changes...");
            var commitMessage = $"Remove submodule {path}";
            var (output3, error3, exitCode3) = RunGitCommandWithOutput($"commit -m \"{commitMessage}\"");
            if (exitCode3 != 0)
            {
                // This might happen if there's nothing staged - don't treat as complete failure
                if (error3.Contains("nothing to commit"))
                {
                    _console.WriteInfo(
                        "Note: No changes were committed. This is normal if the submodule was already removed from Git.");
                    _console.WriteInfo("You may need to manually commit any remaining changes.");
                }
                else
                {
                    _console.WriteError("Error committing changes:");
                    _console.WriteError(error3);
                }
            }
            else
            {
                _console.WriteSuccess("✓ Changes committed successfully.");
            }
        }

        // Clean up the actual directory if it still exists
        if (Directory.Exists(path))
            try
            {
                _console.WriteInfo($"Removing directory '{path}'...");
                Directory.Delete(path, true);
                _console.WriteSuccess($"✓ Directory '{path}' removed successfully.");
            }
            catch (Exception ex)
            {
                _console.WriteError($"Error removing directory: {ex.Message}");
                _console.WriteInfo("You may need to manually remove the directory.");
                success = false;
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
            _console.WriteError($"⚠ There were some issues removing submodule '{path}':");
            _console.WriteError(errorDetails.ToString());
            _console.WriteInfo(
                "\nThe submodule might be partially removed. You may need to manually complete the removal process.");

            // Provide recovery guidance
            _console.WriteInfo("\nRecovery steps if needed:");
            _console.WriteInfo("1. Check if the submodule still exists: git submodule status");
            _console.WriteInfo("2. If needed, manually edit the .gitmodules file to remove the entry");
            _console.WriteInfo("3. Run: git add .gitmodules");
            _console.WriteInfo("4. If any remaining submodule references exist, run: git rm -f " + path);
            _console.WriteInfo("5. Commit the changes: git commit -m \"Remove submodule " + path + "\"");
        }

        WaitForUser();
    }
}