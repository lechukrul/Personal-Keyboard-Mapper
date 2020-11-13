using WindowsInput.Native;

namespace Personal_Keyboard_Mapper.Lib.Extensions
{
    /// <summary>
    /// Extensions for <see cref="VirtualKeyCode"/>
    /// </summary>
    public static class VirtualKeyCodeExtensions
    {
        /// <summary>
        /// Converts the numeric key code to numeric keypad key code.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static VirtualKeyCode ConvertNumericKeyCodeToNumericKeypadKeyCode(this VirtualKeyCode key)
        {
            switch (key)
            {
                case VirtualKeyCode.VK_0:
                    return VirtualKeyCode.NUMPAD0;
                case VirtualKeyCode.VK_1:
                    return VirtualKeyCode.NUMPAD1;
                case VirtualKeyCode.VK_2:
                    return VirtualKeyCode.NUMPAD2;
                case VirtualKeyCode.VK_3:
                    return VirtualKeyCode.NUMPAD3;
                case VirtualKeyCode.VK_4:
                    return VirtualKeyCode.NUMPAD4;
                case VirtualKeyCode.VK_5:
                    return VirtualKeyCode.NUMPAD5;
                case VirtualKeyCode.VK_6:
                    return VirtualKeyCode.NUMPAD6;
                case VirtualKeyCode.VK_7:
                    return VirtualKeyCode.NUMPAD7;
                case VirtualKeyCode.VK_8:
                    return VirtualKeyCode.NUMPAD8;
                case VirtualKeyCode.VK_9:
                    return VirtualKeyCode.NUMPAD9;
                default:
                    return key;
            }
        }
    }
}