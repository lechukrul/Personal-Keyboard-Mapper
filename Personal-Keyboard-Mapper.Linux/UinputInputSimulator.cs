using System;
using System.Collections.Generic;
using System.Linq;
using Personal_Keyboard_Mapper.Core.Enums;
using Personal_Keyboard_Mapper.Core.Interfaces;
using Personal_Keyboard_Mapper.Linux.Native;

namespace Personal_Keyboard_Mapper.Linux
{
    /// <summary>
    /// Sends synthetic input through the uinput virtual device.
    /// Linux counterpart of WindowsInput.InputSimulator.
    /// </summary>
    public sealed class UinputInputSimulator : IInputSimulator
    {
        private readonly UinputDevice device;
        private readonly HashSet<VirtualKeyCode> keysDown = new();

        // Polish programmer layout: diacritics typed as AltGr + base letter
        private static readonly Dictionary<char, char> PolishBaseLetters = new()
        {
            ['ą'] = 'a', ['ć'] = 'c', ['ę'] = 'e', ['ł'] = 'l', ['ń'] = 'n',
            ['ó'] = 'o', ['ś'] = 's', ['ź'] = 'x', ['ż'] = 'z',
            ['Ą'] = 'A', ['Ć'] = 'C', ['Ę'] = 'E', ['Ł'] = 'L', ['Ń'] = 'N',
            ['Ó'] = 'O', ['Ś'] = 'S', ['Ź'] = 'X', ['Ż'] = 'Z',
        };

        public UinputInputSimulator(UinputDevice uinputDevice)
        {
            device = uinputDevice;
        }

        public void KeyDown(VirtualKeyCode key)
        {
            if (KeyCodeMap.TryGetEvdevCode(key, out var code))
            {
                device.EmitKey(code, Libc.KEY_PRESS);
                keysDown.Add(key);
            }
        }

        public void KeyUp(VirtualKeyCode key)
        {
            if (KeyCodeMap.TryGetEvdevCode(key, out var code))
            {
                device.EmitKey(code, Libc.KEY_RELEASE);
                keysDown.Remove(key);
            }
        }

        public void KeyPress(VirtualKeyCode key)
        {
            if (KeyCodeMap.TryGetEvdevCode(key, out var code))
            {
                device.KeyPress(code);
            }
        }

        public void ModifiedKeyStroke(IEnumerable<VirtualKeyCode> modKeys, IEnumerable<VirtualKeyCode> keys)
        {
            var mods = modKeys?.ToList() ?? new List<VirtualKeyCode>();
            foreach (var mod in mods)
            {
                KeyDown(mod);
            }
            foreach (var key in keys ?? Enumerable.Empty<VirtualKeyCode>())
            {
                KeyPress(key);
            }
            foreach (var mod in Enumerable.Reverse(mods))
            {
                KeyUp(mod);
            }
        }

        public void TextEntry(string text)
        {
            foreach (var ch in text)
            {
                TypeChar(ch);
            }
        }

        private void TypeChar(char ch)
        {
            if (PolishBaseLetters.TryGetValue(ch, out var baseLetter))
            {
                var mods = new List<VirtualKeyCode> { VirtualKeyCode.RMENU };
                if (char.IsUpper(baseLetter))
                {
                    mods.Add(VirtualKeyCode.SHIFT);
                }
                ModifiedKeyStroke(mods, new[] { LetterToVk(char.ToLowerInvariant(baseLetter)) });
                return;
            }
            if (ch >= 'a' && ch <= 'z')
            {
                KeyPress(LetterToVk(ch));
                return;
            }
            if (ch >= 'A' && ch <= 'Z')
            {
                ModifiedKeyStroke(new[] { VirtualKeyCode.SHIFT },
                    new[] { LetterToVk(char.ToLowerInvariant(ch)) });
                return;
            }
            if (ch >= '0' && ch <= '9')
            {
                KeyPress(VirtualKeyCode.VK_0 + (ch - '0'));
                return;
            }
            switch (ch)
            {
                case ' ': KeyPress(VirtualKeyCode.SPACE); break;
                case '.': KeyPress(VirtualKeyCode.OEM_PERIOD); break;
                case ',': KeyPress(VirtualKeyCode.OEM_COMMA); break;
                case '-': KeyPress(VirtualKeyCode.OEM_MINUS); break;
                case '\n': KeyPress(VirtualKeyCode.RETURN); break;
                case '\t': KeyPress(VirtualKeyCode.TAB); break;
                // TODO: full punctuation coverage (layout-dependent)
            }
        }

        private static VirtualKeyCode LetterToVk(char lowercaseLetter)
            => VirtualKeyCode.VK_A + (lowercaseLetter - 'a');

        public bool IsKeyDown(VirtualKeyCode key) => keysDown.Contains(key);

        public void MouseLeftClick() => ClickButton(Libc.BTN_LEFT);

        public void MouseLeftDoubleClick()
        {
            ClickButton(Libc.BTN_LEFT);
            ClickButton(Libc.BTN_LEFT);
        }

        public void MouseRightClick() => ClickButton(Libc.BTN_RIGHT);

        public void MouseRightDoubleClick()
        {
            ClickButton(Libc.BTN_RIGHT);
            ClickButton(Libc.BTN_RIGHT);
        }

        public void MouseLeftButtonDown() => device.EmitKey(Libc.BTN_LEFT, Libc.KEY_PRESS);

        public void MouseLeftButtonUp() => device.EmitKey(Libc.BTN_LEFT, Libc.KEY_RELEASE);

        public void MouseMoveBy(int dx, int dy)
        {
            device.Emit(Libc.EV_REL, Libc.REL_X, dx);
            device.Emit(Libc.EV_REL, Libc.REL_Y, dy);
            device.Sync();
        }

        private void ClickButton(ushort button)
        {
            device.EmitKey(button, Libc.KEY_PRESS);
            device.EmitKey(button, Libc.KEY_RELEASE);
        }
    }
}
