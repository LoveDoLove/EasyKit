namespace EasyKit.Controllers;

public class GitController
{
    private readonly ConfirmationService _confirmation;
    private readonly ConsoleService _console;
    private readonly LoggerService _logger;
    private readonly NotificationView _notificationView;
    private readonly ProcessService _processService;
    private readonly PromptView _prompt;
    private readonly Software _software;

    public GitController(
        Software software,
        LoggerService logger,
        ConsoleService console,
        ConfirmationService confirmation,
        PromptView prompt,
        NotificationView notificationView)
    {
        _software = software;
        _logger = logger;
        _console = console;
        _confirmation = confirmation;
        _prompt = prompt;
        _notificationView = notificationView;
        _processService = new ProcessService(logger, console, console.Config);
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
        int menuWidth = 50;
        string colorSchemeStr = "dark";

        // Try to get user preferences from config
        var menuWidthObj = _console.Config.Get("menu_width", 50);
        if (menuWidthObj is int mw)
            menuWidth = mw;

        var colorSchemeObj = _console.Config.Get("color_scheme", "dark");
        if (colorSchemeObj != null)
            colorSchemeStr = colorSchemeObj.ToString() ?? "dark";

        // Apply the appropriate color scheme based on user settings
        var colorScheme = MenuTheme.ColorScheme.Dark;
        if (colorSchemeStr.ToLower() == "light")
            colorScheme = MenuTheme.ColorScheme.Light;

        // Create and configure the menu
        var menuView = new MenuView();
        menuView.CreateMenu("Git Tools", width: menuWidth)
            .AddOption("1", "Init repository", () => InitRepo())
            .AddOption("2", "Status", () => CheckStatus())
            .AddOption("3", "Add all changes", () => AddAll())
            .AddOption("4", "Commit staged changes", () => Commit())
            .AddOption("5", "Push to remote", () => Push())
            .AddOption("6", "Pull from remote", () => Pull())
            .AddOption("7", "Create branch", () => CreateBranch())
            .AddOption("8", "Switch branch", () => SwitchBranch())
            .AddOption("9", "Merge branch", () => MergeBranch())
            .AddOption("10", "View commit history", () => ViewHistory())
            .AddOption("11", "Stash changes", () => StashChanges())
            .AddOption("12", "Apply stash", () => ApplyStash())
            .AddOption("13", "Create pull request (info)", () => CreatePullRequest())
            .AddOption("14", "List pull requests (info)", () => ListPullRequests())
            .AddOption("15", "Run git diagnostics", () => RunDiagnostics())
            .AddOption("0", "Back to main menu", () =>
            {
                /* Return to main menu */
            })
            .WithColors(colorScheme.border, colorScheme.highlight, colorScheme.title, colorScheme.text,
                colorScheme.help)
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
}