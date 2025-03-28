#!/usr/bin/env python3
import os
import zipfile
import glob
import argparse
from datetime import datetime

# Configuration
VERSION = "1.0.0"
OUTPUT_DIR = "Releases"

# Package configurations
NET_RELEASE_SOURCE_DIR = "../_Binaries/Release/net7.0/"
NET_RELEASE_FILES = [
    "Antlr4.Runtime.dll",
    "DamnScript.dll",
    "DamnScript.pdb",
    "DamnScript.deps.json",
]

NET_DEBUG_SOURCE_DIR = "../_Binaries/Debug/net7.0/"
NET_DEBUG_FILES = [
    "Antlr4.Runtime.dll",
    "DamnScript.dll",
    "DamnScript.pdb",
    "DamnScript.deps.json",
]

UNITY_RELEASE_SOURCE_DIR = "../DamnScript/"
UNITY_RELEASE_FILES = [
    "Runtimes/",
    "Parsings/",
    "obj/Release/net7.0/DamnScriptLexer.cs",
    "obj/Release/net7.0/DamnScriptLexer.tokens",
    "obj/Release/net7.0/DamnScriptParser.cs",
    "obj/Release/net7.0/DamnScriptParser.tokens",
    "obj/Release/net7.0/DamnScriptParserBaseListener.cs",
    "obj/Release/net7.0/DamnScriptParserBaseVisitor.cs",
    "obj/Release/net7.0/DamnScriptParserListener.cs",
    "obj/Release/net7.0/DamnScriptParserVisitor.cs",
    "../_Binaries/Release/net7.0/Antlr4.Runtime.dll"
]

UNITY_DEBUG_SOURCE_DIR = "../DamnScript/"
UNITY_DEBUG_FILES = [
    "Runtimes/",
    "Parsings/",
    "obj/Debug/net7.0/DamnScriptLexer.cs",
    "obj/Debug/net7.0/DamnScriptLexer.tokens",
    "obj/Debug/net7.0/DamnScriptParser.cs",
    "obj/Debug/net7.0/DamnScriptParser.tokens",
    "obj/Debug/net7.0/DamnScriptParserBaseListener.cs",
    "obj/Debug/net7.0/DamnScriptParserBaseVisitor.cs",
    "obj/Debug/net7.0/DamnScriptParserListener.cs",
    "obj/Debug/net7.0/DamnScriptParserVisitor.cs",
    "../_Binaries/Debug/net7.0/Antlr4.Runtime.dll"
]

def create_zip_package(source_dir, files_to_include, output_filename):
    """Creates a zip package with specified files from source directory"""
    if not os.path.exists(OUTPUT_DIR):
        os.makedirs(OUTPUT_DIR)

    zip_path = os.path.join(OUTPUT_DIR, output_filename)
    ignore_files = {".DS_Store"}

    with zipfile.ZipFile(zip_path, 'w', zipfile.ZIP_DEFLATED) as zipf:
        for pattern in files_to_include:
            if pattern.endswith('/') or pattern.endswith('\\'):
                dir_path = os.path.join(source_dir, pattern)
                if os.path.exists(dir_path):
                    for root, _, files in os.walk(dir_path):
                        for file in files:
                            if file in ignore_files:
                                continue
                            file_path = os.path.join(root, file)
                            arcname = os.path.relpath(file_path, source_dir)
                            if arcname.startswith("obj/"):
                                arcname = os.path.join("Antrls", os.path.basename(file_path))
                            print(f"Adding {arcname} to zip")
                            zipf.write(file_path, arcname)
                else:
                    print(f"Warning: Directory not found: {dir_path}")
            else:
                matches = glob.glob(os.path.join(source_dir, pattern))
                if matches:
                    for match in matches:
                        if os.path.isfile(match) and os.path.basename(match) not in ignore_files:
                            arcname = os.path.basename(match)
                            if "Antlr4.Runtime.dll" in match or "obj/" in match:
                                arcname = os.path.join("Antrls", os.path.basename(match))
                            print(f"Adding {arcname} to zip")
                            zipf.write(match, arcname)
                else:
                    print(f"Warning: No matches found for pattern: {pattern}")

    print(f"Package created at: {zip_path}")
    return zip_path

def create_net_release():
    """Creates a .NET Release package"""
    print("\n=== Creating .NET Release Package ===")
    output_filename = f"DamnScript_v{VERSION}_NET_Binaries_Release.zip"
    return create_zip_package(NET_RELEASE_SOURCE_DIR, NET_RELEASE_FILES, output_filename)

def create_net_debug():
    """Creates a .NET Debug package"""
    print("\n=== Creating .NET Debug Package ===")
    output_filename = f"DamnScript_v{VERSION}_NET_Binaries_Debug.zip"
    return create_zip_package(NET_DEBUG_SOURCE_DIR, NET_DEBUG_FILES, output_filename)

def create_unity_release():
    """Creates a Unity Release package"""
    print("\n=== Creating Unity Release Package ===")
    output_filename = f"DamnScript_v{VERSION}_Unity_Source_Release.zip"
    return create_zip_package(UNITY_RELEASE_SOURCE_DIR, UNITY_RELEASE_FILES, output_filename)

def create_unity_debug():
    """Creates a Unity Debug package"""
    print("\n=== Creating Unity Debug Package ===")
    output_filename = f"DamnScript_v{VERSION}_Unity_Source_Debug.zip"
    return create_zip_package(UNITY_DEBUG_SOURCE_DIR, UNITY_DEBUG_FILES, output_filename)

def main():
    global VERSION
    parser = argparse.ArgumentParser(description='Create DamnScript packages')
    parser.add_argument('--all', action='store_true', help='Create all packages')
    parser.add_argument('--net-release', action='store_true', help='Create .NET Release package')
    parser.add_argument('--net-debug', action='store_true', help='Create .NET Debug package')
    parser.add_argument('--unity-release', action='store_true', help='Create Unity Release package')
    parser.add_argument('--unity-debug', action='store_true', help='Create Unity Debug package')
    parser.add_argument('--version', type=str, help=f'Package version (default: {VERSION})')

    args = parser.parse_args()

    # If no specific package type is selected, create all
    create_all = args.all or not (args.net_release or args.net_debug or
                                  args.unity_release or args.unity_debug)

    # Update version if specified
    if args.version:
        VERSION = args.version

    if create_all or args.net_release:
        create_net_release()

    if create_all or args.net_debug:
        create_net_debug()

    if create_all or args.unity_release:
        create_unity_release()

    if create_all or args.unity_debug:
        create_unity_debug()

if __name__ == "__main__":
    main()