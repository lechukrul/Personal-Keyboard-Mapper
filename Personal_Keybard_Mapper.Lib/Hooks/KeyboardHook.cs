using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;
using log4net;
using Personal_Keyboard_Mapper.Gui;
using Personal_Keyboard_Mapper.Lib.Enums;
using Personal_Keyboard_Mapper.Lib.Interfaces;
using Personal_Keyboard_Mapper.Lib.PInvokeApFunctions;
using Personal_Keyboard_Mapper.Lib.Structures;
using Personal_Keyboard_Mapper.Lib.Extensions;
using Personal_Keyboard_Mapper.Lib.Model;
using Action = Personal_Keyboard_Mapper.Lib.Model.Action;

namespace Personal_Keyboard_Mapper.Lib.Hooks
{
    /// <summary>
    /// an implementation of keyboard hook used in this app.
    /// </summary>
    public class KeyboardHook : IHook
    {
        public delegate IntPtr KeyboardHookProc(int code, int wParam, ref KeyboardHookStructure.HookStruct lParam);
        public ILog logger;
        const int keyboardKeyDownCode = 0x100;
        const int keyboardKeyUpCode = 0x101;
        private readonly List<VirtualKeyCode> combinationsKeysVirtualCodes;
        readonly List<VirtualKeyCode> numericKeypadKeysVirtualCodes;
        readonly List<VirtualKeyCode> numericKeypadWithShiftKeyVirtualCodes;
        readonly List<VirtualKeyCode> numericKeysVirtualCodes;
        private readonly List<VirtualKeyCode> modKeysVirtualCodes;
        WindowsInputDeviceStateAdaptor KeysStateChecker;
        InputSimulator inputSimulator;
        int combinationSize;
        IEnumerable<IKeyCombination> configCombinations;
        private IKeyCombination currentCombination;
        private IKeyCombination lastUsedCombination;
        IntPtr hookInstance;
        KeyboardHookProc keyboardHookDelegate;
        IntPtr keyboardHookHandler;
        private KeysSoundEffects soundEffects;
        private KeyCombinationsConfiguration baseConfiguration;
        private HelpWindow helperWindow;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardHook" /> class.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="combinationsConfiguration">The combinations configuration.</param>
        /// <param name="effects">The sounds effects.</param>
        /// <param name="helpWindow">Gui helper window</param>
        public KeyboardHook(ILog log, KeyCombinationsConfiguration combinationsConfiguration, KeysSoundEffects effects, 
            HelpWindow helpWindow = null)
        {
            logger = log;
            hookInstance = LoadPInvokeKernel32Library("User32");
            baseConfiguration = combinationsConfiguration;
            currentCombination = baseConfiguration.GetCombinationInstance(logger);
            combinationSize = baseConfiguration.CombinationSize;
            configCombinations = baseConfiguration.Combinations;
            lastUsedCombination = baseConfiguration.GetCombinationInstance(logger);
            combinationsKeysVirtualCodes = configCombinations.ToVirtualKeyCodes().ToList();
            numericKeypadKeysVirtualCodes = Globals.NumericKeypadVirtualKeyCodes.ToList();
            numericKeypadWithShiftKeyVirtualCodes = Globals.NumericKeypadWithShiftVirtualKeyCodes.ToList();
            numericKeysVirtualCodes = Globals.NumericVirtualKeyCodes.ToList();
            modKeysVirtualCodes = Globals.ModKeysVirtualKeyCodes.ToList();
            KeysStateChecker = new WindowsInputDeviceStateAdaptor();
            inputSimulator = new InputSimulator();
            soundEffects = effects;
            helperWindow = helpWindow;
        }

        public KeyboardHook()
        {
        }
           
        public IntPtr ConfigHook(int code, int wParam, ref KeyboardHookStructure.HookStruct lParam)
        {
            var wp = wParam;
            var vk = lParam.vkCode;
            var sk = lParam.scanCode;
            if (code < 0)
            {
                //you need to call CallNextHookEx without further processing
                //and return the value returned by CallNextHookEx
                return ApiFunctions.CallNextHookEx(hookInstance, code, wp, ref lParam);
            }
            if (inputSimulator.InputDeviceState.IsKeyDown(VirtualKeyCode.SHIFT))
            {
                logger.Debug($"vk={vk:X}");
                logger.Debug($"sk={sk:X}");
            }

            if (vk == (int)VirtualKeyCode.LSHIFT && sk == 554)
            {
                ResetPositionCounter();
            }
            if (numericKeysVirtualCodes.Contains((VirtualKeyCode)vk) || sk == 0)
            { 
                ResetPositionCounter();
                return ApiFunctions.CallNextHookEx(hookInstance, code, wp, ref lParam);
            }
            ConcatenatePosition();
            if (combinationsKeysVirtualCodes.Contains((VirtualKeyCode)vk) || numericKeypadKeysVirtualCodes.Contains((VirtualKeyCode)vk)
            || modKeysVirtualCodes.Contains((VirtualKeyCode)vk))
            {
                if (wp == keyboardKeyDownCode)
                {
                    KeyboardKey key;
                    if (!modKeysVirtualCodes.Contains((VirtualKeyCode)vk))
                    {
                        key = new KeyboardKey((VirtualKeyCode)vk, 
                            (KeyCombinationPosition)currentCombination.Keys.Count(x => x != null) + 1);
                    }
                    else
                    {
                        if ((VirtualKeyCode)vk == VirtualKeyCode.LSHIFT || (VirtualKeyCode)vk == VirtualKeyCode.RSHIFT)
                        {
                            vk = (int)VirtualKeyCode.SHIFT;
                        }
                        if ((VirtualKeyCode)vk == VirtualKeyCode.LCONTROL || (VirtualKeyCode)vk == VirtualKeyCode.RCONTROL)
                        {
                            vk = (int)VirtualKeyCode.CONTROL;
                        } 
                        var temp = configCombinations.Where(x => x.Action.IsModKeyAction())
                            .FirstOrDefault(x => x.Action.IsThisVirtualCode((VirtualKeyCode) vk));
                        if (temp != null)
                        {
                            DebugKey(wParam, vk, sk);
                            if (Globals.ModKeysVirtualKeyCodes.Contains((VirtualKeyCode)vk) && Globals.KeyCombinationPositionCounter >= 2)
                            {
                                ResetPositionCounter();
                                ConcatenatePosition();
                            }
                            if (Globals.ModKeysVirtualKeyCodes.Contains(temp.Keys[Globals.KeyCombinationPositionCounter].KeyCode))
                            {
                                currentCombination?.Clear();

                                return ApiFunctions.CallNextHookEx(hookInstance, code, wp, ref lParam);
                            }
                            
                            key = (Globals.IsMouseButtonWithModKey)
                                ? new KeyboardKey(temp.Keys[Globals.KeyCombinationPositionCounter].KeyCode,
                                    (KeyCombinationPosition) currentCombination.Keys.Count(x => x != null) + 1)
                                : new KeyboardKey(temp.Keys[Globals.KeyCombinationPositionCounter].KeyCode,
                                    (KeyCombinationPosition) currentCombination.Keys.Count(x => x != null));
                        }
                        else
                        {
                            throw new Exception("Unable to set a key");
                        }
                    }

                    if (key.CombinationPosition == KeyCombinationPosition.None)
                    {
                        return ApiFunctions.CallNextHookEx(hookInstance, code, wp, ref lParam);
                    }
                    currentCombination.SetKeyByPosition(key.CombinationPosition, key.KeyCode);
                    if (!currentCombination.IsFullCombination())
                    {
                        if (Globals.IsSoundOn)
                        {
                            try
                            { 
                                switch (key.CombinationPosition)
                                {
                                    case KeyCombinationPosition.First:
                                        soundEffects.PlaySound(SoundAction.FirstKey);
                                        Globals.FirstKeyInCombination = key.ToString();
                                        if (Globals.IsHelpWindowOn)
                                        {
                                            var possibleOutputActions = configCombinations
                                                                .Where(x => x.Keys[0].KeyCode.ConvertNumericKeyCodeToNumericKeypadKeyCode() == key.KeyCode)
                                                                .Select(x => x.Action.ToString()) 
                                                                .ToList();
                                            helperWindow.FillHelperRow(possibleOutputActions);
                                            helperWindow.TopMost = true;
                                            helperWindow.Show();
                                        }
                                        break;

                                    case KeyCombinationPosition.Second:
                                        soundEffects.PlaySound(SoundAction.SecondKey); 
                                        break;

                                    case KeyCombinationPosition.Third:
                                        soundEffects.PlaySound(SoundAction.ThirdKey);
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.Message);
                                logger.Error(ex.StackTrace);
                            }
                        }
                    }
                    else
                    {
                        switch (combinationSize)
                        {
                            case 2:
                            {
                                var combinations = configCombinations.Copy();
                                currentCombination =
                                    combinations.FirstOrDefault(x => x.Equals(currentCombination));
                                if (currentCombination == null)
                                {
                                    logger.Warn("No such combination");
                                    currentCombination = baseConfiguration.GetCombinationInstance(logger);
                                    Globals.ResetKeysFlags();
                                    ResetPositionCounter();
                                    PlayLastKeySound(key);
                                    helperWindow.ClearHelperRow();
                                    helperWindow.Hide();
                                    return (IntPtr)1;
                                }
                                if (!currentCombination.IsNotEmptyActionCombination(logger))
                                {
                                    if (lastUsedCombination.Action == null)
                                    {
                                        currentCombination.CopyTo(lastUsedCombination);
                                    }
                                    if (lastUsedCombination.IsFullCombination())
                                    {
                                        ReleasePressedOnceModKey();
                                    }
                                }
                                try
                                {
                                    if (!currentCombination.Action.IsModKeyAction())
                                    {
                                        PlayLastKeySound(key);
                                    }
                                    else
                                    {
                                        currentCombination.CopyTo(lastUsedCombination);
                                        if (currentCombination.Action.IsShiftAction())
                                        {
                                            if (Globals.IsSoundOn)
                                            {
                                                soundEffects.PlaySound(SoundAction.Shift);
                                            } 
                                            if (!Globals.IsShiftPressedOnce && !Globals.IsShiftHoldDown)
                                            {
                                                Globals.ShiftPressCounter = 1;
                                                Globals.IsShiftPressedOnce = true;
                                            }
                                            else if (!Globals.IsShiftHoldDown || Globals.ShiftPressCounter >= 1)
                                            {
                                                Globals.ShiftPressCounter++;
                                                Globals.IsShiftHoldDown = true;
                                                Globals.IsShiftPressedOnce = false;
                                            }

                                            if (Globals.ShiftPressCounter > 2)
                                            {
                                                Globals.IsShiftHoldDown = false;
                                                Globals.IsShiftPressedOnce = false;
                                            }
                                        }
                                        else if (currentCombination.Action.IsLeftAltAction())
                                        {
                                            if (Globals.IsSoundOn)
                                            {
                                                soundEffects.PlaySound(SoundAction.Alt);
                                            }
                                            if (!Globals.IsLeftAltPressedOnce && !Globals.IsLeftAltHoldDown)
                                            {
                                                Globals.LeftAltPressCounter = 1;
                                                Globals.IsLeftAltPressedOnce = true;
                                            }
                                            else if (!Globals.IsLeftAltHoldDown || Globals.LeftAltPressCounter >= 1)
                                            {
                                                Globals.LeftAltPressCounter++;
                                                Globals.IsLeftAltHoldDown = true;
                                                Globals.IsLeftAltPressedOnce = false;
                                            }

                                            if (Globals.LeftAltPressCounter > 2)
                                            {
                                                Globals.IsLeftAltPressedOnce = false;
                                                Globals.IsLeftAltHoldDown = false;
                                            }
                                        }
                                        else if (currentCombination.Action.IsRightAltAction())
                                        {
                                            if (Globals.IsSoundOn)
                                            {
                                                soundEffects.PlaySound(SoundAction.Alt);
                                            }
                                            if (!Globals.IsRightAltPressedOnce && !Globals.IsRightAltHoldDown)
                                            {
                                                Globals.RightAltPressCounter = 1;
                                                Globals.IsRightAltPressedOnce = true;
                                            }
                                            else if (!Globals.IsRightAltHoldDown || Globals.RightAltPressCounter >= 1)
                                            {
                                                Globals.RightAltPressCounter++;
                                                Globals.IsRightAltHoldDown = true;
                                                Globals.IsRightAltPressedOnce = false;
                                            }

                                            if (Globals.RightAltPressCounter > 2)
                                            {
                                                Globals.IsRightAltPressedOnce = false;
                                                Globals.IsRightAltHoldDown = false;
                                            }
                                        }
                                        else if (currentCombination.Action.IsCrtlAction())
                                        {
                                            if (Globals.IsSoundOn)
                                            {
                                                soundEffects.PlaySound(SoundAction.Crtl);
                                            }
                                            if (!Globals.IsCtrlPressedOnce && !Globals.IsCtrlHoldDown)
                                            {
                                                Globals.CtrlPressCounter = 1;
                                                Globals.IsCtrlPressedOnce = true;
                                            }
                                            else if (!Globals.IsCtrlHoldDown || Globals.CtrlPressCounter >= 1)
                                            {
                                                Globals.CtrlPressCounter++;
                                                Globals.IsCtrlHoldDown = true;
                                                Globals.IsCtrlPressedOnce = false;
                                            }

                                            if (Globals.CtrlPressCounter > 2)
                                            {
                                                Globals.IsCtrlHoldDown = false;
                                                Globals.IsCtrlPressedOnce = false;
                                            }
                                        }
                                        else if (currentCombination.Action.IsLeftWinAction())
                                        {
                                            if (Globals.IsSoundOn)
                                            {
                                                soundEffects.PlaySound(SoundAction.Win);
                                            }
                                            if (!Globals.IsLeftWinPressedOnce && !Globals.IsLeftWinHoldDown)
                                            {
                                                Globals.LeftWinPressCounter = 1;
                                                Globals.IsLeftWinPressedOnce = true;
                                            }
                                            else if (!Globals.IsLeftWinHoldDown || Globals.LeftWinPressCounter >= 1)
                                            {
                                                Globals.LeftWinPressCounter++;
                                                Globals.IsLeftWinHoldDown = true;
                                                Globals.IsLeftWinPressedOnce = false;
                                            }

                                            if (Globals.LeftWinPressCounter > 2)
                                            {
                                                Globals.IsLeftWinHoldDown = false;
                                                Globals.IsLeftWinPressedOnce = false;
                                            }
                                        }
                                        else if (currentCombination.Action.IsRightWinAction())
                                        {
                                            if (Globals.IsSoundOn)
                                            {
                                                soundEffects.PlaySound(SoundAction.Win);
                                            }
                                            if (!Globals.IsRightWinPressedOnce && !Globals.IsRightWinHoldDown)
                                            {
                                                Globals.RightWinPressCounter = 1;
                                                Globals.IsRightWinPressedOnce = true;
                                            }
                                            else if (!Globals.IsRightWinHoldDown || Globals.RightWinPressCounter >= 1)
                                            {
                                                Globals.RightWinPressCounter++;
                                                Globals.IsRightWinHoldDown = true;
                                                Globals.IsRightWinPressedOnce = false;
                                            }

                                            if (Globals.RightWinPressCounter > 2)
                                            {
                                                Globals.IsRightWinHoldDown = false;
                                                Globals.IsRightWinPressedOnce = false;
                                            }
                                        } 
                                    } 
                                }
                                catch (Exception e)
                                {
                                    logger.Error(e.StackTrace);
                                }

                                var combinationAction = currentCombination.Action;
                                if (combinationAction == null)
                                {
                                    currentCombination?.Clear();
                                    logger.Warn("No action defined");
                                }

                                else
                                {
                                    var onlyModKeyAction = combinationAction.Run();
                                    if (!onlyModKeyAction)
                                    {
                                        ReleasePressedOnceModKey();
                                    }
                                    currentCombination.Clear();
                                    helperWindow.ClearHelperRow();
                                    helperWindow.Hide();
                                    configCombinations = baseConfiguration.Combinations;

                                }

                                break;
                            }
                            case 3: 
                                // TODO
                                break;
                        }
                    }
                    ResetPositionCounter();
                    return (IntPtr)1;
                } 
                if (wp == keyboardKeyUpCode)
                {
                    if (Globals.NumericKeypadWithShiftVirtualKeyCodes.Contains((VirtualKeyCode)vk))
                    {
                        if (Globals.KeyCombinationPositionCounter >= 2)
                        {
                            currentCombination?.Clear();
                        }
                        ResetPositionCounter();
                        return (IntPtr) 1;
                    } 
                }
                ResetPositionCounter();
                return ApiFunctions.CallNextHookEx(hookInstance, -1, wp, ref lParam);
            }
            ResetPositionCounter();
            return ApiFunctions.CallNextHookEx(hookInstance, code, wp, ref lParam);
        }

        private void ConcatenatePosition()
        {
            Globals.KeyCombinationPositionCounter++;

        }

        private void DebugKey(int keyState, int keyVkCode, int keySkCode)
        {
            logger.Debug("KEY DEBUG START");
            logger.Debug($"position = {Globals.KeyCombinationPositionCounter}");
            logger.Debug($"key state = {keyState}, key vk code = {keyVkCode}, key sk code = {keySkCode}");
            logger.Debug("KEY DEBUG END");
        }

        private void ResetPositionCounter()
        {
            if (Globals.KeyCombinationPositionCounter >= combinationSize)
            {
                Globals.KeyCombinationPositionCounter = -1;
            }
        }

        /// <summary>
        /// Releases the pressed once mod key.
        /// </summary>
        private void ReleasePressedOnceModKey()
        {
            if (lastUsedCombination.Action == null)
            {
                currentCombination.CopyTo(lastUsedCombination);
            }
            if (lastUsedCombination.Action.IsCrtlAction() && Globals.IsCtrlPressedOnce)
            {
                Globals.IsCtrlPressedOnce = false;
            }

            if (lastUsedCombination.Action.IsShiftAction() && Globals.IsShiftPressedOnce)
            {
                Globals.IsShiftPressedOnce = false;
            }

            if (lastUsedCombination.Action.IsLeftAltAction() && Globals.IsLeftAltPressedOnce)
            {
                Globals.IsLeftAltPressedOnce = false;
            }

            if (lastUsedCombination.Action.IsRightAltAction() && Globals.IsRightAltPressedOnce)
            {
                Globals.IsRightAltPressedOnce = false;
            }

            if (lastUsedCombination.Action.IsLeftWinAction() && Globals.IsLeftWinPressedOnce)
            {
                Globals.IsLeftWinPressedOnce = false;
            }

            if (lastUsedCombination.Action.IsRightWinAction() && Globals.IsRightWinPressedOnce)
            {
                Globals.IsRightWinPressedOnce = false;
            }
        }

        private void PlayLastKeySound(KeyboardKey key)
        {
            if (Globals.IsSoundOn)
            {
                switch (key.CombinationPosition)
                {
                    case KeyCombinationPosition.Second:
                        soundEffects.PlaySound(SoundAction.SecondKey);
                        break;

                    case KeyCombinationPosition.Third:
                        soundEffects.PlaySound(SoundAction.ThirdKey);
                        break;
                } 
            }
            Globals.FirstKeyInCombination = String.Empty;
        }

        private bool IsFullCombination()
        {
            return currentCombination.Keys.Length == combinationSize;
        }

        public void StartHook()
        {
            try
            {
                if (keyboardHookDelegate == null)
                {
                    keyboardHookDelegate = ConfigHook;
                }
                keyboardHookHandler = ApiFunctions.SetWindowsHookEx(HookType.WH_KEYBOARD_LL, keyboardHookDelegate, hookInstance, 0);
                logger.Info("Keyboard hook started");
            }
            catch (Exception ex)
            {
                logger.Error($"START KEBOARD HOOK ERROR: {ex.StackTrace}");
            }
        }

        public void StopHook()
        {
            ApiFunctions.UnhookWindowsHookEx(keyboardHookHandler);
        }

        /// <summary>
        /// Loads and gets the pinvoke kernel32 library.
        /// </summary>
        /// <param name="libraryName">Name of the library.</param>
        /// <returns>The library handler</returns>
        public IntPtr LoadPInvokeKernel32Library(string libraryName)
        {
            try
            {
                var libraryHandle = ApiFunctions.LoadLibrary(libraryName);
                return libraryHandle;
            }
            catch (Exception ex)
            {
                logger.Error($"LOAD USER32 LIBRARY ERROR: {ex.StackTrace}");
                return IntPtr.Zero;
            }
        }
    }
}
