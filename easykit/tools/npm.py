"""
NPM tools and utilities
"""
import subprocess
from pathlib import Path
from typing import Optional, List
from rich.console import Console
from rich.table import Table
from rich.prompt import Prompt, Confirm
from rich import box
import json
from ..utils import draw_header, get_logger, confirm_action
from ..core.software import SoftwareChecker
from ..core.config import Config

console = Console()
logger = get_logger(__name__)
config = Config()

class NpmTools:
    def __init__(self):
        self.software_checker = SoftwareChecker(config)
    
    def ensure_npm_installed(self) -> bool:
        """Check if npm is installed and available"""
        if not self.software_checker.check_software('node'):
            console.print("[red]Node.js/NPM is not installed.[/red]")
            console.print("Please install Node.js from https://nodejs.org/")
            return False
        return True
    
    def run_npm_command(self, command: List[str], show_output: bool = True) -> bool:
        """Run an npm command and handle its output"""
        if not self.ensure_npm_installed():
            return False
            
        try:
            process = subprocess.run(
                ['npm'] + command,
                capture_output=True,
                text=True,
                check=False
            )
            
            if show_output:
                if process.stdout:
                    console.print(process.stdout)
                if process.stderr:
                    console.print("[red]" + process.stderr + "[/red]")
                    
            return process.returncode == 0
        except Exception as e:
            logger.error(f"Error running npm command: {e}")
            return False
    
    def install_packages(self):
        """Install npm packages"""
        with console.status("[bold green]Installing npm packages..."):
            success = self.run_npm_command(['install', '--no-fund', '--loglevel=error'])
        
        if success:
            console.print("[green]✓[/green] Packages installed successfully!")
        else:
            console.print("[red]✗[/red] Failed to install packages.")
    
    def update_packages(self):
        """Update npm packages using npm-check-updates"""
        # First, ensure ncu is installed
        try:
            subprocess.run(['ncu', '--version'], capture_output=True, check=True)
        except (subprocess.CalledProcessError, FileNotFoundError):
            console.print("[yellow]Installing npm-check-updates...[/yellow]")
            if not self.run_npm_command(['install', '-g', 'npm-check-updates']):
                console.print("[red]Failed to install npm-check-updates[/red]")
                return
        
        with console.status("[bold green]Checking for updates..."):
            if self.run_npm_command(['exec', 'ncu', '-u']):
                console.print("[green]✓[/green] Package file updated!")
                self.install_packages()
            else:
                console.print("[red]✗[/red] Failed to update packages.")
    
    def build_production(self):
        """Run production build"""
        with console.status("[bold green]Building for production..."):
            if self.run_npm_command(['run', 'build']):
                console.print("[green]✓[/green] Production build completed!")
            else:
                console.print("[red]✗[/red] Build failed.")
    
    def build_development(self):
        """Run development build"""
        with console.status("[bold green]Building for development..."):
            if self.run_npm_command(['run', 'dev']):
                console.print("[green]✓[/green] Development build completed!")
            else:
                console.print("[red]✗[/red] Build failed.")
    
    def security_audit(self):
        """Run npm security audit"""
        with console.status("[bold green]Running security audit..."):
            self.run_npm_command(['audit'])
    
    def run_custom_script(self):
        """Run a custom npm script"""
        try:
            with open('package.json', 'r') as f:
                package_data = json.load(f)
            
            if 'scripts' not in package_data:
                console.print("[yellow]No scripts found in package.json[/yellow]")
                return
            
            scripts = list(package_data['scripts'].keys())
            if not scripts:
                console.print("[yellow]No scripts found in package.json[/yellow]")
                return
            
            table = Table(title="Available Scripts", box=box.ROUNDED)
            table.add_column("Script", style="cyan")
            table.add_column("Command", style="green")
            
            for script in scripts:
                table.add_row(script, package_data['scripts'][script])
            
            console.print(table)
            script = Prompt.ask("\nEnter script name to run", choices=scripts)
            
            with console.status(f"[bold green]Running {script}..."):
                if self.run_npm_command(['run', script]):
                    console.print(f"[green]✓[/green] Script {script} completed!")
                else:
                    console.print(f"[red]✗[/red] Script {script} failed.")
                    
        except FileNotFoundError:
            console.print("[yellow]No package.json found in current directory[/yellow]")
        except json.JSONDecodeError:
            console.print("[red]Invalid package.json file[/red]")
    
    def show_package_info(self):
        """Show package information"""
        try:
            with open('package.json', 'r') as f:
                package_data = json.load(f)
            
            console.print(Panel.fit(
                json.dumps(package_data, indent=2),
                title="package.json",
                border_style="green"
            ))
        except FileNotFoundError:
            console.print("[yellow]No package.json found in current directory[/yellow]")
        except json.JSONDecodeError:
            console.print("[red]Invalid package.json file[/red]")
    
    def reset_cache(self):
        """Reset npm cache"""
        if confirm_action("Are you sure you want to reset the npm cache?", default=False):
            with console.status("[bold yellow]Resetting npm cache..."):
                if self.run_npm_command(['cache', 'clean', '--force']):
                    console.print("[green]✓[/green] Cache reset successfully!")
                else:
                    console.print("[red]✗[/red] Failed to reset cache.")
