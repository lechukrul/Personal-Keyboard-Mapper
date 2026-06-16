# Linux Sound Support — Design Spec

**Date:** 2026-06-16
**Branch:** linux-port
**Status:** Approved

## Goal

Add audio feedback to the Linux version of Personal-Keyboard-Mapper, mirroring the behaviour of the Windows app. Sound is optional (`--no-sound` flag).

## Sounds and triggers

| Sound event | Trigger | WAV file |
|---|---|---|
| `FirstKey` | First key of a combination detected | `key1.wav` |
| `SecondKey` | Action executed (combination matched) | `key2.wav` |
| `Shift` | Shift modifier state activated | `shift.wav` |
| `Ctrl` | Ctrl modifier state activated | `ctrl.wav` |
| `Alt` | Alt modifier state activated | `alt.wav` |
| `Win` | Win modifier state activated | `win.wav` |

## Architecture

```
Program.cs
  ├─ parses --no-sound arg
  ├─ creates OpenAlSoundPlayer(AppContext.BaseDirectory)  [if sound enabled]
  └─ passes ISoundPlayer to MappingEngine

MappingEngine (Core)
  ├─ optional ISoundPlayer field (null = silent)
  ├─ PlaySound(FirstKey)       when _pendingFirstKey is set
  ├─ PlaySound(SecondKey)      at start of ExecuteAction()
  └─ PlaySound(Shift/Ctrl/Alt/Win)  inside UpdateModifierState()

Personal-Keyboard-Mapper.Core  [new files]
  ├─ Interfaces/ISoundPlayer.cs
  └─ Enums/SoundEvent.cs

Personal-Keyboard-Mapper.Linux  [new file]
  └─ OpenAlSoundPlayer.cs
```

## New Core abstractions

**`Enums/SoundEvent.cs`**
```csharp
public enum SoundEvent { FirstKey, SecondKey, Shift, Ctrl, Alt, Win }
```

**`Interfaces/ISoundPlayer.cs`**
```csharp
public interface ISoundPlayer
{
    void PlaySound(SoundEvent sound);
}
```

**`MappingEngine` constructor change:**
```csharp
public MappingEngine(MappingConfig config, IInputSimulator simulator, ISoundPlayer sound = null)
```

## OpenAlSoundPlayer

- NuGet: `Silk.NET.OpenAL`
- Loads all 6 WAV files from `resourceDir` (= `AppContext.BaseDirectory`) at construction time
- Pre-allocates 6 AL buffers and 6 AL sources (one per `SoundEvent`)
- `PlaySound()`: stops source if still playing, then `AL.SourcePlay()`
- Same-type sounds are cut off and restarted; different-type sounds play concurrently
- Implements `IDisposable` — deletes AL sources, buffers, destroys context on dispose

### WAV parser

Built-in, ~50 lines, PCM-only (all project WAVs are uncompressed PCM):
1. Read RIFF header, verify `"RIFF"` / `"WAVE"` magic
2. Find `fmt ` chunk — extract channels, sample rate, bit depth
3. Find `data` chunk — extract raw PCM bytes
4. Pass to `AL.BufferData()` with appropriate `ALFormat`

No additional NuGet required for parsing.

## Build changes (`Personal-Keyboard-Mapper.Linux.csproj`)

WAV files linked from the Windows project's `Properties/Resources/` folder:

```xml
<ItemGroup>
  <Content Include="..\Personal-Keyboard-Mapper\Properties\Resources\key1.wav">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    <Link>key1.wav</Link>
  </Content>
  <!-- repeated for key2, shift, ctrl, alt, win -->
</ItemGroup>
```

## Program.cs wiring

```csharp
bool soundEnabled = !args.Contains("--no-sound");
using var sound = soundEnabled
    ? new OpenAlSoundPlayer(AppContext.BaseDirectory)
    : null;
var engine = new MappingEngine(config, simulator, sound);
```

## Runtime requirements

| Dependency | Ubuntu package | Notes |
|---|---|---|
| `libopenal.so` | `libopenal1` | Usually pre-installed on Ubuntu Desktop |
| `aplay` / ALSA | not needed | OpenAL handles audio output |

If `libopenal1` is missing: `sudo apt install libopenal1`

## Out of scope

- ThirdKey sound (reserved for future 3-key combinations)
- Volume control
- Sound file configuration (hardcoded filenames)
- Windows-side refactor to use `ISoundPlayer`
