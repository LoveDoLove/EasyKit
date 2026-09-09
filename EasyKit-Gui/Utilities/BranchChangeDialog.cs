namespace EasyKit_Gui.Utilities;

public class BranchChangeDialog : Form
{
    public enum BranchChangeAction
    {
        None,
        StageCommit,
        Stash,
        Discard,
        Bring
    }

    private readonly Button _bringButton;
    private readonly Button _cancelButton;
    private readonly RichTextBox _changesBox;
    private readonly Button _discardButton;
    private readonly Button _stageCommitButton;
    private readonly Button _stashButton;

    public BranchChangeDialog(IEnumerable<string> changedFiles)
    {
        Text = "Uncommitted Changes Detected";
        Size = new Size(700, 500);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        _changesBox = new RichTextBox
        {
            Multiline = true,
            ReadOnly = true,
            Dock = DockStyle.Top,
            Height = 350,
            BackColor = Color.FromArgb(40, 40, 60),
            ForeColor = Color.White,
            Font = new Font("Consolas", 10),
            BorderStyle = BorderStyle.FixedSingle,
            ScrollBars = RichTextBoxScrollBars.Vertical
        };
        foreach (var file in changedFiles)
            _changesBox.AppendText(file + Environment.NewLine);

        _stageCommitButton = new Button
        {
            Text = "Stage & Commit",
            Width = 140,
            Height = 32,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
            Left = 20,
            Top = 380
        };
        _stageCommitButton.Click += (s, e) =>
        {
            Action = BranchChangeAction.StageCommit;
            Close();
        };

        _stashButton = new Button
        {
            Text = "Stash Changes",
            Width = 140,
            Height = 32,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
            Left = 180,
            Top = 380
        };
        _stashButton.Click += (s, e) =>
        {
            Action = BranchChangeAction.Stash;
            Close();
        };

        _discardButton = new Button
        {
            Text = "Discard Changes",
            Width = 140,
            Height = 32,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
            Left = 340,
            Top = 380
        };
        _discardButton.Click += (s, e) =>
        {
            Action = BranchChangeAction.Discard;
            Close();
        };

        _bringButton = new Button
        {
            Text = "Bring Changes to New Branch",
            Width = 200,
            Height = 32,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
            Left = 500,
            Top = 380
        };
        _bringButton.Click += (s, e) =>
        {
            Action = BranchChangeAction.Bring;
            Close();
        };

        _cancelButton = new Button
        {
            Text = "Cancel",
            Width = 120,
            Height = 32,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            Left = 560,
            Top = 420
        };
        _cancelButton.Click += (s, e) =>
        {
            Action = BranchChangeAction.None;
            Close();
        };

        Controls.Add(_changesBox);
        Controls.Add(_stageCommitButton);
        Controls.Add(_stashButton);
        Controls.Add(_discardButton);
        Controls.Add(_bringButton);
        Controls.Add(_cancelButton);
    }

    public BranchChangeAction Action { get; private set; } = BranchChangeAction.None;
}