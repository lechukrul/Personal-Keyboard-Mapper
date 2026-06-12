using System;
using Personal_Keyboard_Mapper.Core.Model;

namespace Personal_Keyboard_Mapper.Core.Interfaces
{
    /// <summary>
    /// A global keyboard hook that sees every key event in the system
    /// and can stop it from reaching other applications.
    /// Windows: WH_KEYBOARD_LL. Linux: evdev with EVIOCGRAB + uinput passthrough.
    /// </summary>
    public interface IKeyboardHook : IDisposable
    {
        /// <summary>
        /// Called for every key event. Return true to suppress the event
        /// (it never reaches the rest of the system), false to let it through.
        /// </summary>
        Func<KeyEvent, bool> KeyHandler { get; set; }

        void StartHook();

        void StopHook();
    }
}
