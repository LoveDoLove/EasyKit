"""
Laravel tools and utilities
"""
import subprocess
import os
from pathlib import Path
from typing import Optional, List, Dict
import shutil
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

class LaravelTools:
    def __init__(self):
        self.software_checker = SoftwareChecker(config)
        
    def ensure_php_installed(self) -> bool:
        """Check if PHP is installed and available"""
        try:
            process = subprocess.run(['php', '--version'], 
                                  capture_output=True, 
                                  text=True)
            return process.returncode == 0
        except Exception:
            console.print("[red]PHP is not installed or not in PATH[/red]")
            console.print("Please install PHP from https://www.php.net/")
            return False

    def _find_composer_command(self) -> list:
        """Find the correct Composer command for the current OS."""
        candidates = [
            ['composer'],
            ['composer.bat'],
            ['composer.exe']
        ]
        # Check for composer.phar in current directory
        if Path('composer.phar').exists():
            candidates.append(['php', 'composer.phar'])
        for cmd in candidates:
            try:
                result = subprocess.run(cmd + ['--version'], capture_output=True, text=True)
                if result.returncode == 0:
                    return cmd
            except Exception:
                continue
        return None

    def run_composer_command(self, command: List[str], show_output: bool = True) -> bool:
        """Run a composer command (robust for Windows)."""
        if not self.ensure_php_installed():
            return False
        composer_cmd = self._find_composer_command()
        if not composer_cmd:
            console.print("[red]Composer executable not found. Please ensure Composer is installed and in your PATH.[/red]")
            return False
        try:
            process = subprocess.run(
                composer_cmd + command,
                capture_output=True,
                text=True,
                check=False
            )
            if show_output:
                if process.stdout:
                    if process.returncode == 0:
                        console.print(f"[green]{process.stdout}[/green]")
                    else:
                        console.print(f"[red]{process.stdout}[/red]")
                if process.stderr:
                    if process.returncode == 0:
                        console.print(f"[green]{process.stderr}[/green]")
                    else:
                        console.print(f"[red]{process.stderr}[/red]")
            return process.returncode == 0
        except Exception as e:
            logger.error(f"Error running composer command: {e}")
            return False
            
    def run_artisan_command(self, command: List[str], show_output: bool = True) -> bool:
        """Run an artisan command"""
        if not self.ensure_php_installed():
            return False
        if not Path('artisan').exists():
            console.print("[red]This doesn't appear to be a Laravel project directory.[/red]")
            console.print("Make sure you're in the root directory of a Laravel project.")
            return False
        try:
            process = subprocess.run(
                ['php', 'artisan'] + command,
                capture_output=True,
                text=True,
                check=False
            )
            if show_output:
                if process.stdout:
                    if process.returncode == 0:
                        console.print(f"[green]{process.stdout}[/green]")
                    else:
                        console.print(f"[red]{process.stdout}[/red]")
                if process.stderr:
                    if process.returncode == 0:
                        console.print(f"[green]{process.stderr}[/green]")
                    else:
                        console.print(f"[red]{process.stderr}[/red]")
            return process.returncode == 0
        except Exception as e:
            logger.error(f"Error running artisan command: {e}")
            return False
    
    def quick_setup(self):
        """Perform quick Laravel setup"""
        if not Path('artisan').exists():
            console.print("[red]This doesn't appear to be a Laravel project directory.[/red]")
            return
            
        with console.status("[bold green]Setting up Laravel project..."):
            # Create .env file if needed
            if not Path('.env').exists():
                if Path('.env.example').exists():
                    shutil.copy('.env.example', '.env')
                    console.print("[green]✓[/green] Created .env file")
                else:
                    console.print("[red]✗[/red] .env.example file not found")
                    return
            
            # Install dependencies
            if self.run_composer_command(['install'], show_output=False):
                console.print("[green]✓[/green] Installed Composer dependencies")
            else:
                console.print("[red]✗[/red] Failed to install dependencies")
                return
            
            # Generate key
            if self.run_artisan_command(['key:generate'], show_output=False):
                console.print("[green]✓[/green] Generated application key")
            else:
                console.print("[red]✗[/red] Failed to generate key")
            
            # Clear caches
            self.run_artisan_command(['config:clear'], show_output=False)
            self.run_artisan_command(['cache:clear'], show_output=False)
            console.print("[green]✓[/green] Cleared configuration and cache")
            
            console.print("\n[green]Setup completed successfully![/green]")
    
    def install_packages(self):
        """Install Laravel packages"""
        with console.status("[bold green]Installing packages..."):
            if self.run_composer_command(['install']):
                console.print("[green]✓[/green] Packages installed successfully!")
            else:
                console.print("[red]✗[/red] Failed to install packages.")
    
    def update_packages(self):
        """Update Laravel packages"""
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
    
    def build_production(self):
        """Build for production"""
        steps = [
            ('Installing production dependencies...', ['install', '--no-dev']),
            ('Optimizing configuration...', ['artisan', 'config:cache']),
            ('Optimizing routes...', ['artisan', 'route:cache']),
            ('Optimizing views...', ['artisan', 'view:cache'])
        ]
        
        with console.status("[bold green]Building for production...") as status:
            for step_msg, command in steps:
                status.update(f"[bold green]{step_msg}")
                if command[0] == 'artisan':
                    success = self.run_artisan_command(command[1:], show_output=False)
                else:
                    success = self.run_composer_command(command, show_output=False)
                
                if not success:
                    console.print(f"[red]✗[/red] {step_msg} failed")
                    return
                
            console.print("[green]✓[/green] Production build completed!")
    
    def run_dev_server(self):
        """Run development server"""
        try:
            console.print("[green]Starting development server...[/green]")
            console.print("Press Ctrl+C to stop the server")
            self.run_artisan_command(['serve'])
        except KeyboardInterrupt:
            console.print("\n[yellow]Development server stopped[/yellow]")
    
    def create_storage_link(self):
        """Create storage symbolic link"""
        with console.status("[bold green]Creating storage link..."):
            if self.run_artisan_command(['storage:link']):
                console.print("[green]✓[/green] Storage link created!")
            else:
                console.print("[red]✗[/red] Failed to create storage link.")
    
    def run_database_seeding(self):
        """Run database seeding"""
        if confirm_action("This will refresh your database. Are you sure?", default=False):
            with console.status("[bold green]Running database seeding..."):
                if self.run_artisan_command(['migrate:fresh', '--seed']):
                    console.print("[green]✓[/green] Database seeded successfully!")
                else:
                    console.print("[red]✗[/red] Failed to seed database.")
    
    def test_database(self):
        """Test database connection"""
        with console.status("[bold green]Testing database connection..."):
            if self.run_artisan_command(['db:show']):
                console.print("[green]✓[/green] Database connection successful!")
            else:
                console.print("[red]✗[/red] Database connection failed.")
    
    def check_php_version(self):
        """Check PHP version"""
        try:
            process = subprocess.run(['php', '--version'], 
                                  capture_output=True, 
                                  text=True)
            if process.returncode == 0:
                console.print(f"[green]{process.stdout}[/green]")
            else:
                console.print("[red]Failed to get PHP version[/red]")
        except Exception as e:
            console.print("[red]PHP is not installed or not in PATH[/red]")
    
    def check_configuration(self):
        """Check Laravel configuration"""
        checks = [
            ('PHP Version', ['php', '--version']),
            ('Laravel Version', ['artisan', '--version']),
            ('Environment', ['artisan', 'env']),
            ('Cache Status', ['artisan', 'cache:status']),
            ('Route List', ['artisan', 'route:list', '--compact'])
        ]
        
        table = Table(title="Laravel Configuration", box=box.ROUNDED)
        table.add_column("Check", style="cyan")
        table.add_column("Status", style="green")
        
        for check_name, command in checks:
            try:
                if command[0] == 'php':
                    process = subprocess.run(command, capture_output=True, text=True)
                else:
                    process = subprocess.run(['php'] + command, capture_output=True, text=True)
                
                if process.returncode == 0:
                    result = process.stdout.split('\n')[0]  # First line only
                    table.add_row(check_name, result)
                else:
                    table.add_row(check_name, "[red]Check failed[/red]")
            except Exception as e:
                table.add_row(check_name, f"[red]Error: {str(e)}[/red]")
        
        console.print(table)
    
    def reset_cache(self):
        """Reset all Laravel cache"""
        if confirm_action("Are you sure you want to reset all cache?", default=False):
            commands = [
                'config:clear',
                'cache:clear',
                'view:clear',
                'route:clear',
                'event:clear',
                'optimize:clear'
            ]
            
            with console.status("[bold yellow]Resetting all cache...") as status:
                for command in commands:
                    status.update(f"[bold yellow]Running {command}...")
                    self.run_artisan_command(command.split(':'), show_output=False)
                
                console.print("[green]✓[/green] All cache cleared successfully!")
    
    def sail_menu(self):
        """Show Laravel Sail menu"""
        if not Path('vendor/bin/sail').exists():
            console.print("[yellow]Laravel Sail is not installed in this project.[/yellow]")
            if confirm_action("Would you like to install Laravel Sail?"):
                self.run_composer_command(['require', 'laravel/sail', '--dev'])
                self.run_artisan_command(['sail:install'])
            return
        
        while True:
            console.clear()
            draw_header("Laravel Sail Commands")
            
            table = Table(show_header=False, box=box.ROUNDED)
            table.add_column("Option", style="cyan")
            table.add_column("Description")
            
            table.add_row("0", "Back to Laravel Menu")
            table.add_row("1", "Start Sail")
            table.add_row("2", "Stop Sail")
            table.add_row("3", "Restart Sail")
            table.add_row("4", "Show Container Status")
            
            console.print(table)
            
            choice = Prompt.ask("\nChoose an option", choices=["0", "1", "2", "3", "4"])
            
            if choice == "0":
                break
            elif choice == "1":
                self.run_artisan_command(['sail', 'up', '-d'])
            elif choice == "2":
                self.run_artisan_command(['sail', 'down'])
            elif choice == "3":
                self.run_artisan_command(['sail', 'restart'])
            elif choice == "4":
                self.run_artisan_command(['sail', 'ps'])
