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

        private void ExecuteAction(ActionEntry action)
        {
            if (string.Equals(action.Type, "Mouse", StringComparison.OrdinalIgnoreCase))
            {
                ExecuteMouseAction(action);
                return;
            }

            var resolved = new List<VirtualKeyCode>();
            foreach (var alias in action.OutputVirtualKeys ?? Enumerable.Empty<string>())
            {
                if (KeyAliasResolver.TryResolve(alias, out var vk))
                    resolved.Add(vk);
                else
                    _simulator.TextEntry(alias);
            }

            if (resolved.Count == 0) return;

            var modKeys     = resolved.Where(KeyAliasResolver.IsModifier).ToList();
            var regularKeys = resolved.Where(k => !KeyAliasResolver.IsModifier(k)).ToList();

            if (regularKeys.Count == 0)
            {
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
                    case "lhclick": _simulator.MouseLeftButtonDown();   break;
                    case "rhclick": _simulator.MouseRightButtonDown();  break;
                    default:
                        Console.WriteLine($"[warn] Unknown mouse action: '{alias}'"); break;
                }
            }
        }
    }
}
