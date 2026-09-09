namespace EasyKit_Gui.Utilities;

public class SubmoduleDialog : Form
{
    private readonly Button _addButton;
    private readonly Button _closeButton;
    private readonly Label _pathLabel;
    private readonly TextBox _pathTextBox;
    private readonly Button _removeButton;
    private readonly Label _statusLabel;
    private readonly ListBox _submoduleListBox;
    private readonly Button _updateAllButton;
    private readonly Button _updateButton;
    private readonly Label _urlLabel;
    private readonly TextBox _urlTextBox;

    public SubmoduleDialog(IEnumerable<string> submodules, string statusText)
    {
        Text = "Git Submodules";
        Size = new Size(480, 400);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;

        _submoduleListBox = new ListBox
        {
            Dock = DockStyle.Top,
            Height = 180,
            Font = new Font("Segoe UI", 10F),
            SelectionMode = SelectionMode.One
        };
        _submoduleListBox.Items.AddRange(submodules is string[] arr ? arr : new List<string>(submodules).ToArray());

        _statusLabel = new Label
        {
            Text = statusText,
            Dock = DockStyle.Top,
            Height = 60,
            Font = new Font("Consolas", 9F),
            ForeColor = Color.DimGray,
            Padding = new Padding(8, 8, 8, 8),
            AutoSize = false
        };

        _updateButton = new Button { Text = "Update Selected", Width = 120, Height = 32, Margin = new Padding(8) };
        _updateAllButton = new Button { Text = "Update All", Width = 100, Height = 32, Margin = new Padding(8) };
        _removeButton = new Button { Text = "Remove Selected", Width = 140, Height = 32, Margin = new Padding(8) };
        _closeButton = new Button { Text = "Close", Width = 80, Height = 32, Margin = new Padding(8) };
        _addButton = new Button
            { Text = "Add Submodule", Width = 140, Height = 32, Margin = new Padding(8), Enabled = false };
        _urlLabel = new Label
            { Text = "Repository URL (required):", AutoSize = true, Margin = new Padding(8, 8, 0, 0) };
        _urlTextBox = new TextBox { Width = 320, Margin = new Padding(8, 0, 8, 0) };
        try
        {
            _urlTextBox.PlaceholderText = "e.g. https://github.com/owner/repo.git";
        }
        catch
        {
            /* .NET < 6 fallback */
        }

        _pathLabel = new Label { Text = "Destination Path:", AutoSize = true, Margin = new Padding(8, 8, 0, 0) };
        _pathTextBox = new TextBox { Width = 320, Margin = new Padding(8, 0, 8, 0) };

        // Enable Add button only if both fields are filled and URL looks valid
        void ValidateAddButton()
        {
            bool urlOk = !string.IsNullOrWhiteSpace(_urlTextBox.Text) &&
                         (_urlTextBox.Text.StartsWith("http://") || _urlTextBox.Text.StartsWith("https://") ||
                          _urlTextBox.Text.EndsWith(".git"));
            bool pathOk = !string.IsNullOrWhiteSpace(_pathTextBox.Text);
            _addButton.Enabled = urlOk && pathOk;
        }

        _urlTextBox.TextChanged += (s, e) => ValidateAddButton();
        _pathTextBox.TextChanged += (s, e) => ValidateAddButton();
        ValidateAddButton();

        var addPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 80,
            FlowDirection = FlowDirection.TopDown,
            Padding = new Padding(8, 8, 8, 8),
            AutoSize = true
        };
        addPanel.Controls.Add(_urlLabel);
        addPanel.Controls.Add(_urlTextBox);
        addPanel.Controls.Add(_pathLabel);
        addPanel.Controls.Add(_pathTextBox);
        addPanel.Controls.Add(_addButton);

        // Create a GroupBox to wrap the addPanel for prominence
        var addGroup = new GroupBox
        {
            Text = "Add New Submodule",
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Dock = DockStyle.Top,
            Padding = new Padding(8, 8, 8, 8),
            Height = 180,
            BackColor = Color.FromArgb(240, 248, 255)
        };
        addPanel.Margin = new Padding(8, 8, 8, 8);
        addPanel.BackColor = Color.FromArgb(240, 248, 255);
        addGroup.Controls.Add(addPanel);

        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 48,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(8, 8, 8, 8),
            AutoSize = true
        };
        buttonPanel.Controls.AddRange([_updateButton, _updateAllButton, _removeButton, _closeButton]);

        Controls.Add(buttonPanel);
        Controls.Add(addGroup);
        Controls.Add(_statusLabel);
        Controls.Add(_submoduleListBox);

        _updateButton.Click += (s, e) =>
        {
            UpdateSelected = true;
            DialogResult = DialogResult.OK;
        };
        _updateAllButton.Click += (s, e) =>
        {
            UpdateAll = true;
            DialogResult = DialogResult.OK;
        };
        _removeButton.Click += (s, e) =>
        {
            RemoveSelected = true;
            DialogResult = DialogResult.OK;
        };
        _addButton.Click += (s, e) =>
        {
            AddSubmodule = true;
            DialogResult = DialogResult.OK;
        };
        _closeButton.Click += (s, e) => { DialogResult = DialogResult.Cancel; };
    }

    public string? SelectedSubmodule => _submoduleListBox.SelectedItem?.ToString();
    public bool UpdateSelected { get; private set; }
    public bool UpdateAll { get; private set; }
    public bool RemoveSelected { get; private set; }
    public bool AddSubmodule { get; private set; }
    public string SubmoduleUrl => _urlTextBox.Text.Trim();
    public string SubmodulePath => _pathTextBox.Text.Trim();
}