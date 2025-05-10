"""
Git tools and utilities
"""
import subprocess
import os
from pathlib import Path
from typing import Optional, List, Dict
from datetime import datetime
from rich.console import Console
from rich.table import Table
from rich.prompt import Prompt, Confirm
from rich.panel import Panel
from rich.syntax import Syntax
from rich import box
import re
from ..utils import draw_header, get_logger, confirm_action
from ..core.software import SoftwareChecker
from ..core.config import Config

console = Console()
logger = get_logger(__name__)
config = Config()

class GitTools:
    def __init__(self):
        self.software_checker = SoftwareChecker(config)
        
    def ensure_git_installed(self) -> bool:
        """Check if Git is installed and available"""
        if not self.software_checker.check_software('git'):
            console.print("[red]Git is not installed.[/red]")
            console.print("Please install Git from https://git-scm.com/")
            return False
        return True
    
    def run_git_command(self, command: List[str], show_output: bool = True) -> tuple[bool, str]:
        """Run a git command and return success status and output"""
        if not self.ensure_git_installed():
            return False, ""
            
        try:
            process = subprocess.run(
                ['git'] + command,
                capture_output=True,
                text=True,
                check=False
            )
            
            output = process.stdout + process.stderr
            if show_output and output:
                if process.returncode == 0:
                    console.print(output)
                else:
                    console.print("[red]" + output + "[/red]")
                    
            return process.returncode == 0, output
        except Exception as e:
            logger.error(f"Error running git command: {e}")
            return False, str(e)
    
    def init_repo(self):
        """Initialize a new Git repository"""
        with console.status("[bold green]Initializing repository..."):
            success, _ = self.run_git_command(['init'])
            
        if success:
            console.print("[green]✓[/green] Repository initialized successfully!")
        else:
            console.print("[red]✗[/red] Failed to initialize repository.")
    
    def check_status(self):
        """Check repository status"""
        success, output = self.run_git_command(['status', '--porcelain', '-b'])
        
        if not success:
            return
        
        # Parse status output
        lines = output.split('\n')
        branch_line = lines[0] if lines else ''
        changes = [line for line in lines[1:] if line.strip()]
        
        # Show branch info
        match = re.match(r'## (.+?)(?:\.\.\.(.+))?$', branch_line)
        if match:
            branch = match.group(1)
            remote = match.group(2) if match.group(2) else None
            
            console.print(f"\n[bold cyan]Current branch:[/bold cyan] {branch}")
            if remote:
                console.print(f"[bold cyan]Tracking:[/bold cyan] {remote}")
        
        # Show changes
        if changes:
            table = Table(title="Changes", box=box.ROUNDED)
            table.add_column("Status", style="yellow")
            table.add_column("File", style="green")
            
            for change in changes:
                status = change[:2].strip()
                file = change[3:].strip()
                status_text = {
                    'M': 'Modified',
                    'A': 'Added',
                    'D': 'Deleted',
                    'R': 'Renamed',
                    'C': 'Copied',
                    'U': 'Updated',
                    '??': 'Untracked'
                }.get(status.replace(' ', ''), status)
                
                table.add_row(status_text, file)
            
            console.print(table)
        else:
            console.print("\n[green]Working directory clean[/green]")
    
    def add_all(self):
        """Add all changes to staging"""
        with console.status("[bold green]Adding all changes..."):
            success, _ = self.run_git_command(['add', '.'])
        
        if success:
            console.print("[green]✓[/green] All changes staged!")
        else:
            console.print("[red]✗[/red] Failed to stage changes.")
    
    def commit(self):
        """Commit staged changes"""
        message = Prompt.ask("Enter commit message")
        
        with console.status("[bold green]Committing changes..."):
            success, _ = self.run_git_command(['commit', '-m', message])
        
        if success:
            console.print("[green]✓[/green] Changes committed successfully!")
        else:
            console.print("[red]✗[/red] Failed to commit changes.")
    
    def push(self):
        """Push changes to remote"""
        # Get current branch
        success, output = self.run_git_command(['branch', '--show-current'], show_output=False)
        if not success:
            return
        
        branch = output.strip()
        with console.status(f"[bold green]Pushing to {branch}..."):
            success, _ = self.run_git_command(['push', 'origin', branch])
        
        if success:
            console.print(f"[green]✓[/green] Successfully pushed to {branch}!")
        else:
            console.print("[red]✗[/red] Failed to push changes.")
    
    def pull(self):
        """Pull changes from remote"""
        with console.status("[bold green]Pulling changes..."):
            success, _ = self.run_git_command(['pull'])
        
        if success:
            console.print("[green]✓[/green] Successfully pulled changes!")
        else:
            console.print("[red]✗[/red] Failed to pull changes.")
    
    def create_branch(self):
        """Create a new branch"""
        branch_name = Prompt.ask("Enter new branch name")
        checkout = Confirm.ask("Switch to new branch?", default=True)
        
        command = ['branch', branch_name]
        if checkout:
            command = ['checkout', '-b', branch_name]
        
        with console.status(f"[bold green]Creating branch {branch_name}..."):
            success, _ = self.run_git_command(command)
        
        if success:
            console.print(f"[green]✓[/green] Branch {branch_name} created!")
            if not checkout:
                console.print(f"Use 'Switch Branch' to switch to {branch_name}")
        else:
            console.print("[red]✗[/red] Failed to create branch.")
    
    def switch_branch(self):
        """Switch to a different branch"""
        # Get list of branches
        success, output = self.run_git_command(['branch'], show_output=False)
        if not success:
            return
        
        branches = []
        current = None
        for line in output.split('\n'):
            if line.strip():
                is_current = line.startswith('*')
                branch = line.strip('* ').strip()
                branches.append(branch)
                if is_current:
                    current = branch
        
        if not branches:
            console.print("[yellow]No branches found[/yellow]")
            return
        
        # Show branch selection table
        table = Table(title="Available Branches", box=box.ROUNDED)
        table.add_column("Branch", style="green")
        table.add_column("Status", style="cyan")
        
        for branch in branches:
            status = "current" if branch == current else ""
            table.add_row(branch, status)
        
        console.print(table)
        branch = Prompt.ask("Enter branch name to switch to", choices=branches)
        
        with console.status(f"[bold green]Switching to {branch}..."):
            success, _ = self.run_git_command(['checkout', branch])
        
        if success:
            console.print(f"[green]✓[/green] Switched to {branch}!")
        else:
            console.print("[red]✗[/red] Failed to switch branch.")
    
    def merge_branch(self):
        """Merge a branch into current branch"""
        # Get current branch
        success, current = self.run_git_command(['branch', '--show-current'], show_output=False)
        if not success:
            return
        current = current.strip()
        
        # Get list of other branches
        success, output = self.run_git_command(['branch'], show_output=False)
        if not success:
            return
        
        branches = [b.strip('* ').strip() for b in output.split('\n') if b.strip()]
        branches = [b for b in branches if b != current]
        
        if not branches:
            console.print("[yellow]No other branches to merge[/yellow]")
            return
        
        console.print(f"\nCurrent branch: [cyan]{current}[/cyan]")
        branch = Prompt.ask("Enter branch name to merge", choices=branches)
        
        if confirm_action(f"Merge {branch} into {current}?"):
            with console.status(f"[bold green]Merging {branch}..."):
                success, _ = self.run_git_command(['merge', branch])
            
            if success:
                console.print(f"[green]✓[/green] Merged {branch} into {current}!")
            else:
                console.print("[red]✗[/red] Merge failed. Please resolve conflicts.")
    
    def view_history(self):
        """View commit history"""
        format_str = '--pretty=format:%C(yellow)%h%Creset %C(cyan)%ad%Creset %s %C(green)<%an>%Creset'
        success, output = self.run_git_command(['log', '--date=short', format_str, '-n', '20'])
        
        if success and output:
            console.print("\n[bold]Recent Commits[/bold]")
            console.print(Panel(output, title="Git Log"))
        else:
            console.print("[yellow]No commit history found[/yellow]")
    
    def stash_changes(self):
        """Stash current changes"""
        message = Prompt.ask("Enter stash message (optional)", default="")
        command = ['stash', 'push']
        if message:
            command.extend(['-m', message])
        
        with console.status("[bold green]Stashing changes..."):
            success, _ = self.run_git_command(command)
        
        if success:
            console.print("[green]✓[/green] Changes stashed successfully!")
        else:
            console.print("[red]✗[/red] Failed to stash changes.")
    
    def apply_stash(self):
        """Apply stashed changes"""
        # Get stash list
        success, output = self.run_git_command(['stash', 'list'], show_output=False)
        if not success or not output:
            console.print("[yellow]No stashes found[/yellow]")
            return
        
        stashes = []
        for line in output.split('\n'):
            if line.strip():
                match = re.match(r'stash@{(\d+)}: (.*)', line)
                if match:
                    stashes.append((match.group(1), match.group(2)))
        
        if not stashes:
            console.print("[yellow]No stashes found[/yellow]")
            return
        
        # Show stash selection table
        table = Table(title="Available Stashes", box=box.ROUNDED)
        table.add_column("Index", style="cyan")
        table.add_column("Description", style="green")
        
        for idx, desc in stashes:
            table.add_row(idx, desc)
        
        console.print(table)
        stash_idx = Prompt.ask("Enter stash index to apply")
        
        if confirm_action("Drop stash after applying?", default=False):
            command = ['stash', 'pop', f'stash@{{{stash_idx}}}']
        else:
            command = ['stash', 'apply', f'stash@{{{stash_idx}}}']
        
        with console.status("[bold green]Applying stash..."):
            success, _ = self.run_git_command(command)
        
        if success:
            console.print("[green]✓[/green] Stash applied successfully!")
        else:
            console.print("[red]✗[/red] Failed to apply stash.")
    
    def create_pull_request(self):
        """Create a new pull request"""
        # This is a placeholder for GitHub/GitLab integration
        console.print("[yellow]Pull request creation requires GitHub/GitLab integration.[/yellow]")
        console.print("Please use the web interface for now.")
    
    def list_pull_requests(self):
        """List pull requests"""
        # This is a placeholder for GitHub/GitLab integration
        console.print("[yellow]Pull request listing requires GitHub/GitLab integration.[/yellow]")
        console.print("Please use the web interface for now.")
