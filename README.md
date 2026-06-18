# Android UI Automation Tool

A .NET 9 solution for automating Android tablet UI testing from Windows using ADB-connected devices and JSON instruction playlists.

## Projects
- `AndroidUIAutomation.Domain` - core models and instruction contracts
- `AndroidUIAutomation.Core` - execution engine, strategies, retry logic
- `AndroidUIAutomation.Infrastructure` - JSON parsing, file layout, ADB client, image matcher abstraction implementation
- `AndroidUIAutomation.UI.Console` - CLI runner with `--debug`, screenshot capture, sequence execution
- `AndroidUIAutomation.UI.Wpf` - WPF shell with start/pause/resume/stop controls
- `AndroidUIAutomation.Tests` - MSTest + Moq unit tests

## Folder Structure
```
AndroidUIAutomation/
├── InstructionSets/
├── SearchImages/
├── Screenshots/
└── config.json
```

## Console Usage
```bash
dotnet run --project ./AndroidUIAutomation.UI.Console -- --instruction-set sequence1.json --iterations 1
```

Capture screenshot:
```bash
dotnet run --project ./AndroidUIAutomation.UI.Console -- --capture --name screen1
```

Enable debug logging:
```bash
dotnet run --project ./AndroidUIAutomation.UI.Console -- --debug
```

## Testing
```bash
dotnet test ./AndroidUIAutomation.Tests/AndroidUIAutomation.Tests.csproj
```
