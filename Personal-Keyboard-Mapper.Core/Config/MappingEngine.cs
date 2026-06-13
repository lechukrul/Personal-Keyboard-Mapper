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
        private enum ModifierState { Inactive, SingleUse, Locked }

        private readonly MappingConfig _config;
        private readonly IInputSimulator _simulator;

        private VirtualKeyCode? _pendingFirstKey;
        private readonly HashSet<VirtualKeyCode> _suppressedKeys = new HashSet<VirtualKeyCode>();
        private readonly Dictionary<VirtualKeyCode, ModifierState> _modifierStates = new Dictionary<VirtualKeyCode, ModifierState>();

        public Action OnExitRequested { get; set; }

        public MappingEngine(MappingConfig config, IInputSimulator simulator)
        {
            _config = config;
            _simulator = simulator;
        }

        public bool HandleKey(KeyEvent ev)
        {
            // ESC: hardcoded exit trigger
            if (ev.Key == VirtualKeyCode.ESCAPE && ev.State == KeyState.Down)
            {
                OnExitRequested?.Invoke();
                return true;
            }

            // Suppress Up/Repeat for keys whose Down was suppressed
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
                    _suppressedKeys.Add(ev.Key);
                    _pendingFirstKey = null;
                    ExecuteAction(match.Action);
                    return true;
                }

                if (isFirstKey)
                {
                    _pendingFirstKey = ev.Key;
                    _suppressedKeys.Add(ev.Key);
                    return true;
                }

                _pendingFirstKey = null;
                _suppressedKeys.Add(ev.Key);
                return true;
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

        private void UpdateModifierState(VirtualKeyCode mod)
        {
            if (!_modifierStates.TryGetValue(mod, out var state))
                state = ModifierState.Inactive;

            bool winOnly = (mod == VirtualKeyCode.LWIN || mod == VirtualKeyCode.RWIN);

            switch (state)
            {
                case ModifierState.Inactive:
                    _modifierStates[mod] = ModifierState.SingleUse;
                    break;
                case ModifierState.SingleUse:
                    _modifierStates[mod] = winOnly ? ModifierState.Inactive : ModifierState.Locked;
                    break;
                case ModifierState.Locked:
                    _modifierStates[mod] = ModifierState.Inactive;
                    break;
            }
        }

        private List<VirtualKeyCode> GetAndConsumeActiveModifiers()
        {
            var active = new List<VirtualKeyCode>();
            var toDeactivate = new List<VirtualKeyCode>();

            foreach (var kvp in _modifierStates)
            {
                if (kvp.Value == ModifierState.Inactive) continue;
                active.Add(kvp.Key);
                if (kvp.Value == ModifierState.SingleUse)
                    toDeactivate.Add(kvp.Key);
            }

            foreach (var key in toDeactivate)
                _modifierStates[key] = ModifierState.Inactive;

            return active;
        }

        private void ExecuteAction(ActionEntry action)
        {
            if (string.Equals(action.Type, "Mouse", StringComparison.OrdinalIgnoreCase))
            {
                // Ctrl (locked) works with mouse clicks per the user manual
                var activeMods = GetAndConsumeActiveModifiers();
                foreach (var mod in activeMods) _simulator.KeyDown(mod);
                ExecuteMouseAction(action);
                foreach (var mod in activeMods) _simulator.KeyUp(mod);
                return;
            }

            var resolved = new List<VirtualKeyCode>();
            var textEntries = new List<string>();
            foreach (var alias in action.OutputVirtualKeys ?? Enumerable.Empty<string>())
            {
                if (KeyAliasResolver.TryResolve(alias, out var vk))
                    resolved.Add(vk);
                else
                    textEntries.Add(alias);
            }

            if (resolved.Count == 0 && textEntries.Count == 0) return;

            var configModKeys = resolved.Where(KeyAliasResolver.IsModifier).ToList();
            var regularKeys   = resolved.Where(k => !KeyAliasResolver.IsModifier(k)).ToList();

            // Modifier-only action (e.g. shift, ctrl, alt alone) → advance state machine
            if (textEntries.Count == 0 && regularKeys.Count == 0)
            {
                foreach (var mod in configModKeys)
                    UpdateModifierState(mod);
                return;
            }

            // Self-contained chord defined inline (e.g. ["ctrl","c"]) → execute directly,
            // leave the state machine untouched
            if (configModKeys.Count > 0 && regularKeys.Count > 0)
            {
                _simulator.ModifiedKeyStroke(configModKeys, regularKeys);
                return;
            }

            // Pure regular-key or TextEntry action → apply and consume pending modifiers
            var activeMods = GetAndConsumeActiveModifiers();

            foreach (var text in textEntries)
                _simulator.TextEntry(text);

            if (regularKeys.Count > 0)
            {
                if (activeMods.Count == 0)
                    foreach (var k in regularKeys) _simulator.KeyPress(k);
                else
                    _simulator.ModifiedKeyStroke(activeMods, regularKeys);
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
                    case "lhclick": _simulator.MouseLeftButtonDown();   break;
                    case "rhclick": _simulator.MouseRightButtonDown();  break;
                    default:
                        Console.WriteLine($"[warn] Unknown mouse action: '{alias}'"); break;
                }
            }
        }
    }
}
