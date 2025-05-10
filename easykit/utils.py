"""
Utility functions for EasyKit
"""
import logging
from rich.logging import RichHandler
from rich.console import Console
from rich.theme import Theme
from .core.config import Config

def get_console():
    config = Config()
    color_scheme = config.get('color_scheme', 'dark')
    if color_scheme == 'light':
        theme = Theme({
            "info": "black on white",
            "warning": "yellow",
            "error": "red",
            "success": "green",
        })
        return Console(theme=theme, style="black on white")
    elif color_scheme == 'dark':
        theme = Theme({
            "info": "white on black",
            "warning": "yellow",
            "error": "red",
            "success": "green",
        })
        return Console(theme=theme, style="white on black")
    else:
        return Console()

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
    console = get_console()
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
    console = get_console()
    line = "=" * width
    console.print(line)
    console.print(f" {title} ".center(width))
    console.print(line)
