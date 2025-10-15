using System;
using System.Drawing;
using System.Windows.Forms;

namespace EasyKit_Gui.Views.Modules
{
    public class GitControl : UserControl
    {
        private readonly TextBox logTextBox;
        private readonly Button statusButton;
        private readonly Button initButton;
        private readonly Button addAllButton;
        private readonly Button commitButton;
        private readonly Button pushButton;
        private readonly Button pullButton;
        private readonly Button historyButton;
        private readonly TextBox commitMessageBox;
        private readonly Label commitMessageLabel;
        private readonly FlowLayoutPanel buttonPanel;

        public GitControl()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(60, 80, 120);

            // Button panel
            buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 48,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(8, 8, 8, 8),
                BackColor = Color.FromArgb(50, 70, 110),
                AutoSize = true
            };

            statusButton = CreateButton("Status");
            initButton = CreateButton("Init");
            addAllButton = CreateButton("Add All");
            commitButton = CreateButton("Commit");
            pushButton = CreateButton("Push");
            pullButton = CreateButton("Pull");
            historyButton = CreateButton("History");

            buttonPanel.Controls.AddRange(new Control[]
            {
                statusButton, initButton, addAllButton, commitButton, pushButton, pullButton, historyButton
            });

            // Commit message
            commitMessageLabel = new Label
            {
                Text = "Commit Message:",
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Dock = DockStyle.Top,
                Padding = new Padding(8, 0, 0, 0),
                Height = 24
            };
            commitMessageBox = new TextBox
            {
                Dock = DockStyle.Top,
                Height = 24,
                Margin = new Padding(8, 0, 8, 8)
            };

            // Log output
            logTextBox = new TextBox
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
            this.Controls.Add(logTextBox);
            this.Controls.Add(commitMessageBox);
            this.Controls.Add(commitMessageLabel);
            this.Controls.Add(buttonPanel);

            // Event handlers (to be implemented)
            statusButton.Click += (s, e) => OnStatusClicked();
            initButton.Click += (s, e) => OnInitClicked();
            addAllButton.Click += (s, e) => OnAddAllClicked();
            commitButton.Click += (s, e) => OnCommitClicked();
            pushButton.Click += (s, e) => OnPushClicked();
            pullButton.Click += (s, e) => OnPullClicked();
            historyButton.Click += (s, e) => OnHistoryClicked();
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

        // Event handler stubs (to be implemented)
        private void OnStatusClicked() { AppendLog("[Status] Clicked"); }
        private void OnInitClicked() { AppendLog("[Init] Clicked"); }
        private void OnAddAllClicked() { AppendLog("[Add All] Clicked"); }
        private void OnCommitClicked() { AppendLog($"[Commit] {commitMessageBox.Text}"); }
        private void OnPushClicked() { AppendLog("[Push] Clicked"); }
        private void OnPullClicked() { AppendLog("[Pull] Clicked"); }
        private void OnHistoryClicked() { AppendLog("[History] Clicked"); }

        private void AppendLog(string message)
        {
            logTextBox.AppendText($"{DateTime.Now:HH:mm:ss} {message}{Environment.NewLine}");
        }
    }
}