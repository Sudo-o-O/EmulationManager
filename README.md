# Emulation Manager

A modern, open-source emulator launcher designed to provide a unified game library with Steam integration and cross-platform support.

> **Status:** 🚧 Active Development

---

## Features

### Current

- ✅ Nintendo Switch support (Eden Stable & Nightly)
- ✅ Automatic ROM library scanning
- ✅ Configurable launcher profiles
- ✅ Multiple launch strategies
  - Direct
  - Detached
  - Auto
  - Ask
- ✅ Steam-compatible launching
- ✅ Per-launcher default settings
- ✅ Per-game launch overrides
- ✅ Windows support
- ✅ Open-source

---

## Planned Features

- Steam shortcut generation
- Automatic Steam synchronization
- SteamGridDB artwork
- Game metadata
- Favorites
- Recently Played
- Multiple emulator support
  - RPCS3
  - Dolphin
  - PCSX2
  - Cemu
  - Xenia
- Linux support
- Steam Deck support
- Plugin system

---

## Project Goals

Emulation Manager aims to provide a single launcher for multiple emulators while keeping Steam integration simple and reliable.

The long-term goals include:

- One unified game library
- Cross-platform support (Windows & Linux)
- Automatic Steam integration
- Automatic artwork downloads
- Metadata management
- Extensible plugin architecture

---

## Current Project Structure

```
EmulationManager
│
├── Configuration
├── Forms
├── LaunchStrategies
├── Models
├── Services
├── Steam
└── Platforms (planned)
```

---

## Building

Requirements:

- .NET 10 SDK
- Windows 11 (currently)
- VS Code or Visual Studio

Clone the repository:

```bash
git clone https://github.com/Sudo-o-O/EmulationManager.git
```

Build:

```bash
dotnet build
```

Run:

```bash
dotnet run
```

---

## Roadmap

### Version 0.2

- [ ] Steam Export
- [ ] Steam Synchronization
- [ ] Metadata System
- [ ] Artwork Downloading

### Version 0.3

- [ ] Multiple Emulator Support
- [ ] Linux Support
- [ ] Steam Deck Support

### Version 1.0

- [ ] Plugin System
- [ ] Auto Updates
- [ ] Cross-platform Release

---

## Contributing

Contributions, feature requests, and bug reports are welcome.

Please open an issue before beginning work on large features.

---

## License

License to be determined.