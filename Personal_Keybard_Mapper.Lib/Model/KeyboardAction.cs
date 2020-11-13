using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WindowsInput;
using Personal_Keyboard_Mapper.Lib.Interfaces;
using WindowsInput.Native;
using Personal_Keyboard_Mapper.Lib.Enums;

namespace Personal_Keyboard_Mapper.Lib.Model
{
    public class KeyboardAction : IOutputAction
    {
        /// <inheritdoc />
        public bool isEmptyAction { get; set; } = false;

        public bool OnlyModKeyAction { get; set; } = false;


        public void SendKeyboardAction(IInputSimulator simulator, VirtualKeyCode key)
        {
            if (simulator == null)
            {
                throw new ArgumentNullException();
            }

            simulator.Keyboard.KeyPress(key);
        }

        public void SendKeyboardAction(IInputSimulator simulator, IEnumerable<VirtualKeyCode> modKeys, IEnumerable<VirtualKeyCode> signKeys)
        {
            if (simulator == null)
            {
                throw new ArgumentNullException();
            }

            if (modKeys.Any())
            { 
                if (signKeys.Count() > 1 && signKeys.LastOrDefault() == VirtualKeyCode.LEFT)
                {
                    var offset = signKeys.Count() - 1;
                    var signKeysWithModKeys = signKeys.Take(offset);
                    simulator.Keyboard.ModifiedKeyStroke(modKeys, signKeysWithModKeys);
                    simulator.Keyboard.KeyPress(signKeys.Last());
                }
                else
                {
                    if (signKeys.Any())
                    {
                        simulator.Keyboard.ModifiedKeyStroke(modKeys, signKeys);
                    }
                    else
                    {
                        OnlyModKeyAction = true;
                    }
                }
            }
            else
            {
                var modKeysToPressOnce = Globals.GetModKeysToPressOnce().ToList();
                var modKeysToUse = Globals.GetModKeysToHoldDown().ToList();
                modKeysToUse.AddRange(modKeysToPressOnce);
                if (modKeysToUse.Any())
                {
                    simulator.Keyboard.ModifiedKeyStroke(modKeysToUse, signKeys);
                }
                else
                {
                    simulator.Keyboard.KeyPress(signKeys.ToArray());
                }
            }
        }

        public Task SendKeyboardActionAsync(IInputSimulator simulator, IEnumerable<VirtualKeyCode> modKeys, IEnumerable<VirtualKeyCode> signKeys)
        {
            throw new NotImplementedException();
        }

        public void SendMouseAction(IInputSimulator simulator,
            IEnumerable<VirtualKeyCode> modKeys,
            IEnumerable<VirtualKeyCode> mouseKeys)
        {
            throw new NotImplementedException();
        }

        public Task SendMouseActionAsync(IInputSimulator simulator,
            IEnumerable<VirtualKeyCode> modKeys,
            IEnumerable<VirtualKeyCode> mouseKeys)
        {
            throw new NotImplementedException();
        }
    }
}