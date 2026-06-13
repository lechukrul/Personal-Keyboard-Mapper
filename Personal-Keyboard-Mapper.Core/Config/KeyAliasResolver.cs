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

        public static bool TryResolve(string alias, out VirtualKeyCode key)
        {
            key = default;

            switch (alias.ToLowerInvariant())
            {
                case "ctrl":
                case "crtl":
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
                case "end":
                    key = VirtualKeyCode.END; return true;
                case "home":
                    key = VirtualKeyCode.HOME; return true;
                case "ins":
                case "insert":
                    key = VirtualKeyCode.INSERT; return true;
                case "pgup":
                case "pageup":
                    key = VirtualKeyCode.PRIOR; return true;
                case "pgdn":
                case "pagedown":
                    key = VirtualKeyCode.NEXT; return true;
                case "backslash":
                    key = VirtualKeyCode.OEM_5; return true;
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

            return false;
        }

        public static bool IsModifier(VirtualKeyCode key)
            => Array.IndexOf(ModifierCodes, key) >= 0;
    }
}
