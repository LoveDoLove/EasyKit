using EasyKit.Services;

namespace EasyKit_Gui.Views.Modules;

public class GitControl : UserControl
{
    private readonly Button _addAllButton;
    private readonly FlowLayoutPanel _buttonPanel;
    private readonly Button _clearLogButton;

    // Backend binding: run git commands using CmdService (from EasyKit.Services)
    private readonly CmdService _cmdService = new();
    private readonly Button _commitButton;
    private readonly TextBox _commitMessageBox;
    private readonly Label _commitMessageLabel;
    private readonly Button _historyButton;
    private readonly Button _initButton;
    private readonly TextBox _logTextBox;
    private readonly Button _pullButton;
    private readonly Button _pushButton;
    private readonly Button _statusButton;

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

        // Log output
        _logTextBox = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(40, 40, 60),
            ForeColor = Color.White,
            Font = new Font("Consolas", 10),
            BorderStyle = BorderStyle.FixedSingle
        };

        // Add controls
        Controls.Add(_logTextBox);
        Controls.Add(_commitMessageBox);
        Controls.Add(_commitMessageLabel);
        Controls.Add(_buttonPanel);

        // Event handlers (to be implemented)
        _statusButton.Click += (s, e) => OnStatusClicked();
        _initButton.Click += (s, e) => OnInitClicked();
        _addAllButton.Click += (s, e) => OnAddAllClicked();
        _commitButton.Click += (s, e) => OnCommitClicked();
        _pushButton.Click += (s, e) => OnPushClicked();
        _pullButton.Click += (s, e) => OnPullClicked();
        _historyButton.Click += (s, e) => OnHistoryClicked();
        _clearLogButton.Click += (s, e) => OnClearLogClicked();
    }

    private void OnClearLogClicked()
    {
        _logTextBox.Clear();
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
        AppendLog("[Add All] Running...");
        await Task.Run(() =>
        {
            (string output, string error, int exit) =
                _cmdService.RunProcess("git", "add .", Environment.CurrentDirectory);
            Invoke(() =>
            {
                if (!string.IsNullOrWhiteSpace(error)) AppendLog($"[Error] {error.Trim()}");
                AppendLog("[Add All] Done.");
            });
        });
    }

    private async void OnCommitClicked()
    {
        string msg = _commitMessageBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(msg))
        {
            AppendLog("[Commit] Commit message required.");
            return;
        }

        AppendLog($"[Commit] Running: {msg}");
        await Task.Run(() =>
        {
            (string output, string error, int exit) =
                _cmdService.RunProcess("git", $"commit -m \"{msg.Replace("\"", "'")}\"", Environment.CurrentDirectory);
            Invoke(() =>
            {
                if (!string.IsNullOrWhiteSpace(error)) AppendLog($"[Error] {error.Trim()}");
                if (!string.IsNullOrWhiteSpace(output)) AppendLog(output.Trim());
                AppendLog("[Commit] Done.");
            });
        });
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

    private async void OnHistoryClicked()
    {
        AppendLog("[History] Running...");
        await Task.Run(() =>
        {
            (string output, string error, int exit) = _cmdService.RunProcess("git",
                "--no-pager log --graph --decorate --oneline --all", Environment.CurrentDirectory);
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
        _logTextBox.AppendText($"{DateTime.Now:HH:mm:ss} {message}{Environment.NewLine}");
    }
}