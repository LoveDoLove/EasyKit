using EasyKit_Gui.Utilities;
using EasyKit.Services;

namespace EasyKit_Gui.Views.Modules;

public class GitControl : UserControl
{
    private readonly Button _addAllButton;
    private readonly ComboBox _branchComboBox;
    private readonly Label _branchLabel;
    private readonly FlowLayoutPanel _buttonPanel;
    private readonly Button _clearLogButton;
    private readonly CmdService _cmdService = new();
    private readonly Button _commitButton;
    private readonly TextBox _commitMessageBox;
    private readonly Label _commitMessageLabel;
    private readonly Button _historyButton;
    private readonly Button _initButton;
    private readonly RichTextBox _logRichTextBox;
    private readonly Button _pullButton;
    private readonly Button _pushButton;
    private readonly Button _statusButton;
    private bool _isPopulatingBranches;

    public GitControl()
    {
        Dock = DockStyle.Fill;
        BackColor = Color.FromArgb(60, 80, 120);

        // Button panel
        _buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 48,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(8, 8, 8, 8),
            BackColor = Color.FromArgb(50, 70, 110),
            AutoSize = true
        };

        // Branch selector
        _branchLabel = new Label
        {
            Text = "Branch:",
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            AutoSize = true,
            Padding = new Padding(8, 0, 0, 0),
            Height = 24
        };
        _branchComboBox = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 220,
            Margin = new Padding(8, 0, 8, 0),
            Font = new Font("Segoe UI", 10F),
            BackColor = Color.White,
            ForeColor = Color.Black
        };
        _branchComboBox.SelectedIndexChanged += async (s, e) => await OnBranchSelected();
        _buttonPanel.Controls.Add(_branchLabel);
        _buttonPanel.Controls.Add(_branchComboBox);

        _statusButton = CreateButton("Status");
        _initButton = CreateButton("Init");
        _addAllButton = CreateButton("Add All");
        _commitButton = CreateButton("Commit");
        _pushButton = CreateButton("Push");
        _pullButton = CreateButton("Pull");
        _historyButton = CreateButton("History");
        _clearLogButton = CreateButton("Clear Log");

        _buttonPanel.Controls.AddRange(new Control[]
        {
            _statusButton, _initButton, _addAllButton, _commitButton, _pushButton, _pullButton, _historyButton,
            _clearLogButton
        });

        // Commit message
        _commitMessageLabel = new Label
        {
            Text = "Commit Message:",
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            Dock = DockStyle.Top,
            Padding = new Padding(8, 0, 0, 0),
            Height = 24
        };
        _commitMessageBox = new TextBox
        {
            Dock = DockStyle.Top,
            Height = 24,
            Margin = new Padding(8, 0, 8, 8)
        };

        // Log output (RichTextBox for color)
        _logRichTextBox = new RichTextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = RichTextBoxScrollBars.Vertical,
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(40, 40, 60),
            ForeColor = Color.White,
            Font = new Font("Consolas", 10),
            BorderStyle = BorderStyle.FixedSingle
        };

        // Add controls
        Controls.Add(_logRichTextBox);
        Controls.Add(_commitMessageBox);
        Controls.Add(_commitMessageLabel);
        Controls.Add(_buttonPanel);

        // Event handlers
        _statusButton.Click += (s, e) => OnStatusClicked();
        _initButton.Click += (s, e) => OnInitClicked();
        _addAllButton.Click += (s, e) => OnAddAllClicked();
        _commitButton.Click += (s, e) => OnCommitClicked();
        _pushButton.Click += (s, e) => OnPushClicked();
        _pullButton.Click += (s, e) => OnPullClicked();
        _historyButton.Click += (s, e) => OnHistoryClicked();
        _clearLogButton.Click += (s, e) => OnClearLogClicked();

        // Also repopulate branches after key git actions
        _statusButton.Click += async (s, e) => await PopulateBranchesAsync();
        _initButton.Click += async (s, e) => await PopulateBranchesAsync();
        _commitButton.Click += async (s, e) => await PopulateBranchesAsync();
        _pushButton.Click += async (s, e) => await PopulateBranchesAsync();
        _pullButton.Click += async (s, e) => await PopulateBranchesAsync();

        // Populate branches on load
        Load += async (s, e) => await PopulateBranchesAsync();
    }

    private async Task PopulateBranchesAsync()
    {
        if (_isPopulatingBranches) return;
        _isPopulatingBranches = true;
        try
        {
            var localBranches = new List<string>();
            var remoteBranches = new List<string>();
            string currentBranch = "";
            await Task.Run(() =>
            {
                // Get local branches
                (string output, string error, int exit) =
                    _cmdService.RunProcess("git", "branch", Environment.CurrentDirectory);
                if (!string.IsNullOrWhiteSpace(output))
                {
                    var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        var branch = line.Trim();
                        if (branch.StartsWith("*"))
                        {
                            currentBranch = branch.Substring(2).Trim();
                            branch = currentBranch;
                        }

                        localBranches.Add(branch);
                    }
                }

                // Get remote branches
                (string routput, string rerror, int rexit) =
                    _cmdService.RunProcess("git", "branch -r", Environment.CurrentDirectory);
                if (!string.IsNullOrWhiteSpace(routput))
                {
                    var lines = routput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        var branch = line.Trim();
                        if (!string.IsNullOrWhiteSpace(branch) && !remoteBranches.Contains(branch))
                            remoteBranches.Add(branch);
                    }
                }
            });
            Invoke(() =>
            {
                _branchComboBox.Items.Clear();
                foreach (var b in localBranches)
                    _branchComboBox.Items.Add(b);
                if (remoteBranches.Count > 0)
                {
                    _branchComboBox.Items.Add("--- Remote ---");
                    foreach (var b in remoteBranches)
                        _branchComboBox.Items.Add(b);
                }

                if (!string.IsNullOrWhiteSpace(currentBranch)) _branchComboBox.SelectedItem = currentBranch;
            });
        }
        catch (Exception ex)
        {
            AppendLog($"[Branch] Error: {ex.Message}");
        }
        finally
        {
            _isPopulatingBranches = false;
        }
    }

    private async Task OnBranchSelected()
    {
        if (_branchComboBox.SelectedItem == null) return;
        string selected = _branchComboBox.SelectedItem.ToString();
        if (string.IsNullOrWhiteSpace(selected) || selected == "--- Remote ---") return;
        // Only switch if not already on this branch
        string currentBranch = "";
        await Task.Run(() =>
        {
            (string output, string error, int exit) =
                _cmdService.RunProcess("git", "branch --show-current", Environment.CurrentDirectory);
            currentBranch = output?.Trim() ?? "";
        });
        if (selected == currentBranch) return;
        // If remote branch, check out as local tracking branch
        bool isRemote = selected.Contains("/");
        string checkoutArg = isRemote ? $"checkout --track {selected}" : $"checkout {selected}";
        AppendLog($"[Branch] Switching to {selected}...");
        await Task.Run(() =>
        {
            (string output, string error, int exit) =
                _cmdService.RunProcess("git", checkoutArg, Environment.CurrentDirectory);
            Invoke(() =>
            {
                if (!string.IsNullOrWhiteSpace(error)) AppendLog($"[Error] {error.Trim()}");
                if (!string.IsNullOrWhiteSpace(output)) AppendLog(output.Trim());
            });
        });
        await PopulateBranchesAsync();
        AppendLog($"[Branch] Now on {selected}.");
    }

    private void OnClearLogClicked()
    {
        _logRichTextBox.Clear();
    }

    private Button CreateButton(string text)
    {
        return new Button
        {
            Text = text,
            Width = 90,
            Height = 32,
            Margin = new Padding(4, 0, 4, 0),
            BackColor = Color.FromArgb(70, 100, 160),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
    }

    private async void OnStatusClicked()
    {
        AppendLog("[Status] Running...");
        await Task.Run(() =>
        {
            (string output, string error, int exit) =
                _cmdService.RunProcess("git", "status --porcelain -b", Environment.CurrentDirectory);
            Invoke(() =>
            {
                if (!string.IsNullOrWhiteSpace(error)) AppendLog($"[Error] {error.Trim()}");
                if (!string.IsNullOrWhiteSpace(output)) AppendLog(output.Trim());
                if (string.IsNullOrWhiteSpace(output) && string.IsNullOrWhiteSpace(error))
                    AppendLog("[Status] No output.");
            });
        });
    }

    private async void OnInitClicked()
    {
        AppendLog("[Init] Running...");
        await Task.Run(() =>
        {
            (string output, string error, int exit) =
                _cmdService.RunProcess("git", "init", Environment.CurrentDirectory);
            Invoke(() =>
            {
                if (!string.IsNullOrWhiteSpace(error)) AppendLog($"[Error] {error.Trim()}");
                if (!string.IsNullOrWhiteSpace(output)) AppendLog(output.Trim());
                AppendLog("[Init] Done.");
            });
        });
    }

    private async void OnAddAllClicked()
    {
        AppendLog("[Add All] Checking for changes...");
        // Step 1: Get list of changed/untracked files
        string[] files = Array.Empty<string>();
        await Task.Run(() =>
        {
            (string output, string error, int exit) =
                _cmdService.RunProcess("git", "status --porcelain", Environment.CurrentDirectory);
            if (!string.IsNullOrWhiteSpace(output))
            {
                var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                var fileList = new List<string>();
                foreach (var line in lines)
                {
                    // Format: XY filename
                    var trimmed = line.Length > 3 ? line.Substring(3).Trim() : null;
                    if (!string.IsNullOrWhiteSpace(trimmed))
                        fileList.Add(trimmed);
                }

                files = fileList.ToArray();
            }
        });

        if (files.Length == 0)
        {
            AppendLog("[Add All] No changes to add.");
            return;
        }

        // Step 2: Show dialog for file selection
        var dialog = new AddFilesDialog(files);
        var result = dialog.ShowDialog();
        if (dialog.Confirmed && dialog.SelectedFiles.Count > 0)
        {
            AppendLog($"[Add All] Adding {dialog.SelectedFiles.Count} file(s)...");
            await Task.Run(() =>
            {
                // Add only selected files
                string args = "add --";
                foreach (var file in dialog.SelectedFiles)
                    args += " \"" + file.Replace("\"", "'") + "\"";
                (string output, string error, int exit) =
                    _cmdService.RunProcess("git", args, Environment.CurrentDirectory);
                Invoke(() =>
                {
                    if (!string.IsNullOrWhiteSpace(error)) AppendLog($"[Error] {error.Trim()}");
                    AppendLog("[Add All] Done.");
                });
            });
        }
        else
        {
            AppendLog("[Add All] Cancelled by user.");
        }
    }

    private async void OnCommitClicked()
    {
        // Step 1: Get staged files
        string[] stagedFiles = Array.Empty<string>();
        await Task.Run(() =>
        {
            (string output, string error, int exit) =
                _cmdService.RunProcess("git", "diff --cached --name-status", Environment.CurrentDirectory);
            if (!string.IsNullOrWhiteSpace(output))
            {
                var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                var fileList = new List<string>();
                foreach (var line in lines)
                {
                    // Format: XY filename
                    var trimmed = line.Length > 2 ? line.Substring(2).Trim() : null;
                    if (!string.IsNullOrWhiteSpace(trimmed))
                        fileList.Add(trimmed);
                }

                stagedFiles = fileList.ToArray();
            }
        });

        // If no staged files, prompt user to select files to stage
        if (stagedFiles.Length == 0)
        {
            // Get all changed/untracked files
            string[] files = Array.Empty<string>();
            await Task.Run(() =>
            {
                (string output, string error, int exit) =
                    _cmdService.RunProcess("git", "status --porcelain", Environment.CurrentDirectory);
                if (!string.IsNullOrWhiteSpace(output))
                {
                    var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    var fileList = new List<string>();
                    foreach (var line in lines)
                    {
                        // Format: XY filename
                        var trimmed = line.Length > 3 ? line.Substring(3).Trim() : null;
                        if (!string.IsNullOrWhiteSpace(trimmed))
                            fileList.Add(trimmed);
                    }

                    files = fileList.ToArray();
                }
            });

            if (files.Length == 0)
            {
                AppendLog("[Commit] No changes to stage or commit.");
                return;
            }

            // Show dialog for file selection
            var addDialog = new AddFilesDialog(files);
            var addResult = addDialog.ShowDialog();
            if (addDialog.Confirmed && addDialog.SelectedFiles.Count > 0)
            {
                AppendLog($"[Commit] Staging {addDialog.SelectedFiles.Count} file(s)...");
                await Task.Run(() =>
                {
                    string args = "add --";
                    foreach (var file in addDialog.SelectedFiles)
                        args += " \"" + file.Replace("\"", "'") + "\"";
                    _cmdService.RunProcess("git", args, Environment.CurrentDirectory);
                });
                // Refresh staged files
                await Task.Run(() =>
                {
                    (string output, string error, int exit) =
                        _cmdService.RunProcess("git", "diff --cached --name-status", Environment.CurrentDirectory);
                    if (!string.IsNullOrWhiteSpace(output))
                    {
                        var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        var fileList = new List<string>();
                        foreach (var line in lines)
                        {
                            var trimmed = line.Length > 2 ? line.Substring(2).Trim() : null;
                            if (!string.IsNullOrWhiteSpace(trimmed))
                                fileList.Add(trimmed);
                        }

                        stagedFiles = fileList.ToArray();
                    }
                });
                if (stagedFiles.Length == 0)
                {
                    AppendLog("[Commit] No files staged after selection.");
                    return;
                }
            }
            else
            {
                AppendLog("[Commit] Cancelled by user (no files selected to stage).");
                return;
            }
        }

        // Step 2: Show commit dialog
        var dialog = new CommitDialog(stagedFiles);
        var result = dialog.ShowDialog();
        if (dialog.Confirmed && !string.IsNullOrWhiteSpace(dialog.CommitTitle))
        {
            string title = dialog.CommitTitle.Replace("\"", "'");
            string message = dialog.CommitMessage.Replace("\"", "'");
            AppendLog($"[Commit] Running: {title}");
            await Task.Run(() =>
            {
                (string output, string error, int exit) =
                    _cmdService.RunProcess("git", $"commit -m \"{title}\" -m \"{message}\"",
                        Environment.CurrentDirectory);
                Invoke(() =>
                {
                    if (!string.IsNullOrWhiteSpace(error)) AppendLog($"[Error] {error.Trim()}");
                    if (!string.IsNullOrWhiteSpace(output)) AppendLog(output.Trim());
                    AppendLog("[Commit] Done.");
                });
            });
        }
        else if (dialog.Confirmed && string.IsNullOrWhiteSpace(dialog.CommitTitle))
        {
            AppendLog("[Commit] Commit title required.");
        }
        else
        {
            AppendLog("[Commit] Cancelled by user.");
        }
    }

    private async void OnPushClicked()
    {
        AppendLog("[Push] Running...");
        await Task.Run(() =>
        {
            (string output, string error, int exit) =
                _cmdService.RunProcess("git", "push", Environment.CurrentDirectory);
            Invoke(() =>
            {
                if (!string.IsNullOrWhiteSpace(error)) AppendLog($"[Error] {error.Trim()}");
                if (!string.IsNullOrWhiteSpace(output)) AppendLog(output.Trim());
                AppendLog("[Push] Done.");
            });
        });
    }

    private async void OnPullClicked()
    {
        AppendLog("[Pull] Checking for remote changes...");
        // Step 1: Fetch remote changes
        await Task.Run(() => { _cmdService.RunProcess("git", "fetch", Environment.CurrentDirectory); });

        // Step 2: Determine current branch
        string branch = "";
        await Task.Run(() =>
        {
            (string output, string error, int exit) =
                _cmdService.RunProcess("git", "rev-parse --abbrev-ref HEAD", Environment.CurrentDirectory);
            branch = output?.Trim() ?? "";
        });
        if (string.IsNullOrWhiteSpace(branch))
        {
            AppendLog("[Pull] Could not determine current branch.");
            return;
        }

        // Step 3: Get incoming changes
        string changes = "";
        bool noChanges = false;
        await Task.Run(() =>
        {
            (string output, string error, int exit) = _cmdService.RunProcess(
                "git",
                $"log HEAD..origin/{branch} --oneline --color=always",
                Environment.CurrentDirectory);
            if (string.IsNullOrWhiteSpace(output))
            {
                changes = "No incoming changes detected.";
                noChanges = true;
            }
            else
            {
                changes = output;
                noChanges = false;
            }
        });

        // Step 4: Show confirmation dialog
        var dialog = new PullConfirmationDialog();
        dialog.SetChangesText(changes);
        dialog.SetNoChangesMode(noChanges);
        var result = dialog.ShowDialog();
        if (!noChanges && dialog.Confirmed)
        {
            AppendLog("[Pull] Running...");
            await Task.Run(() =>
            {
                (string output, string error, int exit) =
                    _cmdService.RunProcess("git", "pull", Environment.CurrentDirectory);
                Invoke(() =>
                {
                    if (!string.IsNullOrWhiteSpace(error)) AppendLog($"[Error] {error.Trim()}");
                    if (!string.IsNullOrWhiteSpace(output)) AppendLog(output.Trim());
                    AppendLog("[Pull] Done.");
                });
            });
        }
        else if (noChanges)
        {
            AppendLog("[Pull] No incoming changes.");
        }
        else
        {
            AppendLog("[Pull] Cancelled by user.");
        }
    }

    private async void OnHistoryClicked()
    {
        AppendLog("[History] Running...");
        await Task.Run(() =>
        {
            (string output, string error, int exit) = _cmdService.RunProcess("git",
                "--no-pager log --graph --decorate --oneline --all --color=always", Environment.CurrentDirectory);
            Invoke(() =>
            {
                if (!string.IsNullOrWhiteSpace(error)) AppendLog($"[Error] {error.Trim()}");
                if (!string.IsNullOrWhiteSpace(output)) AppendLog(output.Trim());
                AppendLog("[History] Done.");
            });
        });
    }

    private void AppendLog(string message)
    {
        AnsiColorParser.AppendAnsiText(_logRichTextBox, $"{DateTime.Now:HH:mm:ss} {message}{Environment.NewLine}");
    }
}