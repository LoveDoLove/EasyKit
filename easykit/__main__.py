"""
Main entry point for EasyKit when run as a module
"""
import sys
import os
from pathlib import Path
import webbrowser
from typing import Optional
import click
from rich.panel import Panel
from rich.prompt import Prompt, Confirm
from rich.table import Table
from rich import box
from .core.config import Config
from .core.software import SoftwareChecker
from .utils import draw_header, get_logger, confirm_action, get_console
from packaging import version

logger = get_logger(__name__)
config = Config()

def clear_screen():
    """Clear the terminal screen."""
    os.system('cls')

def main_menu():
    """Display the main menu and handle user input"""
    while True:
        clear_screen()
        draw_header(f"EasyKit Main Menu v{config.get('version', '3.1.9')}")
        console = get_console()
        
        # Create menu table
        table = Table(show_header=False, box=box.ROUNDED)
        table.add_column("Option", style="cyan")
        table.add_column("Description")
        
        table.add_row("0", "Exit")
        table.add_row("1", "NPM Tools")
        table.add_row("2", "Laravel Tools")
        table.add_row("3", "Composer Tools")
        table.add_row("4", "Git Tools")
        table.add_row("5", "Create Shortcuts")
        table.add_row("6", "Settings")
        table.add_row("7", "Update Manager")
        
        get_console().print(table)
        
        if config.get('show_tips', True):
            get_console().print("\n[yellow]TIP:[/yellow] You can customize EasyKit through the Settings menu.")
        
        choice = Prompt.ask("\nChoose an option", choices=["0", "1", "2", "3", "4", "5", "6", "7"])
        
        if choice == "0":
            if exit_program():
                break
        elif choice == "1":
            npm_menu()
        elif choice == "2":
            laravel_menu()
        elif choice == "3":
            composer_menu()
        elif choice == "4":
            git_menu()
        elif choice == "5":
            create_shortcuts()
        elif choice == "6":
            settings_menu()
        elif choice == "7":
            update_manager()

def settings_menu():
    """Display and handle the settings menu"""
    while True:
        clear_screen()
        draw_header("EasyKit Settings")
        console = get_console()
        
        table = Table(show_header=False, box=box.ROUNDED)
        table.add_column("Option", style="cyan")
        table.add_column("Setting", style="green")
        table.add_column("Current Value", style="yellow")
        
        table.add_row("0", "Back to Main Menu", "")
        table.add_row("1", "Toggle Tips", str(config.get('show_tips', True)))
        table.add_row("2", "Toggle Logging", str(config.get('enable_logging', True)))
        table.add_row("3", "Toggle Update Checking", str(config.get('check_updates', True)))
        table.add_row("4", "Change Color Theme", config.get('color_scheme', 'dark'))
        table.add_row("5", "View Logs", "")
        
        get_console().print(table)
        
        choice = Prompt.ask("\nChoose an option", choices=["0", "1", "2", "3", "4", "5"])
        
        if choice == "0":
            break
        elif choice == "1":
            config.set('show_tips', not config.get('show_tips', True))
        elif choice == "2":
            config.set('enable_logging', not config.get('enable_logging', True))
        elif choice == "3":
            config.set('check_updates', not config.get('check_updates', True))
        elif choice == "4":
            change_color_theme()
        elif choice == "5":
            view_logs()

def change_color_theme():
    """Change the color theme settings"""
    clear_screen()
    draw_header("Change Color Theme")
    console = get_console()
    
    themes = ["dark", "light", "system"]
    current_theme = config.get('color_scheme', 'dark')
    
    table = Table(title="Available Themes", box=box.ROUNDED)
    table.add_column("Theme", style="cyan")
    table.add_column("Description")
    
    table.add_row("dark", "Dark mode (easier on the eyes)")
    table.add_row("light", "Light mode (high contrast)")
    table.add_row("system", "Follow system settings")
    
    get_console().print(table)
    get_console().print(f"\nCurrent theme: [green]{current_theme}[/green]")
    
    new_theme = Prompt.ask(
        "\nChoose a theme",
        choices=themes,
        default=current_theme
    )
    
    if new_theme != current_theme:
        config.set('color_scheme', new_theme)
        get_console().print(f"\n[green]Theme changed to {new_theme}[/green]")

def view_logs():
    """View the application logs"""
    log_path = Path(config.get('log_path')) / 'eskit.log'
    console = get_console()
    if log_path.exists():
        if sys.platform == 'win32':
            os.startfile(str(log_path))
        else:
            # Use appropriate commands for other platforms
            if sys.platform == 'darwin':  # macOS
                os.system(f'open "{log_path}"')
            else:  # Linux and others
                os.system(f'xdg-open "{log_path}"')
    else:
        get_console().print("[yellow]No logs found.[/yellow]")
        Prompt.ask("Press Enter to continue")

def exit_program() -> bool:
    """Handle program exit with confirmation if enabled"""
    if config.get('enable_logging', True):
        logger.info("Exiting EasyKit")
    
    if config.get('confirm_exit', True):
        if not confirm_action("Are you sure you want to exit?"):
            return False
    
    console = get_console()
    get_console().print("[green]Thank you for using EasyKit![green]")
    return True

def npm_menu():
    """NPM tools menu"""
    from .tools.npm import NpmTools
    npm_tools = NpmTools()
    
    while True:
        clear_screen()
        draw_header("NPM Tools")
        console = get_console()
        console.print(f"[bold yellow]Current Path:[/bold yellow] {os.getcwd()}")
        
        table = Table(show_header=False, box=box.ROUNDED)
        table.add_column("Option", style="cyan")
        table.add_column("Description")
        
        table.add_row("0", "Back to Main Menu")
        table.add_row("1", "Install NPM Packages")
        table.add_row("2", "Update NPM Packages")
        table.add_row("3", "Build for Production")
        table.add_row("4", "Build for Development")
        table.add_row("5", "Run Security Audit")
        table.add_row("6", "Run Custom Script")
        table.add_row("7", "Show Package Info")
        table.add_row("")
        table.add_row("[red]9[/red]", "[red]Reset All Cache[/red]")
        
        get_console().print()  # Print a newline
        get_console().print(table)
        
        choice = Prompt.ask(
            "\nChoose an option",
            choices=["0", "1", "2", "3", "4", "5", "6", "7", "9"]
        )
        
        try:
            if choice == "0":
                break
            elif choice == "1":
                npm_tools.install_packages()
            elif choice == "2":
                npm_tools.update_packages()
            elif choice == "3":
                npm_tools.build_production()
            elif choice == "4":
                npm_tools.build_development()
            elif choice == "5":
                npm_tools.security_audit()
            elif choice == "6":
                npm_tools.run_custom_script()
            elif choice == "7":
                npm_tools.show_package_info()
            elif choice == "9":
                npm_tools.reset_cache()
        except Exception as e:
            logger.exception("Error in NPM tools")
            get_console().print(f"[red]Error: {str(e)}[/red]")
        
        Prompt.ask("\nPress Enter to continue")

def laravel_menu():
    """Laravel tools menu"""
    from .tools.laravel import LaravelTools
    laravel_tools = LaravelTools()
    
    while True:
        clear_screen()
        draw_header("Laravel Tools")
        console = get_console()
        console.print(f"[bold yellow]Current Path:[/bold yellow] {os.getcwd()}")
        
        table = Table(show_header=False, box=box.ROUNDED)
        table.add_column("Option", style="cyan")
        table.add_column("Description")
        
        table.add_row("0", "Back to Main Menu")
        table.add_row("1", "Quick Auto Setup")
        table.add_row("2", "Install Laravel Packages")
        table.add_row("3", "Update Laravel Packages")
        table.add_row("4", "Regenerate Composer AutoLoad Files")
        table.add_row("5", "Build To Production")
        table.add_row("6", "Run Development Server")
        table.add_row("7", "Create Storage Link")
        table.add_row("8", "Run Database Seeding")
        table.add_row("9", "Test Database Connection")
        table.add_row("10", "Laravel Sail Commands (Docker)")
        table.add_row("")
        table.add_row("11", "Check PHP Version")
        table.add_row("12", "Check Laravel Configuration")
        table.add_row("")
        table.add_row("[red]44[/red]", "[red]Reset All Cache[/red]")
        
        get_console().print()  # Print a newline
        get_console().print(table)
        
        choice = Prompt.ask(
            "\nChoose an option",
            choices=["0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "44"]
        )
        
        try:
            if choice == "0":
                break
            elif choice == "1":
                laravel_tools.quick_setup()
            elif choice == "2":
                laravel_tools.install_packages()
            elif choice == "3":
                laravel_tools.update_packages()
            elif choice == "4":
                laravel_tools.regenerate_autoload()
            elif choice == "5":
                laravel_tools.build_production()
            elif choice == "6":
                laravel_tools.run_dev_server()
            elif choice == "7":
                laravel_tools.create_storage_link()
            elif choice == "8":
                laravel_tools.run_database_seeding()
            elif choice == "9":
                laravel_tools.test_database()
            elif choice == "10":
                laravel_tools.sail_menu()
            elif choice == "11":
                laravel_tools.check_php_version()
            elif choice == "12":
                laravel_tools.check_configuration()
            elif choice == "44":
                laravel_tools.reset_cache()
        except Exception as e:
            logger.exception("Error in Laravel tools")
            get_console().print(f"[red]Error: {str(e)}[/red]")
        
        if choice != "6":  # Don't prompt if running dev server (it has its own exit mechanism)
            Prompt.ask("\nPress Enter to continue")

def composer_menu():
    """Composer tools menu"""
    from .tools.composer import ComposerTools
    composer_tools = ComposerTools()
    
    while True:
        clear_screen()
        draw_header("Composer Tools")
        console = get_console()
        console.print(f"[bold yellow]Current Path:[/bold yellow] {os.getcwd()}")
        
        table = Table(show_header=False, box=box.ROUNDED)
        table.add_column("Option", style="cyan")
        table.add_column("Description")
        
        table.add_row("0", "Back to Main Menu")
        table.add_row("1", "Install Packages")
        table.add_row("2", "Update Packages")
        table.add_row("3", "Regenerate AutoLoad Files")
        table.add_row("4", "Require New Package")
        table.add_row("5", "Create New Project")
        table.add_row("6", "Validate composer.json")
        table.add_row("7", "Show Package Info")
        table.add_row("")
        table.add_row("[red]44[/red]", "[red]Clear Composer Cache[/red]")
        
        get_console().print()  # Print a newline
        get_console().print(table)
        
        choice = Prompt.ask(
            "\nChoose an option",
            choices=["0", "1", "2", "3", "4", "5", "6", "7", "44"]
        )
        
        try:
            if choice == "0":
                break
            elif choice == "1":
                composer_tools.install_packages()
            elif choice == "2":
                composer_tools.update_packages()
            elif choice == "3":
                composer_tools.regenerate_autoload()
            elif choice == "4":
                composer_tools.require_package()
            elif choice == "5":
                composer_tools.create_project()
            elif choice == "6":
                composer_tools.validate_json()
            elif choice == "7":
                composer_tools.show_package_info()
            elif choice == "44":
                composer_tools.clear_cache()
        except Exception as e:
            logger.exception("Error in Composer tools")
            get_console().print(f"[red]Error: {str(e)}[/red]")
        
        Prompt.ask("\nPress Enter to continue")

def git_menu():
    """Git tools menu"""
    from .tools.git import GitTools
    git_tools = GitTools()
    
    while True:
        clear_screen()
        draw_header("Git Tools")
        console = get_console()
        console.print(f"[bold yellow]Current Path:[/bold yellow] {os.getcwd()}")
        
        table = Table(show_header=False, box=box.ROUNDED)
        table.add_column("Option", style="cyan")
        table.add_column("Description")
        
        table.add_row("0", "Back to Main Menu")
        table.add_row("1", "Initialize Repository")
        table.add_row("2", "Check Status")
        table.add_row("3", "Add All Changes")
        table.add_row("4", "Commit Changes")
        table.add_row("5", "Push to Origin")
        table.add_row("6", "Pull from Origin")
        table.add_row("7", "Create New Branch")
        table.add_row("8", "Switch Branch")
        table.add_row("9", "Merge Branch")
        table.add_row("")
        table.add_row("[bold]Advanced:[/bold]", "")
        table.add_row("10", "View Commit History")
        table.add_row("11", "Stash Changes")
        table.add_row("12", "Apply Stash")
        table.add_row("13", "Create Pull Request")
        table.add_row("14", "List Pull Requests")
        
        get_console().print()  # Print a newline
        get_console().print(table)
        
        choice = Prompt.ask(
            "\nChoose an option",
            choices=["0", "1", "2", "3", "4", "5", "6", "7", "8", "9", 
                    "10", "11", "12", "13", "14"]
        )
        
        try:
            if choice == "0":
                break
            elif choice == "1":
                git_tools.init_repo()
            elif choice == "2":
                git_tools.check_status()
            elif choice == "3":
                git_tools.add_all()
            elif choice == "4":
                git_tools.commit()
            elif choice == "5":
                git_tools.push()
            elif choice == "6":
                git_tools.pull()
            elif choice == "7":
                git_tools.create_branch()
            elif choice == "8":
                git_tools.switch_branch()
            elif choice == "9":
                git_tools.merge_branch()
            elif choice == "10":
                git_tools.view_history()
            elif choice == "11":
                git_tools.stash_changes()
            elif choice == "12":
                git_tools.apply_stash()
            elif choice == "13":
                git_tools.create_pull_request()
            elif choice == "14":
                git_tools.list_pull_requests()
        except Exception as e:
            logger.exception("Error in Git tools")
            get_console().print(f"[red]Error: {str(e)}[/red]")
        
        if choice not in ["6"]:  # Don't prompt if running long operations
            Prompt.ask("\nPress Enter to continue")

def create_shortcuts():
    """Create desktop/start menu shortcuts"""
    from .core.shortcuts import ShortcutManager
    shortcut_manager = ShortcutManager()

    while True:
        clear_screen()
        draw_header("Shortcut Manager")
        console = get_console()

        # Show current status
        shortcut_manager.show_shortcut_info()

        # Show menu (grouped: Add/Create, then Remove)
        table = Table(show_header=False, box=box.ROUNDED)
        table.add_column("Option", style="cyan")
        table.add_column("Description")

        table.add_row("0", "Back to Main Menu")
        # --- Add/Create Section ---
        table.add_row("1", "Create Desktop Shortcut")
        table.add_row("2", "Create Start Menu Shortcut")
        table.add_row("3", "Create Both Shortcuts")
        table.add_row("4", "Add Context Menu Entry (Right-Click)")
        table.add_row("")
        # --- Remove Section ---
        table.add_row("5", "Remove Desktop Shortcut")
        table.add_row("6", "Remove Start Menu Shortcut")
        table.add_row("7", "Remove Context Menu Entry")
        table.add_row("8", "Remove All Shortcuts")

        get_console().print("\n")
        get_console().print(table)

        choice = Prompt.ask(
            "\nChoose an option",
            choices=["0", "1", "2", "3", "4", "5", "6", "7", "8"]
        )

        try:
            if choice == "0":
                break
            elif choice == "1":
                shortcut_manager.create_desktop_shortcut()
            elif choice == "2":
                shortcut_manager.create_start_menu_shortcut()
            elif choice == "3":
                shortcut_manager.create_desktop_shortcut()
                shortcut_manager.create_start_menu_shortcut()
            elif choice == "4":
                if confirm_action("Add EasyKit to right-click context menu?", default=True):
                    shortcut_manager.add_context_menu_entry()
            elif choice == "5":
                if confirm_action("Remove desktop shortcut? [Y/n]", default=False):
                    shortcut_manager.remove_desktop_shortcut()
            elif choice == "6":
                if confirm_action("Remove Start Menu shortcut? [Y/n]", default=False):
                    shortcut_manager.remove_start_menu_shortcut()
            elif choice == "7":
                if confirm_action("Remove context menu entry? [Y/n]", default=False):
                    shortcut_manager.remove_context_menu_entry()
            elif choice == "8":
                if confirm_action("Remove all shortcuts? [Y/n]", default=False):
                    shortcut_manager.remove_desktop_shortcut()
                    shortcut_manager.remove_start_menu_shortcut()
                    shortcut_manager.remove_context_menu_entry()
        except Exception as e:
            logger.exception("Error in shortcut manager")
            get_console().print(f"[red]Error: {str(e)}[/red]")

        Prompt.ask("\nPress Enter to continue")

def update_manager():
    """Handle updates and backups"""
    from .core.update import UpdateManager
    update_manager = UpdateManager()
    
    while True:
        clear_screen()
        draw_header("Update Manager")
        console = get_console()
        
        table = Table(show_header=False, box=box.ROUNDED)
        table.add_column("Option", style="cyan")
        table.add_column("Description")
        
        table.add_row("0", "Back to Main Menu")
        table.add_row("1", "Check for Updates")
        table.add_row("2", "Backup Current Scripts")
        table.add_row("3", "View/Restore Backups")
        table.add_row("4", "View Release Notes")
        
        get_console().print(table)
        
        choice = Prompt.ask(
            "\nChoose an option",
            choices=["0", "1", "2", "3", "4"]
        )
        
        try:
            if choice == "0":
                break
            elif choice == "1":
                # Check for updates
                get_console().print(f"\nCurrent version: [cyan]{update_manager.current_version}[cyan]")
                update_info = update_manager.check_for_updates()
                latest_version = update_info['version']
                
                if version.parse(latest_version) > version.parse(update_manager.current_version):
                    get_console().print(f"[green]New version available: {latest_version}[/green]")
                    get_console().print("\n[bold]Release Notes:[/bold]")
                    get_console().print(Markdown(update_info['notes']))
                    
                    if confirm_action("Would you like to download and install the update?"):
                        # Create temp directory for download
                        with tempfile.TemporaryDirectory() as temp_dir:
                            temp_path = Path(temp_dir) / "EasyKit_Setup.exe"
                            
                            # Download installer
                            if update_info['installer_url'] and update_manager.download_update(
                                update_info['installer_url'],
                                temp_path
                            ):
                                # Create backup before updating
                                if confirm_action("Create backup before updating?", default=True):
                                    update_manager.backup_scripts()
                                
                                # Install update
                                update_manager.install_update(temp_path)
                                get_console().print("\n[yellow]Please restart EasyKit after installation.[/yellow]")
                                if confirm_action("Exit EasyKit now?", default=True):
                                    sys.exit(0)
                else:
                    get_console().print("[green]You have the latest version![/green]")
            
            elif choice == "2":
                update_manager.backup_scripts()
            
            elif choice == "3":
                while True:
                    clear_screen()
                    draw_header("Backup Management")
                    
                    update_manager.show_backup_list()
                    
                    backup_menu = Table(show_header=False, box=box.ROUNDED)
                    backup_menu.add_column("Option", style="cyan")
                    backup_menu.add_column("Description")
                    
                    backup_menu.add_row("0", "Back")
                    backup_menu.add_row("1", "Restore Backup")
                    
                    get_console().print("\n")
                    get_console().print(backup_menu)
                    
                    backup_choice = Prompt.ask(
                        "\nChoose an option",
                        choices=["0", "1"]
                    )
                    
                    if backup_choice == "0":
                        break
                    elif backup_choice == "1":
                        backups = update_manager.list_backups()
                        if backups:
                            backup_dates = [b['timestamp'] for b in backups]
                            date = Prompt.ask(
                                "Enter backup date to restore",
                                choices=backup_dates
                            )
                            backup = next(b for b in backups if b['timestamp'] == date)
                            
                            if confirm_action(f"Restore backup from {date}?", default=False):
                                update_manager.restore_backup(backup['path'])
            
            elif choice == "4":
                clear_screen()
                draw_header("Release Notes")
                update_manager.show_release_notes()
        
        except Exception as e:
            logger.exception("Error in update manager")
            get_console().print(f"[red]Error: {str(e)}[/red]")
        
        Prompt.ask("\nPress Enter to continue")

if __name__ == '__main__':
    try:
        main_menu()
    except KeyboardInterrupt:
        console = get_console()
        get_console().print("\n[yellow]Interrupted by user[/yellow]")
        sys.exit(0)
    except Exception as e:
        logger.exception("An unexpected error occurred")
        console = get_console()
        get_console().print(f"[red]An error occurred: {e}[/red]")
        sys.exit(1)
