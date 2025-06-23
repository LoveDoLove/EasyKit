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
    private readonly CmdService _processService;
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
        _processService = new CmdService();
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
            _processService.RunProcess("git", "--version", Environment.CurrentDirectory);
        if (gitVersionExit == 0 && !string.IsNullOrWhiteSpace(gitVersionOutput))
        {
            _console.WriteSuccess($"\u2713 Git is accessible. Version: {gitVersionOutput.Trim()}");
        }
        else
        {
            _console.WriteError("\u2717 Git is not accessible via PATH.");
            _console.WriteInfo("Please install Git from https://git-scm.com/downloads and ensure it is in your PATH.");
            _console.WriteInfo("You can also use the Tool Marketplace in EasyKit to open the download page.");
            _console.WriteInfo("\n===== End of GIT Configuration Diagnostics =====");
            WaitForUser();
            return;
        }

        // Step 2: Show detected git path (removed, always 'git')
        _console.WriteInfo("Detected git path: git");

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
        var (statusOutput, statusError, statusExit) =
            _processService.RunProcess("git", "status", Environment.CurrentDirectory);
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

    private void InitRepo()
    {
        _console.WriteInfo("Initializing git repository in current directory...");
        if (File.Exists(".git/config"))
        {
            _console.WriteInfo("Git repository already exists.");
            WaitForUser();
            return;
        }

        _processService.RunProcess("git", "init", Environment.CurrentDirectory);
        _console.WriteSuccess("✓ Git repository initialized successfully!");
        WaitForUser();
    }

    private void CheckStatus()
    {
        _console.WriteInfo("Checking git status...");
        var (output, error, exitCode) = _processService.RunProcess("git", "status", Environment.CurrentDirectory);

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
        _processService.RunProcess("git", "add .", Environment.CurrentDirectory);
        _console.WriteSuccess("✓ Changes added to staging area!");
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
        _processService.RunProcess("git", $"commit -m \"{message}\"", Environment.CurrentDirectory);
        _console.WriteSuccess("✓ Changes committed successfully!");
        WaitForUser();
    }

    private void Push()
    {
        string remoteBranch = _prompt.Prompt("Enter remote and branch (e.g. 'origin main'): ") ?? "origin main";
        if (string.IsNullOrWhiteSpace(remoteBranch))
            remoteBranch = "origin main";

        _console.WriteInfo($"Pushing to {remoteBranch}...");
        _processService.RunProcessWithStreaming("git", $"push {remoteBranch}", Environment.CurrentDirectory);
        _console.WriteSuccess("✓ Push completed!");
        WaitForUser();
    }

    private void Pull()
    {
        string remoteBranch = _prompt.Prompt("Enter remote and branch (e.g. 'origin main'): ") ?? "origin main";
        if (string.IsNullOrWhiteSpace(remoteBranch))
            remoteBranch = "origin main";

        _console.WriteInfo($"Pulling from {remoteBranch}...");
        _processService.RunProcessWithStreaming("git", $"pull {remoteBranch}", Environment.CurrentDirectory);
        _console.WriteSuccess("✓ Pull completed!");
        WaitForUser();
    }

    private void ViewHistory()
    {
        _console.WriteInfo("Showing git log...");
        _processService.RunProcessWithStreaming("git", "log --oneline --graph --all", Environment.CurrentDirectory);
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
        _processService.RunProcess("git", $"checkout -b {branchName}", Environment.CurrentDirectory);
        _console.WriteSuccess($"✓ Branch '{branchName}' created and checked out successfully!");
        WaitForUser();
    }

    private void SwitchBranch()
    {
        _console.WriteInfo("Available branches:");
        _processService.RunProcess("git", "branch", Environment.CurrentDirectory);
        var branchName = _prompt.Prompt("Enter branch name to switch to: ") ?? "";
        if (string.IsNullOrWhiteSpace(branchName))
        {
            _console.WriteError("Branch name cannot be empty.");
            Console.ReadLine();
            return;
        }

        _console.WriteInfo($"Switching to branch '{branchName}'...");
        _processService.RunProcess("git", $"checkout {branchName}", Environment.CurrentDirectory);
        _console.WriteSuccess($"✓ Switched to branch '{branchName}' successfully!");
        WaitForUser();
    }

    private void MergeBranch()
    {
        _console.WriteInfo("Available branches:");
        _processService.RunProcess("git", "branch", Environment.CurrentDirectory);
        var branchName = _prompt.Prompt("Enter branch name to merge into current branch: ") ?? "";
        if (string.IsNullOrWhiteSpace(branchName))
        {
            _console.WriteError("Branch name cannot be empty.");
            Console.ReadLine();
            return;
        }

        _console.WriteInfo($"Merging branch '{branchName}' into current branch...");
        _processService.RunProcessWithStreaming("git", $"merge {branchName}", Environment.CurrentDirectory);
        _console.WriteSuccess($"✓ Branch {branchName} merged!");
        WaitForUser();
    }

    private void StashChanges()
    {
        var message = _prompt.Prompt("Enter stash message (optional): ") ?? "";
        var command = string.IsNullOrWhiteSpace(message) ? "stash" : $"stash push -m \"{message}\"";
        _console.WriteInfo("Stashing changes...");
        _processService.RunProcess("git", command, Environment.CurrentDirectory);
        _console.WriteSuccess("✓ Changes stashed successfully!");
        WaitForUser();
    }

    private void ApplyStash()
    {
        _console.WriteInfo("Available stashes:");
        _processService.RunProcess("git", "stash list", Environment.CurrentDirectory);
        var stashId = _prompt.Prompt("Enter stash to apply (e.g. 'stash@{0}', leave empty for latest): ") ?? "";
        var command = string.IsNullOrWhiteSpace(stashId) ? "stash apply" : $"stash apply {stashId}";
        _console.WriteInfo("Applying stash...");
        _processService.RunProcess("git", command, Environment.CurrentDirectory);
        _console.WriteSuccess("✓ Stash applied successfully!");
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

        // Run the git submodule add command and stream output for user feedback
        string addCommand = url.StartsWith("--force") ? $"submodule add {url} {path}" : $"submodule add {url} {path}";
        _processService.RunProcessWithStreaming("git", addCommand, Environment.CurrentDirectory);
        _console.WriteSuccess("✓ Submodule add command executed. Check above for any errors or output.");
        _console.WriteInfo("\nSubmodule Information:");
        _processService.RunProcess("git", "submodule status", Environment.CurrentDirectory);
        _console.WriteInfo("\nYou can update this submodule later using 'Update submodule' option.");
        WaitForUser();
    }

    private void UpdateSubmodule()
    {
        // Check if there are any submodules first
        var (statusOutput, statusError, statusExitCode) =
            _processService.RunProcess("git", "submodule status", Environment.CurrentDirectory);
        if (statusExitCode != 0 || string.IsNullOrWhiteSpace(statusOutput))
        {
            _console.WriteError("No submodules found in this repository.");
            if (!string.IsNullOrWhiteSpace(statusError)) _console.WriteError("Error details: " + statusError);
            WaitForUser();
            return;
        }

        // Show available submodules
        _console.WriteInfo("Available submodules:");
        _processService.RunProcess("git", "submodule status", Environment.CurrentDirectory);

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

        // Run the update command and stream output for user feedback
        _processService.RunProcessWithStreaming("git", updateCommand, Environment.CurrentDirectory);
        _console.WriteSuccess("✓ Submodule update command executed. Check above for any errors or output.");
        _console.WriteInfo("\nCurrent submodule status:");
        _processService.RunProcess("git", "submodule status", Environment.CurrentDirectory);
        WaitForUser();
    }

    private void RemoveSubmodule()
    {
        // Check if there are any submodules first
        var (statusOutput, statusError, statusExitCode) =
            _processService.RunProcess("git", "submodule status", Environment.CurrentDirectory);
        if (statusExitCode != 0 || string.IsNullOrWhiteSpace(statusOutput))
        {
            _console.WriteError("No submodules found in this repository.");
            if (!string.IsNullOrWhiteSpace(statusError)) _console.WriteError("Error details: " + statusError);
            WaitForUser();
            return;
        }

        // Show available submodules
        _console.WriteInfo("Available submodules:");
        _processService.RunProcess("git", "submodule status", Environment.CurrentDirectory);

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
                _processService.RunProcess("git", "status --short", Path.GetFullPath(path));

            if (subStatusExitCode == 0 && !string.IsNullOrWhiteSpace(subStatusOutput))
            {
                _console.WriteInfo($"[Warning] The submodule at '{path}' has uncommitted changes:");
                _console.WriteInfo(subStatusOutput);

                if (!_confirmation.ConfirmAction(
                        "These changes will be lost when removing the submodule. Continue?",
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
        var (output1, error1, exitCode1) =
            _processService.RunProcess("git", $"submodule deinit -f {path}", Environment.CurrentDirectory);
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
        var (output2, error2, exitCode2) =
            _processService.RunProcess("git", $"rm -f {path}", Environment.CurrentDirectory);
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
                        _processService.RunProcess("git",
                            $"config --file=.gitmodules --remove-section submodule.{path}",
                            Environment.CurrentDirectory);
                    if (exitCode3 == 0)
                    {
                        gitmodulesEntryRemoved = true;
                        _processService.RunProcess("git", "add .gitmodules",
                            Environment.CurrentDirectory); // Stage the changes
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
            var (output3, error3, exitCode3) = _processService.RunProcess("git", $"commit -m \"{commitMessage}\"",
                Environment.CurrentDirectory);
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
            _processService.RunProcess("git", "submodule status", Environment.CurrentDirectory);
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