"""
Shortcut creation and management utilities
"""
import os
import sys
from pathlib import Path
import winreg
import pythoncom
from win32com.client import Dispatch
import subprocess
from typing import Optional
from rich.console import Console
from rich.table import Table
from rich.prompt import Prompt, Confirm
from rich import box
from ..utils import draw_header, get_logger, confirm_action
from ..core.config import Config
import ctypes

console = Console()
logger = get_logger(__name__)
config = Config()

class ShortcutManager:
    def __init__(self):
        self.app_name = "EasyKit"
        self.script_path = Path(__file__).parent.parent.parent / "run_easykit.py"
        self.icon_path = Path(__file__).parent.parent.parent / "images" / "icon.ico"
        self.working_dir = self.script_path.parent
        
        # Ensure we have python path
        self.python_path = self._get_python_path()
    
    @staticmethod
    def is_admin() -> bool:
        """Check if the current process has admin rights (Windows only)"""
        try:
            return ctypes.windll.shell32.IsUserAnAdmin() != 0
        except Exception:
            return False

    @staticmethod
    def run_as_admin():
        """Rerun the current script as admin using ShellExecuteEx"""
        import sys
        import os
        if sys.platform != 'win32':
            return False
        params = ' '.join([f'"{arg}"' for arg in sys.argv])
        try:
            ctypes.windll.shell32.ShellExecuteW(
                None, "runas", sys.executable, params, os.getcwd(), 1
            )
            return True
        except Exception:
            return False

    def _get_python_path(self) -> str:
        """Get the path to the Python interpreter"""
        if getattr(sys, 'frozen', False):
            # If we're running as a bundled exe, use the current executable
            return str(Path(sys.executable))
        else:
            # Otherwise use the current Python interpreter
            return str(Path(sys.executable))
    
    def _create_shortcut(self, shortcut_path: Path) -> bool:
        """Create a Windows shortcut"""
        try:
            pythoncom.CoInitialize()
            shell = Dispatch('WScript.Shell')
            shortcut = shell.CreateShortCut(str(shortcut_path))
            
            # Set shortcut properties
            shortcut.TargetPath = self.python_path
            shortcut.Arguments = f'"{self.script_path}"'
            shortcut.WorkingDirectory = str(self.working_dir)
            if self.icon_path.exists():
                shortcut.IconLocation = str(self.icon_path)
            shortcut.Save()
            
            return True
        except Exception as e:
            logger.error(f"Error creating shortcut: {e}")
            return False
        finally:
            pythoncom.CoUninitialize()
    
    def _remove_shortcut(self, shortcut_path: Path) -> bool:
        """Remove a Windows shortcut"""
        try:
            if shortcut_path.exists():
                shortcut_path.unlink()
            return True
        except Exception as e:
            logger.error(f"Error removing shortcut: {e}")
            return False
    
    def create_desktop_shortcut(self) -> bool:
        """Create a desktop shortcut"""
        desktop_path = Path(os.path.expanduser("~/Desktop"))
        shortcut_path = desktop_path / f"{self.app_name}.lnk"
        
        with console.status("[bold green]Creating desktop shortcut..."):
            success = self._create_shortcut(shortcut_path)
        
        if success:
            console.print("[green]✓[/green] Desktop shortcut created!")
        else:
            console.print("[red]✗[/red] Failed to create desktop shortcut.")
        
        return success
    
    def create_start_menu_shortcut(self) -> bool:
        """Create a Start Menu shortcut"""
        start_menu_path = Path(os.getenv('APPDATA')) / "Microsoft/Windows/Start Menu/Programs"
        shortcut_path = start_menu_path / f"{self.app_name}.lnk"
        
        with console.status("[bold green]Creating Start Menu shortcut..."):
            success = self._create_shortcut(shortcut_path)
        
        if success:
            console.print("[green]✓[/green] Start Menu shortcut created!")
        else:
            console.print("[red]✗[/red] Failed to create Start Menu shortcut.")
        
        return success
    
    def remove_desktop_shortcut(self) -> bool:
        """Remove desktop shortcut"""
        desktop_path = Path(os.path.expanduser("~/Desktop"))
        shortcut_path = desktop_path / f"{self.app_name}.lnk"
        
        with console.status("[bold yellow]Removing desktop shortcut..."):
            success = self._remove_shortcut(shortcut_path)
        
        if success:
            console.print("[green]✓[/green] Desktop shortcut removed!")
        else:
            console.print("[red]✗[/red] Failed to remove desktop shortcut.")
        
        return success
    
    def remove_start_menu_shortcut(self) -> bool:
        """Remove Start Menu shortcut"""
        start_menu_path = Path(os.getenv('APPDATA')) / "Microsoft/Windows/Start Menu/Programs"
        shortcut_path = start_menu_path / f"{self.app_name}.lnk"
        
        with console.status("[bold yellow]Removing Start Menu shortcut..."):
            success = self._remove_shortcut(shortcut_path)
        
        if success:
            console.print("[green]✓[/green] Start Menu shortcut removed!")
        else:
            console.print("[red]✗[/red] Failed to remove Start Menu shortcut.")
        
        return success
    
    def check_shortcuts(self) -> dict:
        """Check which shortcuts exist"""
        desktop_path = Path(os.path.expanduser("~/Desktop")) / f"{self.app_name}.lnk"
        start_menu_path = Path(os.getenv('APPDATA')) / f"Microsoft/Windows/Start Menu/Programs/{self.app_name}.lnk"
        
        return {
            "desktop": desktop_path.exists(),
            "start_menu": start_menu_path.exists()
        }
    
    def show_shortcut_info(self):
        """Show information about shortcuts"""
        info = self.check_shortcuts()
        
        table = Table(title="Shortcut Status", box=box.ROUNDED)
        table.add_column("Location", style="cyan")
        table.add_column("Status", style="green")
        
        table.add_row(
            "Desktop",
            "[green]✓ Exists[/green]" if info['desktop'] else "[red]✗ Not found[/red]"
        )
        table.add_row(
            "Start Menu",
            "[green]✓ Exists[/green]" if info['start_menu'] else "[red]✗ Not found[/red]"
        )
        
        console.print(table)
        
        console.print("\n[bold]Configuration:[/bold]")
        console.print(f"Python: {self.python_path}")
        console.print(f"Script: {self.script_path}")
        console.print(f"Working Directory: {self.working_dir}")
        console.print(f"Icon: {self.icon_path} ({'exists' if self.icon_path.exists() else 'not found'})")

    def add_context_menu_entry(self) -> bool:
        """Add EasyKit to the Windows right-click context menu for folders"""
        if not self.is_admin():
            console.print("[yellow]Administrator privileges required. Relaunching as admin...[/yellow]")
            if self.run_as_admin():
                sys.exit(0)
            else:
                console.print("[red]Failed to relaunch as administrator.[/red]")
                return False
        try:
            key_path = r"Directory\\shell\\EasyKit"
            command_path = key_path + r"\\command"
            icon_path = str(self.icon_path)
            script_path = str(self.script_path)
            python_path = self.python_path

            # Create the main key
            with winreg.CreateKey(winreg.HKEY_CLASSES_ROOT, key_path) as key:
                winreg.SetValueEx(key, None, 0, winreg.REG_SZ, "Open with EasyKit")
                if self.icon_path.exists():
                    winreg.SetValueEx(key, "Icon", 0, winreg.REG_SZ, icon_path)

            # Create the command subkey
            with winreg.CreateKey(winreg.HKEY_CLASSES_ROOT, command_path) as cmd_key:
                command = f'"{python_path}" "{script_path}" "%1"'
                winreg.SetValueEx(cmd_key, None, 0, winreg.REG_SZ, command)

            console.print("[green]✓[/green] Context menu entry added!")
            return True
        except Exception as e:
            logger.error(f"Error adding context menu entry: {e}")
            console.print(f"[red]✗[/red] Failed to add context menu entry: {e}")
            return False

    def remove_context_menu_entry(self) -> bool:
        """Remove EasyKit from the Windows right-click context menu for folders"""
        if not self.is_admin():
            console.print("[yellow]Administrator privileges required. Relaunching as admin...[/yellow]")
            if self.run_as_admin():
                sys.exit(0)
            else:
                console.print("[red]Failed to relaunch as administrator.[/red]")
                return False
        try:
            key_path = r"Directory\\shell\\EasyKit"
            winreg.DeleteKey(winreg.HKEY_CLASSES_ROOT, key_path + r"\\command")
            winreg.DeleteKey(winreg.HKEY_CLASSES_ROOT, key_path)
            console.print("[green]✓[/green] Context menu entry removed!")
            return True
        except FileNotFoundError:
            console.print("[yellow]Context menu entry not found.[/yellow]")
            return False
        except Exception as e:
            logger.error(f"Error removing context menu entry: {e}")
            console.print(f"[red]✗[/red] Failed to remove context menu entry: {e}")
            return False
