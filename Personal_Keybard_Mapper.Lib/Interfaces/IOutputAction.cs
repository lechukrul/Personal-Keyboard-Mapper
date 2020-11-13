using System.Collections.Generic;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;
using Personal_Keyboard_Mapper.Lib.Enums;

namespace Personal_Keyboard_Mapper.Lib.Interfaces
{
    /// <summary>
    /// Implements a methods of an action related with keys combination.
    /// </summary>
    public interface IOutputAction
    {
        bool isEmptyAction { get; set; } 
        bool OnlyModKeyAction { get; set; }

        /// <summary>
        /// Sends the keyboard action asynchronous.
        /// </summary>
        /// <param name="simulator">The simulator.</param>
        /// <param name="modKeys">The mod keys.</param>
        /// <param name="signKeys">The sign keys.</param>
        /// <returns></returns>
        Task SendKeyboardActionAsync(IInputSimulator simulator, IEnumerable<VirtualKeyCode> modKeys, IEnumerable<VirtualKeyCode> signKeys);

        /// <summary>
        /// Sends the keyboard action.
        /// </summary>
        /// <param name="simulator">The simulator.</param>
        /// <param name="modKeys">The mod keys.</param>
        /// <param name="signKeys">The sign keys.</param>
        void SendKeyboardAction(IInputSimulator simulator, IEnumerable<VirtualKeyCode> modKeys, IEnumerable<VirtualKeyCode> signKeys);

        /// <summary>
        /// Sends the mouse action asynchronous.
        /// </summary>
        /// <param name="simulator">The simulator.</param>
        /// <param name="modKeys"></param>
        /// <param name="mouseKeys"></param>
        /// <returns></returns>
        Task SendMouseActionAsync(IInputSimulator simulator,
            IEnumerable<VirtualKeyCode> modKeys, IEnumerable<VirtualKeyCode> mouseKeys);

        /// <summary>
        /// Sends the mouse action.
        /// </summary>
        /// <param name="simulator">The simulator.</param>
        /// <param name="modKeys"></param>
        /// <param name="mouseKeys"></param>
        void SendMouseAction(IInputSimulator simulator,
            IEnumerable<VirtualKeyCode> modKeys, IEnumerable<VirtualKeyCode> mouseKeys);

    }
}