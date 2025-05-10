"""
Utility functions for EasyKit
"""
import logging
from rich.logging import RichHandler
from rich.console import Console

def get_logger(name: str) -> logging.Logger:
    """Configure and return a logger instance"""
    logging.basicConfig(
        level="INFO",
        format="%(message)s",
        datefmt="[%X]",
        handlers=[RichHandler(rich_tracebacks=True)]
    )
    
    logger = logging.getLogger(name)
    return logger

def confirm_action(message: str, default: bool = True) -> bool:
    """Ask user for confirmation"""
    console = Console()
    suffix = " [Y/n]" if default else " [y/N]"
    response = console.input(f"{message}{suffix}").lower()
    
    if response in ['y', 'yes']:
        return True
    elif response in ['n', 'no']:
        return False
    else:
        return default

def draw_header(title: str, width: int = 50) -> None:
    """Draw a header with the given title"""
    console = Console()
    line = "=" * width
    console.print(line)
    console.print(f" {title} ".center(width))
    console.print(line)
