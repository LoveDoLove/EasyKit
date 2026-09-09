using EasyKit_Gui.Utilities;

namespace EasyKit_Gui.Views;

public class PullConfirmationDialog : Form
{
    private readonly Button _cancelButton;
    private readonly RichTextBox _changesBox;
    private readonly Button _confirmButton;
    private readonly Button _okButton;

    public PullConfirmationDialog()
    {
        Text = "Confirm Pull";
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
            Height = 400,
            BackColor = Color.FromArgb(40, 40, 60),
            ForeColor = Color.White,
            Font = new Font("Consolas", 10),
            BorderStyle = BorderStyle.FixedSingle,
            ScrollBars = RichTextBoxScrollBars.Vertical
        };

        _confirmButton = new Button
        {
            Text = "Confirm Pull",
            DialogResult = DialogResult.OK,
            Width = 120,
            Height = 32,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            Left = 440,
            Top = 420
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
            Left = 570,
            Top = 420
        };
        _cancelButton.Click += (s, e) =>
        {
            Confirmed = false;
            Close();
        };

        _okButton = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Width = 120,
            Height = 32,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            Left = 570,
            Top = 420,
            Visible = false
        };
        _okButton.Click += (s, e) =>
        {
            Confirmed = false;
            Close();
        };

        Controls.Add(_changesBox);
        Controls.Add(_confirmButton);
        Controls.Add(_cancelButton);
        Controls.Add(_okButton);
    }

    public bool Confirmed { get; private set; }

    public void SetChangesText(string ansiText)
    {
        AnsiColorParser.AppendAnsiText(_changesBox, ansiText);
    }

    // Call this after SetChangesText
    public void SetNoChangesMode(bool noChanges)
    {
        if (noChanges)
        {
            _confirmButton.Visible = false;
            _cancelButton.Visible = false;
            _okButton.Visible = true;
        }
        else
        {
            _confirmButton.Visible = true;
            _cancelButton.Visible = true;
            _okButton.Visible = false;
        }
    }
}