"""
Configuration management for EasyKit
"""
import os
from pathlib import Path
from typing import Any, Dict, Optional
import json
from dotenv import load_dotenv
import platformdirs

APP_VERSION = "3.2.1"

class Config:
    def __init__(self):
        self.app_name = "EasyKit"
        self.app_author = "LoveDoLove"
        self._load_config()

    def _load_config(self):
        """Load configuration from various sources"""
        # Default configuration
        self.config = {
            "color_scheme": "dark",
            "enable_logging": True,
            "log_path": str(self._get_log_path()),
            "check_updates": True,
            "show_tips": True,
            "confirm_exit": True,
            "menu_width": 50,
            "version": APP_VERSION
        }

        # Load from config file, or create it if missing
        config_file = self._get_config_file()
        if config_file.exists():
            try:
                with open(config_file, 'r') as f:
                    user_config = json.load(f)
                self.config.update(user_config)
            except Exception as e:
                print(f"Error loading config file: {e}")
        else:
            # Save defaults to config file if it doesn't exist
            self._save_config()

        # Load from environment variables
        load_dotenv()
        for key in self.config.keys():
            env_key = f"ESKIT_{key.upper()}"
            if env_key in os.environ:
                self.config[key] = os.environ[env_key]

    def _get_config_dir(self) -> Path:
        """Get the configuration directory"""
        return Path(platformdirs.user_config_dir(self.app_name, self.app_author))

    def _get_config_file(self) -> Path:
        """Get the configuration file path"""
        config_dir = self._get_config_dir()
        config_dir.mkdir(parents=True, exist_ok=True)
        return config_dir / "config.json"

    def _get_log_path(self) -> Path:
        """Get the log directory path"""
        log_dir = Path(platformdirs.user_log_dir(self.app_name, self.app_author))
        log_dir.mkdir(parents=True, exist_ok=True)
        return log_dir

    def get(self, key: str, default: Any = None) -> Any:
        """Get a configuration value"""
        return self.config.get(key, default)

    def set(self, key: str, value: Any) -> None:
        """Set a configuration value"""
        self.config[key] = value
        self._save_config()

    def _save_config(self) -> None:
        """Save the configuration to file"""
        config_file = self._get_config_file()
        try:
            with open(config_file, 'w') as f:
                json.dump(self.config, f, indent=4)
        except Exception as e:
            print(f"Error saving config file: {e}")
