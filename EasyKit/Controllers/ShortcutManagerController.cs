using System.Diagnostics;
using CommonUtilities.Models.Enums;
using CommonUtilities.Models.Share;
using CommonUtilities.Services.ContextMenuManager;
using CommonUtilities.UI.ConsoleUI;
using CommonUtilities.Utilities.System;
using EasyKit.Models;
using EasyKit.Services;

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
        int menuWidth = 50;
        string colorSchemeStr = "dark";
        var menuWidthObj = _console.Config.Get("menu_width", 50);
        if (menuWidthObj is int mw)
            menuWidth = mw;
        var colorSchemeObj = _console.Config.Get("color_scheme", "dark");
        if (colorSchemeObj != null)
            colorSchemeStr = colorSchemeObj.ToString() ?? "dark";
        var colorScheme = MenuTheme.ColorScheme.Dark;
        if (colorSchemeStr.ToLower() == "light")
            colorScheme = MenuTheme.ColorScheme.Light;
        var menuView = new MenuView();
        menuView.CreateMenu("Shortcut Manager", width: menuWidth)
            .AddOption("1", "View Shortcuts", () => _console.WriteInfo("View Shortcuts - Not Implemented"))
            .AddOption("2", "Manage Context Menu", () => ManageContextMenuAsync().GetAwaiter().GetResult())
            .AddOption("0", "Back", () => { })
            .WithColors(colorScheme.border, colorScheme.highlight, colorScheme.title, colorScheme.text,
                colorScheme.help)
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
        string exePath = Process.GetCurrentProcess().MainModule?.FileName ?? "EasyKit.exe";
        // Use exePath itself as the icon source, or specify a dedicated .ico/.png if available
        string iconPath = exePath;

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
                        Arguments = null, // No additional arguments beyond the path placeholder handled by the manager
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