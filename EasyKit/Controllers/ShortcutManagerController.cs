using CommonUtilities.Helpers.ContextMenuManager;
using CommonUtilities.Models.Share;
using CommonUtilities.Utilities.System;
using EasyKit.Models;
using EasyKit.Services;
using EasyKit.UI.ConsoleUI;
using MenuScope = CommonUtilities.Helpers.ContextMenuManager.MenuScope;

// Keep for LoggerUtilities if used, or remove if not

namespace EasyKit.Controllers;

internal class ShortcutManagerController
{
    private const string EasyKitOpenFileId = "EasyKitOpenFile";
    private const string EasyKitOpenFolderId = "EasyKitOpenFolder";
    private const string EasyKitOpenBackgroundId = "EasyKitOpenBackground";
    private const string EasyKitOpenText = "Open with EasyKit";
    private readonly Config _config;
    private readonly ConsoleService _console;
    private readonly IContextMenuManager _contextMenuManager;
    private readonly PromptView _prompt;

    public ShortcutManagerController(Config config, ConsoleService console, PromptView prompt,
        IContextMenuManager contextMenuManager)
    {
        _config = config;
        _console = console;
        _prompt = prompt;
        _contextMenuManager = contextMenuManager;
    }

    // Note: ShowMenu is not part of the current refactoring task's scope for changes.
    // It currently calls ManageContextMenu which will be refactored.
    private void ShowMenu()
    {
        // Get user settings
        int menuWidth = 100;
        var menuWidthObj = _console.Config.Get("menu_width", 100);
        if (menuWidthObj is int mw)
            menuWidth = mw;
        // Remove color_scheme logic
        var menuView = new MenuView();
        menuView.CreateMenu("Shortcut Manager", width: menuWidth)
            .AddOption("1", "View Shortcuts", () => _console.WriteInfo("View Shortcuts - Not Implemented"))
            .AddOption("2", "Manage Context Menu", () => ManageContextMenuAsync().GetAwaiter().GetResult())
            .AddOption("0", "Back", () => { })
            .WithColors(MenuTheme.ColorScheme.Dark.border, MenuTheme.ColorScheme.Dark.highlight, MenuTheme.ColorScheme.Dark.title, MenuTheme.ColorScheme.Dark.text, MenuTheme.ColorScheme.Dark.help)
            .WithHelpText("Manage your keyboard shortcuts and context menu. Press 0 to return.")
            .WithSubtitle("Shortcut Manager")
            .Show();
    }

    public async Task ManageContextMenuAsync() // Changed to async
    {
        while (true)
        {
            // Use the config setting as the source of truth for whether the option is "on" or "off"
            bool isOpenWithEasyKitEnabled = _config.Get("open_with_easykit", false) is bool b && b;

            _console.WriteInfo("\nContext Menu:");
            _console.WriteInfo(
                $"  1. {(isOpenWithEasyKitEnabled ? "Remove" : "Add")} 'Open with EasyKit' option (Files, Folders, Background)");
            _console.WriteInfo("  0. Back");

            var choice = _prompt.Prompt("Select an option: ");
            switch (choice)
            {
                case "1":
                    await ToggleOpenWithEasyKitAsync(isOpenWithEasyKitEnabled);
                    break;
                case "0":
                    return; // Exit the loop and method
                default:
                    _console.WriteError("Invalid option. Please try again.");
                    break;
            }
        }
    }

    private async Task ToggleOpenWithEasyKitAsync(bool currentlyEnabled)
    {
        // Use Environment.ProcessPath (preferred in .NET 6+) for the executable path
        string exePath = System.Environment.ProcessPath ?? "EasyKit.exe";
        // Prefer a dedicated icon file if available, otherwise fallback to exePath
        string iconPath = exePath;
        string iconCandidate = Path.Combine(AppContext.BaseDirectory, "icon.ico");
        if (File.Exists(iconCandidate))
            iconPath = iconCandidate;

        var scopeString = _config.Get("context_menu_scope", "user")?.ToString()?.ToLowerInvariant() ?? "user";
        MenuScope scope = scopeString == "system" ? MenuScope.System : MenuScope.User;

        var entriesToManage = new List<(string Id, TargetType TargetType)>
        {
            (EasyKitOpenFileId, TargetType.File),
            (EasyKitOpenFolderId, TargetType.Folder),
            (EasyKitOpenBackgroundId, TargetType.Background)
        };

        try
        {
            if (!currentlyEnabled) // Add the entries
            {
                _console.WriteInfo("Adding 'Open with EasyKit' context menu entries...");
                foreach (var entryDef in entriesToManage)
                {
                    var contextMenuEntry = new ContextMenuEntry
                    {
                        Id = entryDef.Id,
                        Text = EasyKitOpenText,
                        Command = exePath,
                        Arguments = string.Empty, // No additional arguments beyond the path placeholder handled by the manager
                        IconPath = iconPath,
                        Scope = scope,
                        TargetType = entryDef.TargetType
                    };
                    await _contextMenuManager.AddEntryAsync(contextMenuEntry);
                }

                _config.Set("open_with_easykit", true);
                _console.WriteSuccess("'Open with EasyKit' option added successfully.");
            }
            else // Remove the entries
            {
                _console.WriteInfo("Removing 'Open with EasyKit' context menu entries...");
                foreach (var entryDef in entriesToManage) await _contextMenuManager.RemoveEntryAsync(entryDef.Id);
                _config.Set("open_with_easykit", false);
                _console.WriteSuccess("'Open with EasyKit' option removed successfully.");
            }

            _config.SaveConfig();
        }
        catch (PlatformNotSupportedException ex)
        {
            _console.WriteError($"Operation failed: {ex.Message}");
            LoggerUtilities.Error(ex, "ContextMenu operation failed due to platform not supported.");
        }
        catch (UnauthorizedAccessException ex)
        {
            _console.WriteError(
                $"Permission denied. Please ensure EasyKit has the necessary permissions (e.g., run as administrator for system-wide changes). Details: {ex.Message}");
            LoggerUtilities.Error(ex, "ContextMenu operation failed due to UnauthorizedAccessException.");
        }
        catch (Exception ex)
        {
            _console.WriteError($"An error occurred: {ex.Message}");
            LoggerUtilities.Error(ex, "ContextMenu operation failed.");
        }
    }
}