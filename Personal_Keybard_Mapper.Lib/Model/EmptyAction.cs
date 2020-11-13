using System.Collections.Generic;
using Personal_Keyboard_Mapper.Lib.Enums;
using Personal_Keyboard_Mapper.Lib.Interfaces;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;

namespace Personal_Keyboard_Mapper.Lib.Model
{
    public class EmptyAction : IOutputAction
    {
        public bool isEmptyAction { get; set; } = true;
        public bool OnlyModKeyAction { get; set; } = false;

        public Task SendKeyboardActionAsync(IInputSimulator simulator, IEnumerable<VirtualKeyCode> modKeys, IEnumerable<VirtualKeyCode> signKeys)
        {
            throw new System.NotImplementedException();
        }

        public void SendKeyboardAction(IInputSimulator simulator, IEnumerable<VirtualKeyCode> modKeys, IEnumerable<VirtualKeyCode> signKeys)
        {
            throw new System.NotImplementedException();
        }

        public Task SendMouseActionAsync(IInputSimulator simulator,
            IEnumerable<VirtualKeyCode> modKeys,
            IEnumerable<VirtualKeyCode> mouseKeys)
        {
            throw new System.NotImplementedException();
        }

        public void SendMouseAction(IInputSimulator simulator,
            IEnumerable<VirtualKeyCode> modKeys,
            IEnumerable<VirtualKeyCode> mouseKeys)
        {
            throw new System.NotImplementedException();
        }
    }
}