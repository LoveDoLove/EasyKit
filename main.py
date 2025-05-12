#!/usr/bin/env python3
"""
Entry point script for EasyKit
"""
import sys
import os
from easykit.__main__ import main_menu

if __name__ == '__main__':
    if len(sys.argv) > 1:
        os.chdir(sys.argv[1])
    main_menu()
