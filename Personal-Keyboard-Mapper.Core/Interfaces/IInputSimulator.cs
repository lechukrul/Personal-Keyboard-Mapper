using System.Collections.Generic;
using Personal_Keyboard_Mapper.Core.Enums;

namespace Personal_Keyboard_Mapper.Core.Interfaces
{
    /// <summary>
    /// Sends synthetic keyboard and mouse input to the system.
    /// Windows: WindowsInput.InputSimulator. Linux: uinput virtual device.
    /// </summary>
    public interface IInputSimulator
    {
        void KeyDown(VirtualKeyCode key);

        void KeyUp(VirtualKeyCode key);

        void KeyPress(VirtualKeyCode key);

        /// <summary>
        /// Holds down the modifier keys, presses the keys, releases the modifiers.
        /// </summary>
        void ModifiedKeyStroke(IEnumerable<VirtualKeyCode> modKeys, IEnumerable<VirtualKeyCode> keys);

        /// <summary>
        /// Types a unicode string (used by word prediction to insert a suffix).
        /// </summary>
        void TextEntry(string text);

        bool IsKeyDown(VirtualKeyCode key);

        void MouseLeftClick();

        void MouseLeftDoubleClick();

        void MouseRightClick();

        void MouseRightDoubleClick();

        void MouseLeftButtonDown();

        void MouseLeftButtonUp();

        void MouseMoveBy(int dx, int dy);
    }
}
