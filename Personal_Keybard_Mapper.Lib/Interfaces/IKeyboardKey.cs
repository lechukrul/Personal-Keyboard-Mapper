using System;
using System.Collections;
using WindowsInput.Native;
using Personal_Keyboard_Mapper.Lib.Enums;

namespace Personal_Keyboard_Mapper.Lib.Interfaces
{
    public interface IKeyboardKey : ICloneable
    {
        /// <summary>
        /// Gets or sets the key code.
        /// </summary>
        /// <value>
        /// The key code.
        /// </value>
        public VirtualKeyCode KeyCode { get; set; }

        /// <summary>
        /// Gets or sets the combination position.
        /// </summary>
        /// <value>
        /// The combination position.
        /// </value>
        public KeyCombinationPosition CombinationPosition { get; set; }
    }
}