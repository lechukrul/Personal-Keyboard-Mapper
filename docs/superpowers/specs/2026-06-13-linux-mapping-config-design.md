# Linux Mapping Config — Design

**Date:** 2026-06-13  
**Branch:** linux-port  
**Scope:** Podłączenie logiki mapowania klawiszy do implementacji Linux z konfiguracją JSON

---

## Cel

Zastąpić hardcoded logikę w `Program.cs` (PoC) systemem wczytującym mapowania kombinacji klawiszy z pliku JSON — analogicznie do Windows app (`.keysconfig`). Format pliku identyczny z Windows, bez zależności od Win32 API.

---

## Architektura

### Nowe pliki — `Personal-Keyboard-Mapper.Core/Config/`

| Plik | Odpowiedzialność |
|------|-----------------|
| `MappingConfig.cs` | POCO: `MappingConfig`, `CombinationEntry`, `ActionEntry` |
| `MappingConfigLoader.cs` | Wczytuje i deserializuje JSON przez `System.Text.Json` |
| `KeyAliasResolver.cs` | Mapuje aliasy string → `VirtualKeyCode` lub akcja myszy |
| `MappingEngine.cs` | Maszyna stanów 2-klawiszowa; wykonuje akcje przez `IInputSimulator` |

Żadnych zależności Windows. `System.Text.Json` jest standardem .NET 8 — bez dodatkowych paczek.

---

## Format pliku JSON

Identyczny z Windows `default.keysconfig`:

```json
{
  "CombinationSize": 2,
  "Combinations": [
    {
      "FirstKey": 1,
      "SecondKey": 2,
      "Action": {
        "Type": "Keyboard",
        "OutputVirtualKeys": ["ctrl", "c"]
      }
    },
    {
      "FirstKey": 1,
      "SecondKey": 3,
      "Action": {
        "Type": "Mouse",
        "OutputVirtualKeys": ["lclick"]
      }
    }
  ]
}
```

### Mapowanie klawiszy (FirstKey / SecondKey)

Liczby całkowite 0–9 → `VirtualKeyCode.NUMPAD0`–`VirtualKeyCode.NUMPAD9`.  
Kody VK numapadów są kolejne (96–105), więc: `VirtualKeyCode.NUMPAD0 + n`.  
Nie używamy Win32 `VkKeyScan` — mapowanie jest deterministyczne.

### Aliasy w OutputVirtualKeys

| Alias | Akcja |
|-------|-------|
| `"ctrl"` / `"crtl"` | `VirtualKeyCode.CONTROL` |
| `"shift"` | `VirtualKeyCode.SHIFT` |
| `"alt"` | `VirtualKeyCode.LMENU` |
| `"ralt"` | `VirtualKeyCode.RMENU` |
| `"win"` | `VirtualKeyCode.LWIN` |
| `"lclick"` | `simulator.MouseLeftClick()` |
| `"rclick"` | `simulator.MouseRightClick()` |
| `"ldclick"` | `simulator.MouseLeftDoubleClick()` |
| `"rdclick"` | `simulator.MouseRightDoubleClick()` |
| `" "` | `VirtualKeyCode.SPACE` |
| `"a"`–`"z"` | `VirtualKeyCode.VK_A`–`VirtualKeyCode.VK_Z` |
| `"0"`–`"9"` | `VirtualKeyCode.VK_0`–`VirtualKeyCode.VK_9` |

Nieznane aliasy są ignorowane z ostrzeżeniem na konsolę.

---

## MappingEngine — logika stanów

Maszyna przetwarza eventy z `EvdevKeyboardHook.KeyHandler`:

```
KeyState.Down + klawisz jest FirstKey jakiejś kombinacji
  → zapamiętaj jako pendingFirstKey, zwróć true (suppress)

KeyState.Down + pendingFirstKey ustawiony + klawisz pasuje jako SecondKey
  → wykonaj akcję, wyczyść pendingFirstKey, zwróć true (suppress)

KeyState.Down + pendingFirstKey ustawiony + klawisz NIE pasuje jako SecondKey, ale jest FirstKey
  → zastąp pendingFirstKey nowym, zwróć true (suppress — nowy "start" kombinacji)

KeyState.Down + pendingFirstKey ustawiony + klawisz NIE pasuje w ogóle
  → wyczyść pendingFirstKey, zwróć false (przepuść ten klawisz; pierwszy jest stracony)

KeyState.Up/Repeat + klawisz był suppressed (pending lub część kombinacji)
  → zwróć true (suppress — para Down+Up musi być spójna)

Każdy inny klawisz
  → zwróć false (przepuść bez zmian)
```

Klucz: system musi widzieć pary Down+Up albo nic — nigdy Up bez Down.

### Wykonanie akcji przez IInputSimulator

- `Type: "Keyboard"` z modifikatorami + zwykłe klawisze → `simulator.ModifiedKeyStroke(modKeys, regularKeys)`
- `Type: "Keyboard"` bez modifikatorów → `simulator.KeyPress(key)`
- `Type: "Keyboard"` tylko modifikator → `simulator.KeyPress(modKey)`
- `Type: "Mouse"` → odpowiednia metoda symulatora

---

## Program.cs — wiring

```csharp
string device    = args.Length > 0 ? args[0] : "/dev/input/event0";
string configPath = args.Length > 1 ? args[1] : "default.keysconfig";

using var uinput    = new UinputDevice();
using var hook      = new EvdevKeyboardHook(device, uinput);
var simulator       = new UinputInputSimulator(uinput);

var config = MappingConfigLoader.Load(configPath);
var engine = new MappingEngine(config, simulator);

hook.KeyHandler = engine.HandleKey;

var exitRequested = new ManualResetEventSlim();
engine.OnExitRequested = () => exitRequested.Set();  // ESC hardcoded

hook.StartHook();
exitRequested.Wait();
hook.StopHook();
```

ESC pozostaje hardcoded jako wyjście — bezpiecznik, nie konfigurowalny.

Plik `default.keysconfig` kopiowany do katalogu output przez MSBuild (tak samo jak w Windows app).

---

## Uwagi implementacyjne

- **NUMLOCK:** VirtualBox emituje syntetyczne eventy NUMLOCK wokół każdego klawisza numpada. Jeśli NUMLOCK nie jest w konfiguracji jako FirstKey, engine go przepuści (reset pending). Może być konieczne zachowanie z PoC: ignorować NUMLOCK w `Program.cs` przed przekazaniem do engine (zwrócić `false` bez zmiany stanu).

---

## Co NIE jest w scope

- UI do edycji konfiguracji
- Walidacja duplikatów kombinacji (można dodać później)
- Obsługa `CombinationSize: 3`
- Hot-reload konfiguracji bez restartu
