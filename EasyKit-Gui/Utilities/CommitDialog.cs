namespace EasyKit_Gui.Utilities;

public class CommitDialog : Form
{
    private readonly Button _cancelButton;
    private readonly Button _confirmButton;
    private readonly ListBox _filesList;
    private readonly TextBox _messageBox;
    private readonly TextBox _titleBox;

    public CommitDialog(IEnumerable<string> stagedFiles)
    {
        Text = "Commit Changes";
        Size = new Size(600, 600);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        var titleLabel = new Label
        {
            Text = "Title:",
            Dock = DockStyle.Top,
            Height = 20,
            ForeColor = Color.White,
            BackColor = Color.Transparent
        };
        _titleBox = new TextBox
        {
            Dock = DockStyle.Top,
            Height = 24,
            Font = new Font("Segoe UI", 10)
        };
        var messageLabel = new Label
        {
            Text = "Message:",
            Dock = DockStyle.Top,
            Height = 20,
            ForeColor = Color.White,
            BackColor = Color.Transparent
        };
        _messageBox = new TextBox
        {
            Dock = DockStyle.Top,
            Height = 80,
            Multiline = true,
            Font = new Font("Segoe UI", 10)
        };
        var filesLabel = new Label
        {
            Text = "Staged Files:",
            Dock = DockStyle.Top,
            Height = 20,
            ForeColor = Color.White,
            BackColor = Color.Transparent
        };
        _filesList = new ListBox
        {
            Dock = DockStyle.Top,
            Height = 300,
            Font = new Font("Consolas", 10)
        };
        foreach (var file in stagedFiles)
            _filesList.Items.Add(file);

        _confirmButton = new Button
        {
            Text = "Commit",
            DialogResult = DialogResult.OK,
            Width = 120,
            Height = 32,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            Left = 340,
            Top = 500
        };
        _confirmButton.Click += (s, e) =>
        {
            Confirmed = true;
            Close();
        };

        _cancelButton = new Button
        {
            Text = "Cancel",
            DialogResult = DialogResult.Cancel,
            Width = 120,
            Height = 32,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            Left = 470,
            Top = 500
        };
        _cancelButton.Click += (s, e) =>
        {
            Confirmed = false;
            Close();
        };

        Controls.Add(_filesList);
        Controls.Add(filesLabel);
        Controls.Add(_messageBox);
        Controls.Add(messageLabel);
        Controls.Add(_titleBox);
        Controls.Add(titleLabel);
        Controls.Add(_confirmButton);
        Controls.Add(_cancelButton);
        BackColor = Color.FromArgb(60, 80, 120);
    }

    public string CommitTitle => _titleBox.Text.Trim();
    public string CommitMessage => _messageBox.Text.Trim();
    public bool Confirmed { get; private set; }
}