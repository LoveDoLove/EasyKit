namespace EasyKit_Gui.Utilities;

public class SubmoduleDialog : Form
{
    private readonly Button _closeButton;
    private readonly Button _removeButton;
    private readonly Label _statusLabel;
    private readonly ListBox _submoduleListBox;
    private readonly Button _updateAllButton;
    private readonly Button _updateButton;

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
        _closeButton.Click += (s, e) => { DialogResult = DialogResult.Cancel; };
    }

    public string? SelectedSubmodule => _submoduleListBox.SelectedItem?.ToString();
    public bool UpdateSelected { get; private set; }
    public bool UpdateAll { get; private set; }
    public bool RemoveSelected { get; private set; }
}