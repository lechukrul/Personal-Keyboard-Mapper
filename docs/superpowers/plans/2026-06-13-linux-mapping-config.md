# Linux Mapping Config — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Zastąpić hardcoded logikę PoC w `Program.cs` systemem wczytującym mapowania kombinacji klawiszy z pliku JSON, identycznym z Windows `.keysconfig`.

**Architecture:** Cztery nowe klasy w `Personal-Keyboard-Mapper.Core/Config/` tworzą pipeline: `MappingConfigLoader` wczytuje JSON → `MappingConfig` to POCO model → `KeyAliasResolver` tłumaczy aliasy string na `VirtualKeyCode` → `MappingEngine` to maszyna stanów która przetwarza `KeyEvent` z hooka i wywołuje `IInputSimulator`. `Program.cs` podpina engine jako `KeyHandler`. Brak frameworka testów — weryfikacja przez build + ręczne uruchomienie na maszynie Linux.

**Tech Stack:** C# .NET 8, `System.Text.Json` (wbudowany w .NET 8), `Personal-Keyboard-Mapper.Core` (VirtualKeyCode, IInputSimulator, KeyEvent, KeyState)

---

## Mapa plików

| Plik | Akcja | Odpowiedzialność |
|------|-------|-----------------|
| `Personal-Keyboard-Mapper.Core/Config/MappingConfig.cs` | Utwórz | POCO: MappingConfig, CombinationEntry, ActionEntry |
| `Personal-Keyboard-Mapper.Core/Config/MappingConfigLoader.cs` | Utwórz | Wczytuje i deserializuje JSON |
| `Personal-Keyboard-Mapper.Core/Config/KeyAliasResolver.cs` | Utwórz | Mapuje aliasy string → VirtualKeyCode |
| `Personal-Keyboard-Mapper.Core/Config/MappingEngine.cs` | Utwórz | Maszyna stanów 2-klawiszowa |
| `Personal-Keyboard-Mapper.Linux/default.keysconfig` | Utwórz | Domyślna konfiguracja dla Linux |
| `Personal-Keyboard-Mapper.Linux/Personal-Keyboard-Mapper.Linux.csproj` | Modyfikuj | Dodaj MSBuild copy dla keysconfig |
| `Personal-Keyboard-Mapper.Linux/Program.cs` | Modyfikuj | Zastąp hardcoded logikę engine + config |

---

### Task 1: MappingConfig — modele POCO

**Files:**
- Create: `Personal-Keyboard-Mapper.Core/Config/MappingConfig.cs`

- [ ] **Krok 1: Utwórz plik z modelami**

```csharp
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Personal_Keyboard_Mapper.Core.Config
{
    public class MappingConfig
    {
        [JsonPropertyName("CombinationSize")]
        public int CombinationSize { get; set; }

        [JsonPropertyName("Combinations")]
        public List<CombinationEntry> Combinations { get; set; }
    }

    public class CombinationEntry
    {
        [JsonPropertyName("FirstKey")]
        public int FirstKey { get; set; }

        [JsonPropertyName("SecondKey")]
        public int SecondKey { get; set; }

        [JsonPropertyName("Action")]
        public ActionEntry Action { get; set; }
    }

    public class ActionEntry
    {
        [JsonPropertyName("Type")]
        public string Type { get; set; }

        [JsonPropertyName("OutputVirtualKeys")]
        public List<string> OutputVirtualKeys { get; set; }
    }
}
```

- [ ] **Krok 2: Sprawdź że projekt kompiluje**

```powershell
cd Personal-Keyboard-Mapper.Core
dotnet build
```
Oczekiwane: `Build succeeded. 0 Error(s)`

- [ ] **Krok 3: Commit**

```bash
git add Personal-Keyboard-Mapper.Core/Config/MappingConfig.cs
git commit -m "Add MappingConfig POCO models for Linux JSON config"
```

---

### Task 2: MappingConfigLoader — wczytywanie JSON

**Files:**
- Create: `Personal-Keyboard-Mapper.Core/Config/MappingConfigLoader.cs`

- [ ] **Krok 1: Utwórz loader**

```csharp
using System;
using System.IO;
using System.Text.Json;

namespace Personal_Keyboard_Mapper.Core.Config
{
    public static class MappingConfigLoader
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public static MappingConfig Load(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Config file not found: {path}");

            var json = File.ReadAllText(path);
            var config = JsonSerializer.Deserialize<MappingConfig>(json, Options);

            if (config == null)
                throw new InvalidOperationException($"Failed to deserialize config from: {path}");
            if (config.Combinations == null)
                throw new InvalidOperationException("Config is missing 'Combinations' array.");

            return config;
        }
    }
}
```

- [ ] **Krok 2: Sprawdź że projekt kompiluje**

```powershell
cd Personal-Keyboard-Mapper.Core
dotnet build
```
Oczekiwane: `Build succeeded. 0 Error(s)`

- [ ] **Krok 3: Commit**

```bash
git add Personal-Keyboard-Mapper.Core/Config/MappingConfigLoader.cs
git commit -m "Add MappingConfigLoader — reads .keysconfig JSON via System.Text.Json"
```

---

### Task 3: KeyAliasResolver — tłumaczenie aliasów

**Files:**
- Create: `Personal-Keyboard-Mapper.Core/Config/KeyAliasResolver.cs`

- [ ] **Krok 1: Utwórz resolver**

```csharp
using System;
using Personal_Keyboard_Mapper.Core.Enums;

namespace Personal_Keyboard_Mapper.Core.Config
{
    public static class KeyAliasResolver
    {
        private static readonly VirtualKeyCode[] ModifierCodes =
        {
            VirtualKeyCode.CONTROL, VirtualKeyCode.SHIFT,
            VirtualKeyCode.LMENU,   VirtualKeyCode.RMENU,
            VirtualKeyCode.LWIN,    VirtualKeyCode.RWIN,
        };

        /// <summary>
        /// Tłumaczy alias string (np. "ctrl", "c", " ") na VirtualKeyCode.
        /// Nieznane aliasy zwracają false i wypisują ostrzeżenie.
        /// </summary>
        public static bool TryResolve(string alias, out VirtualKeyCode key)
        {
            key = default;

            switch (alias.ToLowerInvariant())
            {
                case "ctrl":
                case "crtl":  // literówka obecna w Windows default.keysconfig
                    key = VirtualKeyCode.CONTROL; return true;
                case "shift":
                    key = VirtualKeyCode.SHIFT; return true;
                case "alt":
                    key = VirtualKeyCode.LMENU; return true;
                case "ralt":
                    key = VirtualKeyCode.RMENU; return true;
                case "win":
                    key = VirtualKeyCode.LWIN; return true;
                case " ":
                    key = VirtualKeyCode.SPACE; return true;
                case "enter":
                    key = VirtualKeyCode.RETURN; return true;
                case "tab":
                    key = VirtualKeyCode.TAB; return true;
                case "back":
                case "backspace":
                    key = VirtualKeyCode.BACK; return true;
                case "delete":
                case "del":
                    key = VirtualKeyCode.DELETE; return true;
                case "esc":
                case "escape":
                    key = VirtualKeyCode.ESCAPE; return true;
                case "left":
                    key = VirtualKeyCode.LEFT; return true;
                case "right":
                    key = VirtualKeyCode.RIGHT; return true;
                case "up":
                    key = VirtualKeyCode.UP; return true;
                case "down":
                    key = VirtualKeyCode.DOWN; return true;
            }

            if (alias.Length == 1)
            {
                char c = alias[0];
                if (c >= 'a' && c <= 'z')
                {
                    key = VirtualKeyCode.VK_A + (c - 'a'); return true;
                }
                if (c >= 'A' && c <= 'Z')
                {
                    key = VirtualKeyCode.VK_A + (char.ToLowerInvariant(c) - 'a'); return true;
                }
                if (c >= '0' && c <= '9')
                {
                    key = VirtualKeyCode.VK_0 + (c - '0'); return true;
                }
            }

            Console.WriteLine($"[warn] Unknown key alias: '{alias}'");
            return false;
        }

        public static bool IsModifier(VirtualKeyCode key)
            => Array.IndexOf(ModifierCodes, key) >= 0;
    }
}
```

- [ ] **Krok 2: Sprawdź że projekt kompiluje**

```powershell
cd Personal-Keyboard-Mapper.Core
dotnet build
```
Oczekiwane: `Build succeeded. 0 Error(s)`

- [ ] **Krok 3: Commit**

```bash
git add Personal-Keyboard-Mapper.Core/Config/KeyAliasResolver.cs
git commit -m "Add KeyAliasResolver — maps string aliases to VirtualKeyCode"
```

---

### Task 4: MappingEngine — maszyna stanów

**Files:**
- Create: `Personal-Keyboard-Mapper.Core/Config/MappingEngine.cs`

Logika suppress: każdy klawisz którego `KeyDown` został suppressed trafia do `_suppressedKeys`. Jego `KeyUp`/`Repeat` są automatycznie suppressed i usuwane ze zbioru po `Up`. System zawsze widzi pary Down+Up albo nic.

- [ ] **Krok 1: Utwórz engine**

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using Personal_Keyboard_Mapper.Core.Enums;
using Personal_Keyboard_Mapper.Core.Interfaces;
using Personal_Keyboard_Mapper.Core.Model;

namespace Personal_Keyboard_Mapper.Core.Config
{
    public class MappingEngine
    {
        private readonly MappingConfig _config;
        private readonly IInputSimulator _simulator;

        private VirtualKeyCode? _pendingFirstKey;
        private readonly HashSet<VirtualKeyCode> _suppressedKeys = new HashSet<VirtualKeyCode>();

        /// <summary>
        /// Wywoływane gdy ESC zostanie naciśnięty (hardcoded exit).
        /// </summary>
        public Action OnExitRequested { get; set; }

        public MappingEngine(MappingConfig config, IInputSimulator simulator)
        {
            _config = config;
            _simulator = simulator;
        }

        /// <summary>
        /// Główny handler — podpinany jako EvdevKeyboardHook.KeyHandler.
        /// Zwraca true = suppress (nie przekazuj do systemu), false = przepuść.
        /// </summary>
        public bool HandleKey(KeyEvent ev)
        {
            // ESC: hardcoded bezpiecznik wyjścia
            if (ev.Key == VirtualKeyCode.ESCAPE && ev.State == KeyState.Down)
            {
                OnExitRequested?.Invoke();
                return true;
            }

            // Suppress Up/Repeat dla klawiszy, których Down był suppressed
            if (_suppressedKeys.Contains(ev.Key) && ev.State != KeyState.Down)
            {
                if (ev.State == KeyState.Up)
                    _suppressedKeys.Remove(ev.Key);
                return true;
            }

            if (ev.State != KeyState.Down)
                return false;

            var isFirstKey = IsFirstKey(ev.Key);

            if (_pendingFirstKey.HasValue)
            {
                var match = FindCombination(_pendingFirstKey.Value, ev.Key);
                if (match != null)
                {
                    // Kombinacja dopasowana — wykonaj akcję
                    _suppressedKeys.Add(ev.Key);
                    _pendingFirstKey = null;
                    ExecuteAction(match.Action);
                    return true;
                }

                if (isFirstKey)
                {
                    // Nowy pierwszy klawisz — zastąp pending
                    _pendingFirstKey = ev.Key;
                    _suppressedKeys.Add(ev.Key);
                    return true;
                }

                // Klawisz nie pasuje do żadnej kombinacji — anuluj pending, przepuść
                _pendingFirstKey = null;
                return false;
            }

            if (isFirstKey)
            {
                _pendingFirstKey = ev.Key;
                _suppressedKeys.Add(ev.Key);
                return true;
            }

            return false;
        }

        private bool IsFirstKey(VirtualKeyCode key)
            => _config.Combinations.Any(c => NumpadToVk(c.FirstKey) == key);

        private CombinationEntry FindCombination(VirtualKeyCode first, VirtualKeyCode second)
            => _config.Combinations.FirstOrDefault(c =>
                NumpadToVk(c.FirstKey) == first &&
                NumpadToVk(c.SecondKey) == second);

        private static VirtualKeyCode NumpadToVk(int index)
            => VirtualKeyCode.NUMPAD0 + index;

        private void ExecuteAction(ActionEntry action)
        {
            if (string.Equals(action.Type, "Mouse", StringComparison.OrdinalIgnoreCase))
            {
                ExecuteMouseAction(action);
                return;
            }

            // Keyboard action
            var resolved = new List<VirtualKeyCode>();
            foreach (var alias in action.OutputVirtualKeys ?? Enumerable.Empty<string>())
            {
                if (KeyAliasResolver.TryResolve(alias, out var vk))
                    resolved.Add(vk);
            }

            if (resolved.Count == 0) return;

            var modKeys     = resolved.Where(KeyAliasResolver.IsModifier).ToList();
            var regularKeys = resolved.Where(k => !KeyAliasResolver.IsModifier(k)).ToList();

            if (regularKeys.Count == 0)
            {
                // Tylko modifikatory — naciśnij każdy osobno
                foreach (var mod in modKeys)
                    _simulator.KeyPress(mod);
            }
            else if (modKeys.Count == 0)
            {
                foreach (var k in regularKeys)
                    _simulator.KeyPress(k);
            }
            else
            {
                _simulator.ModifiedKeyStroke(modKeys, regularKeys);
            }
        }

        private void ExecuteMouseAction(ActionEntry action)
        {
            foreach (var alias in action.OutputVirtualKeys ?? Enumerable.Empty<string>())
            {
                switch (alias.ToLowerInvariant())
                {
                    case "lclick":  _simulator.MouseLeftClick();        break;
                    case "rclick":  _simulator.MouseRightClick();       break;
                    case "ldclick": _simulator.MouseLeftDoubleClick();  break;
                    case "rdclick": _simulator.MouseRightDoubleClick(); break;
                    default:
                        Console.WriteLine($"[warn] Unknown mouse action: '{alias}'"); break;
                }
            }
        }
    }
}
```

- [ ] **Krok 2: Sprawdź że projekt kompiluje**

```powershell
cd Personal-Keyboard-Mapper.Core
dotnet build
```
Oczekiwane: `Build succeeded. 0 Error(s)`

- [ ] **Krok 3: Commit**

```bash
git add Personal-Keyboard-Mapper.Core/Config/MappingEngine.cs
git commit -m "Add MappingEngine — two-key stateful combination matcher"
```

---

### Task 5: default.keysconfig i MSBuild copy

**Files:**
- Create: `Personal-Keyboard-Mapper.Linux/default.keysconfig`
- Modify: `Personal-Keyboard-Mapper.Linux/Personal-Keyboard-Mapper.Linux.csproj`

- [ ] **Krok 1: Utwórz domyślny plik konfiguracji**

Ścieżka: `Personal-Keyboard-Mapper.Linux/default.keysconfig`

```json
{
  "CombinationSize": 2,
  "Combinations": [
    {
      "FirstKey": 1,
      "SecondKey": 1,
      "Action": {
        "Type": "Keyboard",
        "OutputVirtualKeys": [ " " ]
      }
    },
    {
      "FirstKey": 1,
      "SecondKey": 2,
      "Action": {
        "Type": "Keyboard",
        "OutputVirtualKeys": [ "ctrl", "c" ]
      }
    },
    {
      "FirstKey": 1,
      "SecondKey": 3,
      "Action": {
        "Type": "Keyboard",
        "OutputVirtualKeys": [ "ctrl", "v" ]
      }
    },
    {
      "FirstKey": 1,
      "SecondKey": 4,
      "Action": {
        "Type": "Keyboard",
        "OutputVirtualKeys": [ "ctrl", "z" ]
      }
    },
    {
      "FirstKey": 2,
      "SecondKey": 1,
      "Action": {
        "Type": "Mouse",
        "OutputVirtualKeys": [ "lclick" ]
      }
    },
    {
      "FirstKey": 2,
      "SecondKey": 2,
      "Action": {
        "Type": "Mouse",
        "OutputVirtualKeys": [ "ldclick" ]
      }
    },
    {
      "FirstKey": 2,
      "SecondKey": 3,
      "Action": {
        "Type": "Mouse",
        "OutputVirtualKeys": [ "rclick" ]
      }
    }
  ]
}
```

- [ ] **Krok 2: Dodaj MSBuild copy do .csproj**

W pliku `Personal-Keyboard-Mapper.Linux/Personal-Keyboard-Mapper.Linux.csproj` dodaj przed `</Project>`:

```xml
  <ItemGroup>
    <Content Include="default.keysconfig">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
  </ItemGroup>
```

Pełny plik po zmianie:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <RootNamespace>Personal_Keyboard_Mapper.Linux</RootNamespace>
    <AssemblyName>Personal_Keyboard_Mapper.Linux</AssemblyName>
    <LangVersion>latest</LangVersion>
    <Nullable>disable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\Personal-Keyboard-Mapper.Core\Personal-Keyboard-Mapper.Core.csproj" />
  </ItemGroup>
  <ItemGroup>
    <Content Include="default.keysconfig">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
  </ItemGroup>
</Project>
```

- [ ] **Krok 3: Sprawdź że projekt kompiluje i plik jest w output**

```powershell
cd Personal-Keyboard-Mapper.Linux
dotnet build
```
Oczekiwane: `Build succeeded. 0 Error(s)` i plik `bin/Debug/net8.0/default.keysconfig` istnieje.

- [ ] **Krok 4: Commit**

```bash
git add Personal-Keyboard-Mapper.Linux/default.keysconfig
git add Personal-Keyboard-Mapper.Linux/Personal-Keyboard-Mapper.Linux.csproj
git commit -m "Add default.keysconfig for Linux and wire MSBuild copy"
```

---

### Task 6: Aktualizacja Program.cs

**Files:**
- Modify: `Personal-Keyboard-Mapper.Linux/Program.cs`

Zastępujemy całą hardcoded logikę (zmienne `firstKey`, ręczne sprawdzanie NUMPAD1/NUMPAD2) przez `MappingConfigLoader` + `MappingEngine`. NUMLOCK nadal ignorowany przed przekazaniem do engine (VirtualBox quirk z PoC).

- [ ] **Krok 1: Zastąp zawartość Program.cs**

```csharp
using System;
using System.Threading;
using Personal_Keyboard_Mapper.Core.Config;
using Personal_Keyboard_Mapper.Core.Enums;
using Personal_Keyboard_Mapper.Linux;
using Personal_Keyboard_Mapper.Linux.Native;

string device     = args.Length > 0 ? args[0] : "/dev/input/event0";
string configPath = args.Length > 1 ? args[1] : "default.keysconfig";

var config    = MappingConfigLoader.Load(configPath);
using var uinput    = new UinputDevice();
using var hook      = new EvdevKeyboardHook(device, uinput);
var simulator       = new UinputInputSimulator(uinput);
var engine          = new MappingEngine(config, simulator);
var exitRequested   = new ManualResetEventSlim();

engine.OnExitRequested = () => exitRequested.Set();

hook.KeyHandler = ev =>
{
    // VirtualBox emituje syntetyczne NUMLOCK wokół każdego klawisza numpada —
    // ignoruj je żeby nie resetowały stanu engine.
    if (ev.Key == VirtualKeyCode.NUMLOCK)
        return false;

    return engine.HandleKey(ev);
};

Console.WriteLine($"Loaded {config.Combinations.Count} combinations from '{configPath}'.");
Console.WriteLine($"Grabbing {device} — press ESC to exit.");

hook.StartHook();
exitRequested.Wait();
hook.StopHook();
Console.WriteLine("Grab released, bye.");
```

- [ ] **Krok 2: Sprawdź że projekt kompiluje**

```powershell
cd Personal-Keyboard-Mapper.Linux
dotnet build
```
Oczekiwane: `Build succeeded. 0 Error(s)`

- [ ] **Krok 3: Commit**

```bash
git add Personal-Keyboard-Mapper.Linux/Program.cs
git commit -m "Wire MappingEngine and MappingConfigLoader into Program.cs"
```

---

### Task 7: Weryfikacja na Linux VM

> Wykonaj ten task na maszynie Linux (Ubuntu) gdzie evdev/uinput działa.

- [ ] **Krok 1: Skopiuj/zbuilduj na Linux**

```bash
dotnet publish Personal-Keyboard-Mapper.Linux \
  -c Debug -r linux-x64 --no-self-contained \
  -o /tmp/km-linux
```

Lub uruchom bezpośrednio przez `dotnet run` w katalogu projektu.

- [ ] **Krok 2: Uruchom z domyślnym configiem**

```bash
sudo dotnet run --project Personal-Keyboard-Mapper.Linux -- /dev/input/eventX
```

Zamień `/dev/input/eventX` na właściwy device (sprawdź `ls /dev/input/by-id/`).

Oczekiwane:
```
Loaded 7 combinations from 'default.keysconfig'.
Grabbing /dev/input/eventX — press ESC to exit.
```

- [ ] **Krok 3: Przetestuj kombinacje**

Przetestuj każdą kombinację z `default.keysconfig`:
- NUMPAD1 → NUMPAD1: powinien wpisać spację
- NUMPAD1 → NUMPAD2: powinien wykonać Ctrl+C
- NUMPAD1 → NUMPAD3: powinien wykonać Ctrl+V
- NUMPAD2 → NUMPAD1: powinien wykonać lewy klik myszy
- ESC: powinien wyjść z programu

- [ ] **Krok 4: Przetestuj edge case — klawisz poza kombinacją**

Naciśnij klawisz który nie jest w żadnej kombinacji (np. litera A na klawiaturze) — powinien przejść do systemu normalnie.

- [ ] **Krok 5: Przetestuj edge case — nieznany drugi klawisz**

Naciśnij NUMPAD1 (pending), potem klawisz którego nie ma jako SecondKey dla 1 (np. NUMPAD9 jeśli nie ma go w configu) — NUMPAD9 powinien przejść do systemu, NUMPAD1 jest utracony (expected per design).

- [ ] **Krok 6: Commit po pomyślnej weryfikacji**

```bash
git commit --allow-empty -m "Verify: Linux mapping config works end-to-end"
```
