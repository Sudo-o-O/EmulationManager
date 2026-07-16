# Emulation Manager

Emulation Manager is a Windows game-library scanner and emulator launcher designed for Steam integration.

## Current features

- Scans a structured ROM library
- Detects base games, updates, and DLC
- Detects Eden Stable and Eden Nightly
- Launches Nintendo Switch games
- Supports command-line launching with --launch

## Current default paths

- ROMs: D:\Games\ROMs
- Eden Stable: C:\Emulation\Emulators\Eden\eden.exe
- Eden Nightly: C:\Emulation\Emulators\Eden Nightly\eden.exe

These paths will become user-configurable in a future release.

## Build

dotnet build --configuration Release

## Run

dotnet run --configuration Release

## Command-line launch

EmulationManager.exe --launch "D:\path\to\game.nsp"

## Legal

This project does not include games, firmware, encryption keys, BIOS files, or emulator binaries. Users are responsible for complying with applicable laws and emulator requirements.
