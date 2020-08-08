"""This script is for generating .parsetree files automatically for unit testing the parser."""

import os
import argparse

# Folder name
test_scripts_path = "test-scripts"
parser = argparse.ArgumentParser()
parser.add_argument("--verbose", "-v", action="store_true")
namespace = parser.parse_args()

for f in os.listdir(test_scripts_path):
    if f.endswith('.boink'):
        stdout = os.popen(f"powershell ../boinkmain/bin/debug/netcoreapp3.1/boinkmain.exe parse {os.path.join(test_scripts_path, f)} {test_scripts_path}")
        if namespace.verbose:
            print(stdout.read())