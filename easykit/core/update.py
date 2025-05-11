"""
Update management and version control
"""
import os
import sys
import shutil
import tempfile
from datetime import datetime
from pathlib import Path
import requests
import json
from packaging import version
from rich.console import Console
from rich.table import Table
from rich.prompt import Prompt, Confirm
from rich.markdown import Markdown
from rich.panel import Panel
from rich.progress import Progress, SpinnerColumn, TextColumn
from rich import box
from ..utils import draw_header, get_logger, confirm_action
from ..core.config import Config

console = Console()
logger = get_logger(__name__)
config = Config()

class UpdateManager:
    def __init__(self):
        self.current_version = "3.2.0"
        self.repo_owner = "LoveDoLove"
        self.repo_name = "EasyKit"
        self.base_url = f"https://api.github.com/repos/{self.repo_owner}/{self.repo_name}"
        self.backup_dir = Path(__file__).parent.parent.parent / "scripts/core/backups"
    
    def check_for_updates(self) -> dict:
        """Check GitHub for updates"""
        try:
            with console.status("[bold green]Checking for updates..."):
                response = requests.get(
                    f"{self.base_url}/releases/latest",
                    headers={'Accept': 'application/vnd.github.v3+json'}
                )
                response.raise_for_status()
                release_data = response.json()
                
                latest_version = release_data['tag_name'].lstrip('v')
                zip_asset = next(
                    (a for a in release_data['assets'] if a['name'].endswith('.zip')),
                    None
                )
                installer_asset = next(
                    (a for a in release_data['assets'] if a['name'].endswith('Setup.exe')),
                    None
                )
                
                return {
                    'version': latest_version,
                    'zip_url': zip_asset['browser_download_url'] if zip_asset else None,
                    'installer_url': installer_asset['browser_download_url'] if installer_asset else None,
                    'notes': release_data['body'],
                    'published_at': release_data['published_at']
                }
        except Exception as e:
            logger.error(f"Error checking for updates: {e}")
            raise
    
    def backup_scripts(self) -> bool:
        """Backup current scripts"""
        timestamp = datetime.now().strftime("%Y-%m-%d_%H-%M-%S")
        backup_path = self.backup_dir / f"backup_{timestamp}"
        
        try:
            # Create backup directory
            backup_path.mkdir(parents=True, exist_ok=True)
            
            # Define source directories to backup
            core_dir = Path(__file__).parent.parent.parent / "scripts/core"
            tools_dir = Path(__file__).parent.parent.parent / "scripts/tools"
            
            with console.status("[bold green]Creating backup...") as status:            # Backup Python files and executables
                for ext in ['.py', '.pyd', '.dll', '.exe']:
                    for script in core_dir.glob(f"*{ext}"):
                        if script.name != "backups":
                            status.update(f"[bold green]Backing up {script.name}...")
                            shutil.copy2(script, backup_path)
                
                # Backup tool modules
                for script in tools_dir.glob("*.py"):
                    status.update(f"[bold green]Backing up {script.name}...")
                    shutil.copy2(script, backup_path)
            
            console.print(f"[green]✓[/green] Backup created at: {backup_path}")
            return True
        except Exception as e:
            logger.error(f"Error creating backup: {e}")
            console.print(f"[red]Error creating backup: {e}[/red]")
            return False
    
    def list_backups(self) -> list:
        """List available backups"""
        try:
            backups = []
            for backup_dir in sorted(self.backup_dir.glob("backup_*"), reverse=True):
                if backup_dir.is_dir():
                    timestamp = backup_dir.name.replace("backup_", "")
                    script_count = len(list(backup_dir.glob("*.*")))
                    backups.append({
                        'path': backup_dir,
                        'timestamp': timestamp,
                        'script_count': script_count
                    })
            return backups
        except Exception as e:
            logger.error(f"Error listing backups: {e}")
            return []
    
    def show_backup_list(self):
        """Display list of available backups"""
        backups = self.list_backups()
        
        if not backups:
            console.print("[yellow]No backups found[/yellow]")
            return
        
        table = Table(title="Available Backups", box=box.ROUNDED)
        table.add_column("Date", style="cyan")
        table.add_column("Time", style="green")
        table.add_column("Scripts", justify="right")
        table.add_column("Path")
        
        for backup in backups:
            timestamp = backup['timestamp']
            date, time = timestamp.split('_')
            time = time.replace('-', ':')
            table.add_row(
                date,
                time,
                str(backup['script_count']),
                str(backup['path'])
            )
        
        console.print(table)
    
    def restore_backup(self, backup_path: Path) -> bool:
        """Restore scripts from a backup"""
        try:
            if not backup_path.exists():
                console.print("[red]Backup not found[/red]")
                return False
            
            core_dir = Path(__file__).parent.parent.parent / "scripts/core"
            tools_dir = Path(__file__).parent.parent.parent / "scripts/tools"
            
            with console.status("[bold yellow]Restoring backup...") as status:
                # Restore core scripts
                for script in backup_path.glob("*.bat"):
                    if "composer" in script.name or "git" in script.name or "laravel" in script.name or "npm" in script.name:
                        target = tools_dir / script.name
                    else:
                        target = core_dir / script.name
                    
                    status.update(f"[bold yellow]Restoring {script.name}...")
                    shutil.copy2(script, target)
            
            console.print("[green]✓[/green] Backup restored successfully!")
            return True
        except Exception as e:
            logger.error(f"Error restoring backup: {e}")
            console.print(f"[red]Error restoring backup: {e}[/red]")
            return False
    
    def show_release_notes(self):
        """Show release notes for current and available versions"""
        docs_dir = Path(__file__).parent.parent.parent / "docs"

        # Get all release note files (support both release_notes_*.md and *release-notes.md)
        release_notes = list(docs_dir.glob("release_notes_*.md"))
        release_notes += list(docs_dir.glob("*release-notes.md"))
        # Remove duplicates (if any)
        release_notes = list({str(p): p for p in release_notes}.values())
        # Sort by version if possible, else by name
        def extract_version(p):
            stem = p.stem
            # Try to extract version from both patterns
            if stem.startswith("release_notes_"):
                return stem.replace("release_notes_", "")
            elif "-release-notes" in stem:
                return stem.replace("-release-notes", "")
            return stem
        release_notes = sorted(
            release_notes,
            key=lambda p: extract_version(p),
            reverse=True
        )

        if not release_notes:
            console.print("[yellow]No release notes found[/yellow]")
            return

        for note_file in release_notes:
            try:
                with open(note_file, 'r', encoding='utf-8') as f:
                    content = f.read()
                ver = extract_version(note_file)
                console.print(f"\n[bold cyan]Version {ver}:[/bold cyan]")
                console.print(Markdown(content))
            except Exception as e:
                logger.error(f"Error reading release notes: {e}")
                console.print(f"[red]Error reading {note_file.name}[/red]")
    
    def download_update(self, url: str, target_path: Path) -> bool:
        """Download update file"""
        try:
            with console.status("[bold green]Downloading update...") as status:
                response = requests.get(url, stream=True)
                response.raise_for_status()
                
                total_size = int(response.headers.get('content-length', 0))
                block_size = 8192
                
                with open(target_path, 'wb') as f:
                    if total_size == 0:
                        f.write(response.content)
                    else:
                        with Progress(
                            SpinnerColumn(),
                            TextColumn("[bold green]Downloading..."),
                            transient=True
                        ) as progress:
                            task = progress.add_task("", total=total_size)
                            for data in response.iter_content(block_size):
                                f.write(data)
                                progress.update(task, advance=len(data))
            
            return True
        except Exception as e:
            logger.error(f"Error downloading update: {e}")
            if target_path.exists():
                target_path.unlink()
            return False
    
    def install_update(self, installer_path: Path) -> bool:
        """Install downloaded update"""
        try:
            console.print("\n[bold yellow]Starting installer...[/bold yellow]")
            console.print("Please follow the installation wizard.")
            
            # Run installer
            os.startfile(str(installer_path))
            return True
        except Exception as e:
            logger.error(f"Error starting installer: {e}")
            console.print(f"[red]Error starting installer: {e}[/red]")
            return False
