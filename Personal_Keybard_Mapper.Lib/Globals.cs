using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Resources;
using WindowsInput;
using WindowsInput.Native;
using Personal_Keyboard_Mapper.Lib.Interfaces;
using Action = Personal_Keyboard_Mapper.Lib.Model.Action;

namespace Personal_Keyboard_Mapper.Lib
{
    public static class Globals
    {
        public static int KeyCombinationPositionCounter { get; set; } = -1;
        public static int ShiftPressCounter { get; set; }
        public static int CtrlPressCounter { get; set; }
        public static int RightAltPressCounter { get; set; }
        public static int LeftAltPressCounter { get; set; }
        public static int RightWinPressCounter { get; set; }
        public static int LeftWinPressCounter { get; set; }

        public static bool IsSoundOn { get; set; }
        public static bool IsHelpWindowOn { get; set; }

        public static bool IsShiftPressedOnce { get; set; }

        public static bool IsShiftHoldDown { get; set; }

        public static bool IsCtrlPressedOnce { get; set; }

        public static bool IsCtrlHoldDown { get; set; }

        public static bool IsLeftAltPressedOnce { get; set; }

        public static bool IsLeftAltHoldDown { get; set; }

        public static bool IsRightAltPressedOnce { get; set; }

        public static bool IsRightAltHoldDown { get; set; }

        public static bool IsLeftWinPressedOnce { get; set; }

        public static bool IsLeftWinHoldDown { get; set; }

        public static bool IsRightWinPressedOnce { get; set; }

        public static bool IsRightWinHoldDown { get; set; }
        public static bool IsRightMouseButtonHoldDown { get; set; }
        public static bool IsLeftMouseButtonHoldDown { get; set; }
        public static bool IsMouseButtonWithModKey { get; set; }

        public static string FirstKeyInCombination { get; set; }

        public static ResXResourceSet AliasResources { get; set; }

        public static ResXResourceSet GlobalResources { get; set; }

        /// <summary>
        /// Gets the numeric keypad virtual key codes.
        /// </summary>
        /// <value>
        /// The numeric keypad virtual key codes.
        /// </value>
        public static IEnumerable<VirtualKeyCode> NumericKeypadVirtualKeyCodes { get; } = new List<VirtualKeyCode>()
        {
            VirtualKeyCode.NUMPAD0,
            VirtualKeyCode.NUMPAD1,
            VirtualKeyCode.NUMPAD2,
            VirtualKeyCode.NUMPAD3,
            VirtualKeyCode.NUMPAD4,
            VirtualKeyCode.NUMPAD5,
            VirtualKeyCode.NUMPAD6,
            VirtualKeyCode.NUMPAD7,
            VirtualKeyCode.NUMPAD8, 
            VirtualKeyCode.NUMPAD9
        };



        public static IEnumerable<VirtualKeyCode> NumericKeypadWithShiftVirtualKeyCodes { get; } = new List<VirtualKeyCode>()
        {
            VirtualKeyCode.INSERT,
            VirtualKeyCode.END,
            VirtualKeyCode.DOWN,
            VirtualKeyCode.NEXT,
            VirtualKeyCode.LEFT,
            VirtualKeyCode.RIGHT,
            VirtualKeyCode.HOME,
            VirtualKeyCode.UP,
            VirtualKeyCode.PRIOR
        };

        public static IEnumerable<VirtualKeyCode> ModKeysVirtualKeyCodes { get; } = new List<VirtualKeyCode>()
        {
            VirtualKeyCode.LSHIFT,
            VirtualKeyCode.RSHIFT,
            VirtualKeyCode.SHIFT,
            VirtualKeyCode.RCONTROL, 
            VirtualKeyCode.LCONTROL, 
            VirtualKeyCode.CONTROL,
            VirtualKeyCode.RMENU, 
            VirtualKeyCode.LMENU
        };

        /// <summary>
        /// Gets the numeric keys virtual key codes.
        /// </summary>
        /// <value>
        /// The numeric keys virtual key codes.
        /// </value>
        public static IEnumerable<VirtualKeyCode> NumericVirtualKeyCodes { get; } = new List<VirtualKeyCode>()
        {
            VirtualKeyCode.VK_0,
            VirtualKeyCode.VK_1,
            VirtualKeyCode.VK_2,
            VirtualKeyCode.VK_3,
            VirtualKeyCode.VK_4,
            VirtualKeyCode.VK_5,
            VirtualKeyCode.VK_6,
            VirtualKeyCode.VK_7,
            VirtualKeyCode.VK_8,
            VirtualKeyCode.VK_9
        };

        public static IEnumerable<string> OpeningBraces { get; } = new List<string>
        {
            "(",
            "{",
            "[",
            "<"
        };

        public static IEnumerable<string> Braces { get; } = new List<string>
        {
            "()",
            "{}",
            "[]",
            "<>"
        };

        public static IEnumerable<string> ActionsWithLeftArrow { get; } = new List<string>
        {
            "()",
            "{}",
            "[]",
            "<>",
            "''",
            @""""""
        };

        public static IEnumerable<string> DotOrSemiColon { get; } = new List<string>
        {
            ".",
            ";"
        };

        public static IEnumerable<string> PolishSigns { get; } = new List<string>
        {
            "ą",
            "Ą",
            "ć",
            "Ć",
            "ę",
            "Ę",
            "ł",
            "Ł",
            "ó",
            "Ó",
            "ś",
            "Ś",
            "ż",
            "Ż",
            "ź",
            "Ź"
        };

        public static List<(string sign, VirtualKeyCode[] keys)> BracketsKeyCodes { get; } = new List<(string sign, VirtualKeyCode[] keys)>()
        {
            ("(", new[] {VirtualKeyCode.SHIFT, VirtualKeyCode.VK_9}),
            (")", new[] {VirtualKeyCode.SHIFT, VirtualKeyCode.VK_0}),
            ("[", new[] {VirtualKeyCode.OEM_4}),
            ("]", new[] {VirtualKeyCode.OEM_6}),
            ("{", new[] {VirtualKeyCode.SHIFT, VirtualKeyCode.OEM_4}),
            ("}", new[] {VirtualKeyCode.SHIFT, VirtualKeyCode.OEM_6}),
            ("<", new[] {VirtualKeyCode.SHIFT, VirtualKeyCode.OEM_COMMA}),
            (">", new[] {VirtualKeyCode.SHIFT, VirtualKeyCode.OEM_PERIOD}),
        };

        public static IEnumerable<(string sign, VirtualKeyCode[] keys)> OpenCloseBrackets
        {
            get { return _OpenCloseBracketsList(); }
        }

        private static IEnumerable<(string sign, VirtualKeyCode[] keys)> _OpenCloseBracketsList()
        {
            var roundBracketsKeysCodes = BracketsKeyCodes[0].keys.ToList();
            roundBracketsKeysCodes.AddRange(BracketsKeyCodes[1].keys);
            var squareBracesKeysCodes = BracketsKeyCodes[2].keys.ToList();
            squareBracesKeysCodes.AddRange(BracketsKeyCodes[3].keys);
            var curlyBracketsKeysCodes = BracketsKeyCodes[4].keys.ToList();
            curlyBracketsKeysCodes.AddRange(BracketsKeyCodes[5].keys);
            var angleBracesKeysCodes = BracketsKeyCodes[6].keys.ToList();
            angleBracesKeysCodes.AddRange(BracketsKeyCodes[7].keys);
            return new List<(string sign, VirtualKeyCode[] keys)>()
            {
                ("()", roundBracketsKeysCodes.ToArray()),
                ("{}", curlyBracketsKeysCodes.ToArray()),
                ("[]", squareBracesKeysCodes.ToArray()),
                ("<>", angleBracesKeysCodes.ToArray()),
            };
        }

        /// <summary>
        /// Gets the mod keys to press once.
        /// </summary>
        /// <returns></returns>
        public static IList<VirtualKeyCode> GetModKeysToPressOnce()
        {
            var result = new List<VirtualKeyCode>();
            if (IsShiftPressedOnce)
            {
                result.Add(VirtualKeyCode.SHIFT);
            }

            if (IsCtrlPressedOnce)
            {
                result.Add(VirtualKeyCode.CONTROL);
            }

            if (IsRightAltPressedOnce)
            {
                result.Add(VirtualKeyCode.RMENU);
            }

            if (IsLeftAltPressedOnce)
            {
                result.Add(VirtualKeyCode.LMENU);
            }

            if (IsRightWinPressedOnce)
            {
                result.Add(VirtualKeyCode.RWIN);
            }

            if (IsLeftWinHoldDown)
            {
                result.Add(VirtualKeyCode.LWIN);
            }

            return result;
        }

        /// <summary>
        /// Gets the mod keys to hold down.
        /// </summary>
        /// <returns></returns>
        public static IList<VirtualKeyCode> GetModKeysToHoldDown()
        {
            var result = new List<VirtualKeyCode>();
            if (IsShiftHoldDown)
            {
                result.Add(VirtualKeyCode.SHIFT);
            }

            if (IsCtrlHoldDown)
            {
                result.Add(VirtualKeyCode.CONTROL);
            }

            if (IsRightAltHoldDown)
            {
                result.Add(VirtualKeyCode.RMENU);
            }

            if (IsLeftAltHoldDown)
            {
                result.Add(VirtualKeyCode.LMENU);
            }

            if (IsRightWinHoldDown)
            {
                result.Add(VirtualKeyCode.RWIN);
            }

            if (IsLeftWinHoldDown)
            {
                result.Add(VirtualKeyCode.LWIN);
            }

            return result;
        }

        public static void ReleaseModKey(VirtualKeyCode modKey)
        {
            switch (modKey)
            {
                case VirtualKeyCode.SHIFT:
                    IsShiftPressedOnce = false;
                    break;

                case VirtualKeyCode.CONTROL:
                    IsCtrlPressedOnce = false;
                    break;

                case VirtualKeyCode.RMENU:
                    IsRightAltPressedOnce = false;
                    break;

                case VirtualKeyCode.LMENU:
                    IsLeftAltPressedOnce = false;
                    break;

                case VirtualKeyCode.RWIN:
                    IsRightWinPressedOnce = false;
                    break;

                case VirtualKeyCode.LWIN:
                    IsLeftWinPressedOnce = false;
                    break;
            }
        }

        public static void ResetKeysFlags()
        {
            IsShiftPressedOnce = false;
            IsCtrlPressedOnce = false;
            IsLeftAltPressedOnce = false;
            IsRightAltPressedOnce = false;
            IsRightWinPressedOnce = false;
            IsLeftWinPressedOnce = false;
            IsShiftHoldDown = false;
            IsCtrlHoldDown = false;
            IsRightAltHoldDown = false;
            IsLeftAltHoldDown = false;
            IsRightWinHoldDown = false;
            IsLeftWinHoldDown = false;
        }
    }
}