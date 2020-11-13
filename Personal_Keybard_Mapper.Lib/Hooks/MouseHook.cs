using System;
using System.Collections.Generic;
using System.Linq;
using WindowsInput;
using WindowsInput.Native;
using log4net;
using Personal_Keyboard_Mapper.Lib.Enums;
using Personal_Keyboard_Mapper.Lib.Interfaces;
using Personal_Keyboard_Mapper.Lib.PInvokeApFunctions;
using Personal_Keyboard_Mapper.Lib.Structures;

namespace Personal_Keyboard_Mapper.Lib.Hooks
{
    /// <summary>
    /// an implementation of mouse hook used in this app.
    /// </summary>
    public class MouseHook : IHook
    {
        public delegate IntPtr MouseHookProc(int code, int wParam, ref MouseHookStructure.HookStruct lParam);

        public ILog logger;
        private MouseHookProc mouseHookProc;
        private IntPtr mouseHookHandler;
        private WindowsInputDeviceStateAdaptor deviceStateChecker ;
        private InputSimulator inputSimulator;
        private IntPtr hInstance;
        public MouseHook(ILog log)
        {
            logger = log;
            mouseHookProc = null;
            mouseHookHandler = IntPtr.Zero;
            deviceStateChecker = new WindowsInputDeviceStateAdaptor();
            inputSimulator = new InputSimulator();
            hInstance = ApiFunctions.LoadLibrary("User32");
        } 

        /// <summary>
        /// Global mouse hook procedure
        /// </summary>
        /// <param name="code"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private IntPtr ConfigHook(int code, int wParam, ref MouseHookStructure.HookStruct lParam)
        {
            //DebugMousePressSignal(code, wParam, lParam);
            if (code < 0)
            {
                //you need to call CallNextHookEx without further processing
                //and return the value returned by CallNextHookEx
                return ApiFunctions.CallNextHookEx(mouseHookHandler, code, wParam, ref lParam);
            } 
            if (MouseCodes.WM_LBUTTONDOWN == (MouseCodes)wParam || MouseCodes.WM_RBUTTONDOWN == (MouseCodes)wParam)

            {
                var modKeysToPressOnce = Globals.GetModKeysToPressOnce().ToList();
                var modKeysToUse = Globals.GetModKeysToHoldDown().ToList();
                modKeysToUse.AddRange(modKeysToPressOnce);

                CheckCrtlKeyPressed();
                //CheckShiftKeyPressed();
            }
            return ApiFunctions.CallNextHookEx(mouseHookHandler, code, wParam, ref lParam);
        }

        private void DebugMousePressSignal(int code, int wParam, MouseHookStructure.HookStruct lParam)
        {
            logger.Debug($"mouse code = {code}");
            logger.Debug($"mouse wparam = {wParam}");
            logger.Debug($"mouse lparam struct = (info = {lParam.dwExtraInfo}, mouseData {lParam.mousedata}, " +
                         $"testCode {lParam.wHitTestCode}, Point.X = {lParam.pt.x}, Point.Y = {lParam.pt.y})");
        }

        private void CheckCrtlKeyPressed()
        {
            if (Globals.IsCtrlPressedOnce || Globals.IsCtrlHoldDown)
            {
                if (inputSimulator.InputDeviceState.IsKeyUp(VirtualKeyCode.CONTROL))
                {
                    inputSimulator.Keyboard.KeyDown(VirtualKeyCode.CONTROL);
                }

                Globals.IsCtrlPressedOnce = false;
            }
            else if ((!Globals.IsCtrlHoldDown && inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.CONTROL))
                     || !Globals.IsCtrlPressedOnce)
            {
                if (inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.CONTROL))
                {
                    inputSimulator.Keyboard.KeyUp(VirtualKeyCode.CONTROL);
                }
            }
        }

        private void CheckShiftKeyPressed()
        {
            DebugKeyState(VirtualKeyCode.SHIFT);
            if (Globals.IsShiftPressedOnce || Globals.IsShiftHoldDown)
            {
                if (inputSimulator.InputDeviceState.IsKeyUp(VirtualKeyCode.SHIFT))
                {
                    Globals.IsMouseButtonWithModKey = true;
                    inputSimulator.Keyboard.KeyDown(VirtualKeyCode.SHIFT);
                    Globals.KeyCombinationPositionCounter = -1;
                }

                Globals.IsShiftPressedOnce = false;
            }
            else if ((!Globals.IsShiftHoldDown && inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.SHIFT))
                     || !Globals.IsShiftPressedOnce)
            {
                Globals.IsMouseButtonWithModKey = false;
                if (inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.SHIFT))
                {
                    inputSimulator.Keyboard.KeyUp(VirtualKeyCode.SHIFT);
                }
                Globals.KeyCombinationPositionCounter = -1;
            }
        }

        void DebugKeyState(VirtualKeyCode key)
        {
            if (inputSimulator.InputDeviceState.IsKeyUp(key))
            {
                logger.Debug($"{key} is up");
            }
            else
            {
                logger.Debug($"{key} is down");
            }
        }

        public void StartHook()
        {
            if (mouseHookProc == null)
            {
                mouseHookProc = new MouseHookProc(ConfigHook);
            }

            mouseHookHandler = ApiFunctions.SetWindowsHookEx(HookType.WH_MOUSE_LL, mouseHookProc, hInstance, 0);
            logger.Info("Keyboard hook started");

        }

        public void StopHook()
        {
                ApiFunctions.UnhookWindowsHookEx(mouseHookHandler);
        }
    }
}