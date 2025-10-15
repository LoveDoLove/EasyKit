namespace EasyKit_Gui.Utilities;

public class AddFilesDialog : Form
{
    private readonly Button _cancelButton;
    private readonly Button _confirmButton;
    private readonly CheckedListBox _filesList;
    private readonly CheckBox _selectAllBox;

    public AddFilesDialog(IEnumerable<string> files)
    {
        Text = "Select Files to Add";
        Size = new Size(600, 500);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        _filesList = new CheckedListBox
        {
            Dock = DockStyle.Top,
            Height = 370,
            CheckOnClick = true,
            Font = new Font("Consolas", 10)
        };
        foreach (var file in files)
            _filesList.Items.Add(file, true);

        _selectAllBox = new CheckBox
        {
            Text = "Select All",
            Dock = DockStyle.Top,
            Height = 24,
            Checked = true
        };
        _selectAllBox.CheckedChanged += (s, e) =>
        {
            for (int i = 0; i < _filesList.Items.Count; i++)
                _filesList.SetItemChecked(i, _selectAllBox.Checked);
        };

        _confirmButton = new Button
        {
            Text = "Confirm Add",
            DialogResult = DialogResult.OK,
            Width = 120,
            Height = 32,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            Left = 340,
            Top = 400
        };
        _confirmButton.Click += (s, e) =>
        {
            Confirmed = true;
            SelectedFiles.Clear();
            foreach (var item in _filesList.CheckedItems)
                SelectedFiles.Add(item.ToString());
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
            Top = 400
        };
        _cancelButton.Click += (s, e) =>
        {
            Confirmed = false;
            Close();
        };

        Controls.Add(_filesList);
        Controls.Add(_selectAllBox);
        Controls.Add(_confirmButton);
        Controls.Add(_cancelButton);
    }

    public List<string> SelectedFiles { get; } = new();
    public bool Confirmed { get; private set; }
}