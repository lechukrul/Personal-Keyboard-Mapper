using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;
using Personal_Keyboard_Mapper.Lib.Enums;
using Personal_Keyboard_Mapper.Lib.Interfaces;

namespace Personal_Keyboard_Mapper.Lib.Model
{
    public class MouseAction : IOutputAction
    {
        /// <inheritdoc />
        public bool isEmptyAction { get; set; } = false;

        public bool OnlyModKeyAction { get; set; } = false;


        /// <inheritdoc />
        public async void SendMouseAction(IInputSimulator simulator,
            IEnumerable<VirtualKeyCode> modKeys, IEnumerable<VirtualKeyCode> mouseKeys)
        {
            if (simulator == null)
            {
                throw new ArgumentNullException(nameof(simulator)); 
            }

            try
            { 
                if (mouseKeys.Any())
                {
                    var modKeysToPressOnce = Globals.GetModKeysToPressOnce().ToList();
                    var modKeysToUse = Globals.GetModKeysToHoldDown().ToList();
                    modKeysToUse.AddRange(modKeysToPressOnce);
                    switch (mouseKeys.First())
                    {
                        case VirtualKeyCode.LBUTTON:
                            if (mouseKeys.Count() == 1)
                            {
                                foreach (var key in modKeysToUse)
                                {
                                    if (simulator.InputDeviceState.IsKeyUp(key))
                                    {
                                        simulator.Keyboard.KeyDown(key);
                                    }
                                }

                                if (simulator.InputDeviceState.IsKeyUp(VirtualKeyCode.LBUTTON))
                                {
                                    simulator.Mouse.LeftButtonClick();
                                }
                                foreach (var key in modKeysToUse)
                                {
                                    if (simulator.InputDeviceState.IsKeyDown(key))
                                    {
                                        simulator.Keyboard.KeyUp(key);
                                    }
                                }
                            }
                            else if (mouseKeys.Count() == 2)
                            {
                                foreach (var key in modKeysToUse)
                                {
                                    if (simulator.InputDeviceState.IsKeyUp(key))
                                    {
                                        simulator.Keyboard.KeyDown(key);
                                    }
                                }

                                if (simulator.InputDeviceState.IsKeyUp(VirtualKeyCode.LBUTTON))
                                {
                                    simulator.Mouse.LeftButtonDoubleClick();
                                }
                                foreach (var key in modKeysToUse)
                                {
                                    if (simulator.InputDeviceState.IsKeyDown(key))
                                    {
                                        simulator.Keyboard.KeyUp(key);
                                    }
                                }
                            }
                            else
                            {
                                if (!Globals.IsLeftMouseButtonHoldDown)
                                {
                                    Globals.IsLeftMouseButtonHoldDown = true;
                                    simulator.Mouse.LeftButtonDown();
                                }
                                else
                                {
                                    Globals.IsLeftMouseButtonHoldDown = false;
                                    simulator.Mouse.LeftButtonUp();
                                }
                            }
                            break;

                        case VirtualKeyCode.RBUTTON:
                            if (mouseKeys.Count() == 1)
                            {
                                foreach (var key in modKeysToUse)
                                {
                                    if (simulator.InputDeviceState.IsKeyUp(key))
                                    {
                                        simulator.Keyboard.KeyDown(key);
                                    }
                                }

                                if (simulator.InputDeviceState.IsKeyUp(VirtualKeyCode.RBUTTON))
                                {
                                    simulator.Mouse.RightButtonClick();
                                }
                                foreach (var key in modKeysToUse)
                                {
                                    if (simulator.InputDeviceState.IsKeyDown(key))
                                    {
                                        simulator.Keyboard.KeyUp(key);
                                    }
                                }
                            }
                            else if (mouseKeys.Count() == 2)
                            {
                                if (simulator.InputDeviceState.IsKeyUp(VirtualKeyCode.RBUTTON))
                                {
                                    simulator.Mouse.RightButtonDoubleClick();
                                }
                            }
                            else
                            {
                                if (!Globals.IsRightMouseButtonHoldDown)
                                {
                                    Globals.IsRightMouseButtonHoldDown = true;
                                    simulator.Mouse.RightButtonDown();
                                }
                                else
                                {
                                    Globals.IsRightMouseButtonHoldDown = false;
                                    simulator.Mouse.RightButtonUp();
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.StackTrace);
            }
        }

        public Task SendMouseActionAsync(IInputSimulator simulator,
            IEnumerable<VirtualKeyCode> modKeys,
            IEnumerable<VirtualKeyCode> mouseKeys)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task SendKeyboardActionAsync(IInputSimulator simulator, IEnumerable<VirtualKeyCode> modKeys, IEnumerable<VirtualKeyCode> signKeys)
        {
            throw new NotImplementedException();
        }

        public void SendKeyboardAction(IInputSimulator simulator, IEnumerable<VirtualKeyCode> modKeys, IEnumerable<VirtualKeyCode> signKeys)
        {
            throw new NotImplementedException();
        }

    }
}