"""
Main entry point for EasyKit CLI
"""
import click
from rich.console import Console
from .core.software import SoftwareChecker
from .core.config import Config
from .utils import draw_header, get_logger

console = Console()
logger = get_logger(__name__)

@click.group()
@click.version_option()
def cli():
    """EasyKit - Cross-platform Development Environment Setup Tool"""
    pass

@cli.command()
@click.argument('software')
@click.option('--install', is_flag=True, help="Install if not found")
def check(software: str, install: bool):
    """Check if specific software is installed"""
    config = Config()
    checker = SoftwareChecker(config)
    draw_header(f"Checking {software}")
    
    is_installed = checker.check_software(software)
    if is_installed:
        console.print(f"[green]✓[/green] {software} is installed")
    else:
        console.print(f"[red]✗[/red] {software} is not installed")
        if install:
            # TODO: Implement installation logic
            pass

@cli.command()
def config():
    """Show current configuration"""
    config = Config()
    draw_header("Current Configuration")
    for key, value in config.config.items():
        console.print(f"{key}: {value}")

def main():
    cli()
