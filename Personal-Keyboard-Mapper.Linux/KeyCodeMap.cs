using System.Collections.Generic;
using Personal_Keyboard_Mapper.Core.Enums;

namespace Personal_Keyboard_Mapper.Linux
{
    /// <summary>
    /// Translates between Linux evdev key codes and the platform-neutral
    /// VirtualKeyCode enum (Win32 VK values) used by the core library.
    /// </summary>
    public static class KeyCodeMap
    {
        // evdev code -> VirtualKeyCode
        private static readonly Dictionary<ushort, VirtualKeyCode> EvdevToVk = new()
        {
            [1] = VirtualKeyCode.ESCAPE,
            [2] = VirtualKeyCode.VK_1,
            [3] = VirtualKeyCode.VK_2,
            [4] = VirtualKeyCode.VK_3,
            [5] = VirtualKeyCode.VK_4,
            [6] = VirtualKeyCode.VK_5,
            [7] = VirtualKeyCode.VK_6,
            [8] = VirtualKeyCode.VK_7,
            [9] = VirtualKeyCode.VK_8,
            [10] = VirtualKeyCode.VK_9,
            [11] = VirtualKeyCode.VK_0,
            [12] = VirtualKeyCode.OEM_MINUS,
            [13] = VirtualKeyCode.OEM_PLUS,
            [14] = VirtualKeyCode.BACK,
            [15] = VirtualKeyCode.TAB,
            [16] = VirtualKeyCode.VK_Q,
            [17] = VirtualKeyCode.VK_W,
            [18] = VirtualKeyCode.VK_E,
            [19] = VirtualKeyCode.VK_R,
            [20] = VirtualKeyCode.VK_T,
            [21] = VirtualKeyCode.VK_Y,
            [22] = VirtualKeyCode.VK_U,
            [23] = VirtualKeyCode.VK_I,
            [24] = VirtualKeyCode.VK_O,
            [25] = VirtualKeyCode.VK_P,
            [26] = VirtualKeyCode.OEM_4,
            [27] = VirtualKeyCode.OEM_6,
            [28] = VirtualKeyCode.RETURN,
            [29] = VirtualKeyCode.LCONTROL,
            [30] = VirtualKeyCode.VK_A,
            [31] = VirtualKeyCode.VK_S,
            [32] = VirtualKeyCode.VK_D,
            [33] = VirtualKeyCode.VK_F,
            [34] = VirtualKeyCode.VK_G,
            [35] = VirtualKeyCode.VK_H,
            [36] = VirtualKeyCode.VK_J,
            [37] = VirtualKeyCode.VK_K,
            [38] = VirtualKeyCode.VK_L,
            [39] = VirtualKeyCode.OEM_1,
            [40] = VirtualKeyCode.OEM_7,
            [41] = VirtualKeyCode.OEM_3,
            [42] = VirtualKeyCode.LSHIFT,
            [43] = VirtualKeyCode.OEM_5,
            [44] = VirtualKeyCode.VK_Z,
            [45] = VirtualKeyCode.VK_X,
            [46] = VirtualKeyCode.VK_C,
            [47] = VirtualKeyCode.VK_V,
            [48] = VirtualKeyCode.VK_B,
            [49] = VirtualKeyCode.VK_N,
            [50] = VirtualKeyCode.VK_M,
            [51] = VirtualKeyCode.OEM_COMMA,
            [52] = VirtualKeyCode.OEM_PERIOD,
            [53] = VirtualKeyCode.OEM_2,
            [54] = VirtualKeyCode.RSHIFT,
            [55] = VirtualKeyCode.MULTIPLY,
            [56] = VirtualKeyCode.LMENU,
            [57] = VirtualKeyCode.SPACE,
            [58] = VirtualKeyCode.CAPITAL,
            [59] = VirtualKeyCode.F1,
            [60] = VirtualKeyCode.F2,
            [61] = VirtualKeyCode.F3,
            [62] = VirtualKeyCode.F4,
            [63] = VirtualKeyCode.F5,
            [64] = VirtualKeyCode.F6,
            [65] = VirtualKeyCode.F7,
            [66] = VirtualKeyCode.F8,
            [67] = VirtualKeyCode.F9,
            [68] = VirtualKeyCode.F10,
            [69] = VirtualKeyCode.NUMLOCK,
            [70] = VirtualKeyCode.SCROLL,
            [71] = VirtualKeyCode.NUMPAD7,
            [72] = VirtualKeyCode.NUMPAD8,
            [73] = VirtualKeyCode.NUMPAD9,
            [74] = VirtualKeyCode.SUBTRACT,
            [75] = VirtualKeyCode.NUMPAD4,
            [76] = VirtualKeyCode.NUMPAD5,
            [77] = VirtualKeyCode.NUMPAD6,
            [78] = VirtualKeyCode.ADD,
            [79] = VirtualKeyCode.NUMPAD1,
            [80] = VirtualKeyCode.NUMPAD2,
            [81] = VirtualKeyCode.NUMPAD3,
            [82] = VirtualKeyCode.NUMPAD0,
            [83] = VirtualKeyCode.DECIMAL,
            [87] = VirtualKeyCode.F11,
            [88] = VirtualKeyCode.F12,
            [96] = VirtualKeyCode.RETURN,   // keypad enter
            [97] = VirtualKeyCode.RCONTROL,
            [98] = VirtualKeyCode.DIVIDE,
            [99] = VirtualKeyCode.SNAPSHOT,
            [100] = VirtualKeyCode.RMENU,
            [102] = VirtualKeyCode.HOME,
            [103] = VirtualKeyCode.UP,
            [104] = VirtualKeyCode.PRIOR,
            [105] = VirtualKeyCode.LEFT,
            [106] = VirtualKeyCode.RIGHT,
            [107] = VirtualKeyCode.END,
            [108] = VirtualKeyCode.DOWN,
            [109] = VirtualKeyCode.NEXT,
            [110] = VirtualKeyCode.INSERT,
            [111] = VirtualKeyCode.DELETE,
            [119] = VirtualKeyCode.PAUSE,
            [125] = VirtualKeyCode.LWIN,
            [126] = VirtualKeyCode.RWIN,
            [127] = VirtualKeyCode.APPS,
        };

        // VirtualKeyCode -> evdev code (built from the table above plus
        // generic modifiers, which map to their left-hand variants)
        private static readonly Dictionary<VirtualKeyCode, ushort> VkToEvdev = BuildReverseMap();

        private static Dictionary<VirtualKeyCode, ushort> BuildReverseMap()
        {
            var map = new Dictionary<VirtualKeyCode, ushort>();
            foreach (var pair in EvdevToVk)
            {
                if (!map.ContainsKey(pair.Value))
                {
                    map[pair.Value] = pair.Key;
                }
            }
            map[VirtualKeyCode.SHIFT] = 42;    // KEY_LEFTSHIFT
            map[VirtualKeyCode.CONTROL] = 29;  // KEY_LEFTCTRL
            map[VirtualKeyCode.MENU] = 56;     // KEY_LEFTALT
            return map;
        }

        public static bool TryGetVirtualKey(ushort evdevCode, out VirtualKeyCode vk)
            => EvdevToVk.TryGetValue(evdevCode, out vk);

        public static bool TryGetEvdevCode(VirtualKeyCode vk, out ushort evdevCode)
            => VkToEvdev.TryGetValue(vk, out evdevCode);
    }
}
