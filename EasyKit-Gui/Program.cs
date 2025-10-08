using EasyKit_Gui.Views;

namespace EasyKit_Gui;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new HomePage());
    }
}