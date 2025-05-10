"""
Composer tools and utilities
"""
import subprocess
import os
from pathlib import Path
from typing import Optional, List
from rich.console import Console
from rich.table import Table
from rich.prompt import Prompt, Confirm
from rich.panel import Panel
from rich import box
import json
from ..utils import draw_header, get_logger, confirm_action
from ..core.software import SoftwareChecker
from ..core.config import Config

console = Console()
logger = get_logger(__name__)
config = Config()

class ComposerTools:
    def __init__(self):
        self.software_checker = SoftwareChecker(config)
    
    def ensure_composer_installed(self) -> bool:
        """Check if Composer is installed and available"""
        if not self.software_checker.check_software('composer'):
            console.print("[red]Composer is not installed.[/red]")
            console.print("Please install Composer from https://getcomposer.org/")
            return False
        return True
    
    def run_composer_command(self, command: List[str], show_output: bool = True) -> bool:
        """Run a composer command"""
        if not self.ensure_composer_installed():
            return False
            
        try:
            process = subprocess.run(
                ['composer'] + command,
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
            logger.error(f"Error running composer command: {e}")
            return False
    
    def install_packages(self):
        """Install Composer packages"""
        with console.status("[bold green]Installing packages..."):
            if self.run_composer_command(['install']):
                console.print("[green]✓[/green] Packages installed successfully!")
            else:
                console.print("[red]✗[/red] Failed to install packages.")
    
    def update_packages(self):
        """Update Composer packages"""
        with console.status("[bold green]Updating packages..."):
            if self.run_composer_command(['update']):
                console.print("[green]✓[/green] Packages updated successfully!")
            else:
                console.print("[red]✗[/red] Failed to update packages.")
    
    def regenerate_autoload(self):
        """Regenerate composer autoload files"""
        with console.status("[bold green]Regenerating autoload files..."):
            if self.run_composer_command(['dump-autoload']):
                console.print("[green]✓[/green] Autoload files regenerated!")
            else:
                console.print("[red]✗[/red] Failed to regenerate autoload files.")
    
    def require_package(self):
        """Require a new package"""
        package = Prompt.ask("\nEnter package name (e.g. 'vendor/package')")
        dev = Confirm.ask("Is this a development dependency?", default=False)
        
        command = ['require']
        if dev:
            command.append('--dev')
        command.append(package)
        
        with console.status(f"[bold green]Installing {package}..."):
            if self.run_composer_command(command):
                console.print(f"[green]✓[/green] Package {package} installed successfully!")
            else:
                console.print(f"[red]✗[/red] Failed to install {package}.")
    
    def create_project(self):
        """Create a new Composer project"""
        package = Prompt.ask("\nEnter project package (e.g. 'laravel/laravel')")
        directory = Prompt.ask("Enter project directory name", default=Path(package).name)
        
        with console.status(f"[bold green]Creating new project from {package}..."):
            if self.run_composer_command(['create-project', package, directory]):
                console.print(f"[green]✓[/green] Project created successfully in {directory}!")
            else:
                console.print("[red]✗[/red] Failed to create project.")
    
    def validate_json(self):
        """Validate composer.json file"""
        if not Path('composer.json').exists():
            console.print("[yellow]No composer.json found in current directory[/yellow]")
            return
            
        with console.status("[bold green]Validating composer.json..."):
            if self.run_composer_command(['validate']):
                console.print("[green]✓[/green] composer.json is valid!")
            else:
                console.print("[red]✗[/red] composer.json validation failed.")
    
    def clear_cache(self):
        """Clear Composer cache"""
        if confirm_action("Are you sure you want to clear Composer cache?", default=False):
            with console.status("[bold yellow]Clearing Composer cache..."):
                if self.run_composer_command(['clear-cache']):
                    console.print("[green]✓[/green] Cache cleared successfully!")
                else:
                    console.print("[red]✗[/red] Failed to clear cache.")
    
    def show_package_info(self):
        """Show package information from composer.json"""
        try:
            with open('composer.json', 'r') as f:
                package_data = json.load(f)
            
            console.print(Panel.fit(
                json.dumps(package_data, indent=2),
                title="composer.json",
                border_style="green"
            ))
        except FileNotFoundError:
            console.print("[yellow]No composer.json found in current directory[/yellow]")
        except json.JSONDecodeError:
            console.print("[red]Invalid composer.json file[/red]")
