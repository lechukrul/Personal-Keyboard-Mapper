# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Purpose

Windows desktop accessibility tool that maps sequential two-key combinations (e.g. numpad sequences) to full keyboard/mouse actions, enabling people with motor disabilities to operate a computer with a minimal input device.

## Build Commands

```powershell
# Build (Debug)
msbuild Personal-Keyboard-Mapper.sln

# Build (Release)
msbuild Personal-Keyboard-Mapper.sln /p:Configuration=Release

# Clean
msbuild Personal-Keyboard-Mapper.sln /t:Clean
```

No test framework is configured. No linter is configured.

**Post-build** (automatic via MSBuild): copies audio resources from `Properties/Resources/` and `default.keysconfig` into the output directory. The executable requires `WindowsInput.dll` in the same directory to run.

## Solution Structure

| Project | Type | Framework | Role |
|---|---|---|---|
| `Personal-Keyboard-Mapper` | WinExe | .NET 4.7.2 | Main WinForms UI |
| `Personal_Keybard_Mapper.Lib` | Library | .NET 4.6.1 | All core logic (note: "Keybard" typo is intentional/historical) |
| `Personal-Keyboard-Mapper.Gui` | Library | .NET 4.6.1 | Floating overlay windows |
| `Personal_Keybard_Mapper.Console` | Exe | .NET 4.7.2 | Minimal console entry point |

## Architecture & Data Flow

```
Program.Main()
  └─ MainWindow  ←→  JsonConfigSource (.keysconfig files)
       └─ GlobalHookService  (central orchestrator)
             ├─ KeyboardHook  (WH_KEYBOARD_LL global hook)
             │    └─ ConfigHook()  →  matches KeyCombinationsConfiguration
             │                    →  executes IOutputAction via InputSimulator
             │                    →  plays KeysSoundEffects
             │                    →  updates HelpWindow overlay
             └─ MouseHook  (WH_MOUSE_LL global hook)
```

**Key matching flow**: `KeyboardHook.ConfigHook()` receives every system keypress, checks it against the loaded `KeyCombinationsConfiguration`, and on a full match executes the bound action.

## Core Abstractions (in `Personal_Keybard_Mapper.Lib`)

- `IKeyCombination` — implemented by `TwoKeysCombination` and `ThreeKeysCombination` (future)
- `IOutputAction` — implemented by `KeyboardAction`, `MouseAction`, and string-output actions
- `IConfigSource` — implemented by `JsonConfigSource` (only implementation)
- `IHook` — implemented by `KeyboardHook` and `MouseHook`
- `Globals` static class — shared state: active key codes, sound-enabled flag, resource sets

## Config File Format (`.keysconfig`)

JSON files deserialized by a custom `CombinationsConfigurationConverter` (JSON.NET):

```json
{
  "CombinationSize": 2,
  "Combinations": [
    {
      "FirstKey": "0",
      "SecondKey": "1",
      "Action": { ... }
    }
  ]
}
```

Action types and key aliases are defined in `Personal-Keyboard-Mapper/App.config`. The default config template is `default.keysconfig` (copied to output on build).

## Key Conventions

- Logging via **log4net** — every major class takes `ILog` via constructor injection; config in `log4net.config`.
- UI culture forced to Polish (`pl-PL`) at startup; English fallback via `.en-US` `.resx` variants.
- Audio feedback uses `.wav` files embedded as resources in `Properties/Resources/` — different sounds for regular keys vs. modifier keys.
- `KeyCombinationsConfiguration.GetCombinationInstance()` is the factory for instantiating combination types.

## Changelog


### 2026-05-06 18:29

- .gitignore

### 2026-05-08 17:45

- Personal-Keyboard-Mapper.Gui/Personal-Keyboard-Mapper.Gui.csproj
- Personal-Keyboard-Mapper.Gui/PredictionOverlayWindow.Designer.cs
- Personal-Keyboard-Mapper.Gui/PredictionOverlayWindow.cs
- Personal-Keyboard-Mapper/App.config
- Personal-Keyboard-Mapper/ConfigEditor.cs
- Personal-Keyboard-Mapper/MainWindow.Designer.cs
- Personal-Keyboard-Mapper/MainWindow.cs
- Personal-Keyboard-Mapper/MainWindow.resx
- Personal-Keyboard-Mapper/Personal-Keyboard-Mapper.csproj
- Personal-Keyboard-Mapper/Properties/Resources.Designer.cs
- Personal-Keyboard-Mapper/Properties/Settings.Designer.cs
- Personal-Keyboard-Mapper/Properties/aliases.Designer.cs
- Personal_Keybard_Mapper.Lib/App.config
- Personal_Keybard_Mapper.Lib/Globals.cs
- Personal_Keybard_Mapper.Lib/Hooks/KeyboardHook.cs
- Personal_Keybard_Mapper.Lib/Hooks/MouseHook.cs
- Personal_Keybard_Mapper.Lib/Personal_Keyboard_Mapper.Lib.csproj
- Personal_Keybard_Mapper.Lib/Prediction/WordFrequencyModel.cs
- Personal_Keybard_Mapper.Lib/Prediction/WordPredictionService.cs
- Personal_Keybard_Mapper.Lib/Service/GlobalHookService.cs
- README.md

### 2026-05-09 13:26

- Personal-Keyboard-Mapper/MainWindow.Designer.cs
- Personal-Keyboard-Mapper/MainWindow.cs
- Personal_Keybard_Mapper.Lib/Model/Action.cs
- Personal_Keybard_Mapper.Lib/Prediction/WordPredictionService.cs

### 2026-05-09 14:55

- Personal_Keybard_Mapper.Lib/Prediction/WordPredictionService.cs

### 2026-05-09 14:56

- Personal_Keybard_Mapper.Lib/Prediction/WordPredictionService.cs

### 2026-06-10

- Personal_Keybard_Mapper.Lib/Prediction/WordFrequencyModel.cs

### 2026-06-10 12:12

- Personal_Keybard_Mapper.Lib/Prediction/WordFrequencyModel.cs

### 2026-06-12 19:28

- Linux.Evdev.PoC/Linux.Evdev.PoC.csproj
- Linux.Evdev.PoC/Program.cs
- Personal-Keyboard-Mapper.Core/Enums/ActionType.cs
- Personal-Keyboard-Mapper.Core/Enums/KeyCombinationPosition.cs
- Personal-Keyboard-Mapper.Core/Enums/KeyState.cs
- Personal-Keyboard-Mapper.Core/Enums/VirtualKeyCode.cs
- Personal-Keyboard-Mapper.Core/Interfaces/IInputSimulator.cs
- Personal-Keyboard-Mapper.Core/Interfaces/IKeyboardHook.cs
- Personal-Keyboard-Mapper.Core/Model/KeyEvent.cs
- Personal-Keyboard-Mapper.Core/Personal-Keyboard-Mapper.Core.csproj
- Personal-Keyboard-Mapper.Linux/EvdevKeyboardHook.cs
- Personal-Keyboard-Mapper.Linux/KeyCodeMap.cs
- Personal-Keyboard-Mapper.Linux/Native/Libc.cs
- Personal-Keyboard-Mapper.Linux/Native/UinputDevice.cs
- Personal-Keyboard-Mapper.Linux/Personal-Keyboard-Mapper.Linux.csproj
- Personal-Keyboard-Mapper.Linux/Program.cs
- Personal-Keyboard-Mapper.Linux/UinputInputSimulator.cs

### 2026-06-13 15:56

- docs/superpowers/specs/2026-06-13-linux-mapping-config-design.md

### 2026-06-13 16:06

- docs/superpowers/plans/2026-06-13-linux-mapping-config.md

### 2026-06-13 16:09

- Personal-Keyboard-Mapper.Core/Config/MappingConfig.cs

### 2026-06-13 16:11

- Personal-Keyboard-Mapper.Core/Config/MappingConfigLoader.cs

### 2026-06-13 16:14

- Personal-Keyboard-Mapper.Core/Config/KeyAliasResolver.cs

### 2026-06-13 16:16

- Personal-Keyboard-Mapper.Core/Config/MappingEngine.cs
