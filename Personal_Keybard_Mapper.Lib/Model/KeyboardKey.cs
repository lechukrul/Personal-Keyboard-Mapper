using System;
using WindowsInput.Native;
using Personal_Keyboard_Mapper.Lib.Enums;
using Personal_Keyboard_Mapper.Lib.Interfaces;

namespace Personal_Keyboard_Mapper.Lib.Model
{
    /// <summary>
    /// Represents a single pressed keyboard key
    /// </summary>
    /// <seealso cref="IKeyboardKey" />
    public class KeyboardKey : IKeyboardKey
    {
        public KeyboardKey(VirtualKeyCode virtualKeyCode, KeyCombinationPosition position)
        {
            KeyCode = virtualKeyCode;   
            CombinationPosition = position;
        }
        /// <inheritdoc />
        public VirtualKeyCode KeyCode { get; set; }

        /// <inheritdoc />
        public KeyCombinationPosition CombinationPosition { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return KeyCode.ToString();
        }

        public object Clone()
        {
            return new KeyboardKey(KeyCode, CombinationPosition);
        }
         
    }
}