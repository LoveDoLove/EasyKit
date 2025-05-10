"""
Core functionality for software checking and installation
"""
import os
import sys
import subprocess
import platform
from pathlib import Path
import logging
from typing import Optional
import click
from rich.console import Console
from rich.logging import RichHandler
from rich.progress import Progress
from .config import Config
from easykit.utils import get_logger

console = Console()
logger = get_logger(__name__)

class SoftwareChecker:
    def __init__(self, config: Config):
        self.config = config
        self.system = platform.system().lower()

    def check_software(self, software_name: str) -> bool:
        """Check if specific software is installed"""
        method_name = f'_check_{software_name.lower()}'
        if hasattr(self, method_name):
            return getattr(self, method_name)()
        else:
            logger.error(f"No check method found for {software_name}")
            return False

    def _check_choco(self) -> bool:
        """Check if Chocolatey is installed"""
        if self.system != 'windows':
            logger.info("Chocolatey is only available on Windows")
            return False
        
        try:
            result = subprocess.run(['where', 'choco.exe'], 
                                 capture_output=True, 
                                 text=True)
            return result.returncode == 0
        except Exception as e:
            logger.error(f"Error checking Chocolatey: {e}")
            return False

    def _check_git(self) -> bool:
        """Check if Git is installed"""
        try:
            result = subprocess.run(['git', '--version'], 
                                 capture_output=True, 
                                 text=True)
            return result.returncode == 0
        except Exception as e:
            logger.error(f"Error checking Git: {e}")
            return False

    def _check_node(self) -> bool:
        """Check if Node.js is installed"""
        try:
            result = subprocess.run(['node', '--version'], 
                                 capture_output=True, 
                                 text=True)
            return result.returncode == 0
        except Exception as e:
            logger.error(f"Error checking Node.js: {e}")
            return False

    def _check_composer(self) -> bool:
        """Check if Composer is installed"""
        try:
            result = subprocess.run(['composer', '--version'], 
                                 capture_output=True, 
                                 text=True)
            return result.returncode == 0
        except Exception as e:
            logger.error(f"Error checking Composer: {e}")
            return False

@click.command()
@click.argument('software', required=True)
@click.option('--install', is_flag=True, help="Automatically install if not found")
def check_software_cli(software: str, install: bool):
    """Check if specific software is installed"""
    config = Config()
    checker = SoftwareChecker(config)
    
    with console.status(f"Checking for {software}..."):
        is_installed = checker.check_software(software)
    
    if is_installed:
        console.print(f"[green]✓[/green] {software} is installed")
    else:
        console.print(f"[red]✗[/red] {software} is not installed")
        if install:
            # TODO: Implement installation logic
            pass

if __name__ == '__main__':
    check_software_cli()
