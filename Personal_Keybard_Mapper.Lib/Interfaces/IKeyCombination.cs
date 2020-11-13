using System;
using WindowsInput.Native;
using log4net;
using Personal_Keyboard_Mapper.Lib.Enums;
using Action = Personal_Keyboard_Mapper.Lib.Model.Action;

namespace Personal_Keyboard_Mapper.Lib.Interfaces
{
    public interface IKeyCombination: ICloneable
    {
        ILog logger { get; set; }

        /// <summary>
        /// Gets or sets the keys in combination.
        /// </summary>
        /// <value>
        /// The keys.
        /// </value>
        public IKeyboardKey[] Keys { get; set; }

        public Action Action { get; set; }

        /// <summary>
        /// Sets the key by position.
        /// </summary>
        /// <param name="position">The key combination position.</param>
        /// <param name="keyCode">The key code.</param>
        void SetKeyByPosition(KeyCombinationPosition position, VirtualKeyCode keyCode);

        /// <summary>
        /// Clears this instance.
        /// </summary>
        void Clear();

        /// <summary>
        /// Sets the logger.
        /// </summary>
        /// <param name="log">The logger.</param>
        void SetLogger(ILog log);
    }
}