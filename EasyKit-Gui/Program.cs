using EasyKit_Gui.Views;

namespace EasyKit_Gui;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        // Auto-detect working directory from command-line argument (if any)
        string? originalArg = args.Length > 0 ? args[0] : null;
        if (!string.IsNullOrEmpty(originalArg))
            try
            {
                if (Directory.Exists(originalArg))
                {
                    Environment.CurrentDirectory = originalArg;
                }
                else if (File.Exists(originalArg))
                {
                    string? directory = Path.GetDirectoryName(originalArg);
                    if (!string.IsNullOrEmpty(directory))
                        Environment.CurrentDirectory = directory;
                }
            }
            catch
            {
                // Ignore errors, fallback to default directory
            }

        ApplicationConfiguration.Initialize();
        Application.Run(new HomePage(Environment.CurrentDirectory));
    }
}