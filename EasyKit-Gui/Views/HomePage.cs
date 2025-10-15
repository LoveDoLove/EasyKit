using EasyKit_Gui.Views.Modules;

namespace EasyKit_Gui.Views;

public partial class HomePage : Form
{
    private readonly UserControl _composerControl;
    private readonly UserControl _gitControl;
    private readonly UserControl _laravelControl;
    private readonly UserControl _npmControl;
    private readonly UserControl _settingsControl;
    private readonly UserControl _toolMarketplaceControl;

    public HomePage()
    {
        InitializeComponent();
        // Initialize UserControls (actual module controls)
        _gitControl = new GitControl();
        _composerControl = new ComposerControl();
        _laravelControl = new LaravelControl();
        _npmControl = new NpmControl();
        _toolMarketplaceControl = new ToolMarketplaceControl();
        _settingsControl = new SettingsControl();

        // Wire up navigation
        navListBox.SelectedIndexChanged += NavListBox_SelectedIndexChanged;
        // Load default module
        LoadModule(0);
    }

    private void NavListBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        LoadModule(navListBox.SelectedIndex);
    }

    private void LoadModule(int index)
    {
        mainPanel.Controls.Clear();
        switch (index)
        {
            case 0:
                mainPanel.Controls.Add(_gitControl);
                break;
            case 1:
                mainPanel.Controls.Add(_composerControl);
                break;
            case 2:
                mainPanel.Controls.Add(_laravelControl);
                break;
            case 3:
                mainPanel.Controls.Add(_npmControl);
                break;
            case 4:
                mainPanel.Controls.Add(_toolMarketplaceControl);
                break;
            case 5:
                mainPanel.Controls.Add(_settingsControl);
                break;
        }
    }
}