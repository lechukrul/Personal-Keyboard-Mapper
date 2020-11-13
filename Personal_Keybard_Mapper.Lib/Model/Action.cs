using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Personal_Keyboard_Mapper.Lib.Converters;
using Personal_Keyboard_Mapper.Lib.Enums;
using Personal_Keyboard_Mapper.Lib.Interfaces;

namespace Personal_Keyboard_Mapper.Lib.Model
{
    /// <summary>
    /// Represents an action related with the specific keys combination
    /// </summary>
    public  class Action : ICloneable
    {
        [JsonIgnore]
        public ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public ActionType Type { get; set; }

        [JsonConverter(typeof(OutputKeysConverter))]
        [JsonProperty("OutputVirtualKeys")]
        public IEnumerable<VirtualKeyCode> OutputVirtualKeys { get; set; }

        /// <summary>
        /// Determines whether an action is to send CRTL key.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if an action is to send CRTL key; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCrtlAction()
        {
            if (Type == ActionType.Keyboard && OutputVirtualKeys.Count() == 1)
            {
                try
                {
                    var virtKey = OutputVirtualKeys.First();
                    if (virtKey == VirtualKeyCode.CONTROL)
                    {
                        return true;
                    }
                }
                catch (Exception e)
                {
                    log.Error(e.StackTrace);
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether an action is to send SHIFT key.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if an action is to send SHIFT key; otherwise, <c>false</c>.
        /// </returns>
        public bool IsShiftAction()
        {
            if (Type == ActionType.Keyboard && OutputVirtualKeys.Count() == 1)
            {
                try
                {
                    var virtKey = OutputVirtualKeys.First();
                    if (virtKey == VirtualKeyCode.SHIFT)
                    {
                        return true;
                    }
                }
                catch (Exception e)
                {
                    log.Error(e.StackTrace);
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether an action is to send right ALT key.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if an action is to send right ALT key; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRightAltAction()
        {
            if (Type == ActionType.Keyboard && OutputVirtualKeys.Count() == 1)
            {
                try
                {
                    var virtKey = OutputVirtualKeys.First();
                    if (virtKey == VirtualKeyCode.RMENU)
                    {
                        return true;
                    }
                }
                catch (Exception e)
                {
                    log.Error(e.StackTrace);
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether an action is to send left ALT key.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if an action is to send left ALT key; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLeftAltAction()
        {
            if (Type == ActionType.Keyboard && OutputVirtualKeys.Count() == 1)
            {
                try
                {
                    var virtKey = OutputVirtualKeys.First();
                    if (virtKey == VirtualKeyCode.LMENU)
                    {
                        return true;
                    }
                }
                catch (Exception e)
                {
                    log.Error(e.StackTrace);
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether an action is to send left WIN key.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if an action is to send left WIN key; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLeftWinAction()
        {
            if (Type == ActionType.Keyboard && OutputVirtualKeys.Count() == 1)
            {
                try
                {
                    var virtKey = OutputVirtualKeys.First();
                    if (virtKey == VirtualKeyCode.LWIN)
                    {
                        return true;
                    }
                }
                catch (Exception e)
                {
                    log.Error(e.StackTrace);
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether an action is to send Right WIN key.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if an action is to send Right WIN key; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRightWinAction()
        {
            if (Type == ActionType.Keyboard && OutputVirtualKeys.Count() == 1)
            {
                try
                {
                    var virtKey = OutputVirtualKeys.First();
                    if (virtKey == VirtualKeyCode.RWIN)
                    {
                        return true;
                    }
                }
                catch (Exception e)
                {
                    log.Error(e.StackTrace);
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether modification key action is performed.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if modification key action is performed; otherwise, <c>false</c>.
        /// </returns>
        public bool IsModKeyAction()
        {
            return IsShiftAction() || IsCrtlAction() || IsLeftAltAction() || IsRightAltAction() || IsLeftWinAction() ||
                   IsRightWinAction();
        }

        public bool IsThisVirtualCode(VirtualKeyCode key)
        {
            if (OutputVirtualKeys.Count() != 1)
            {
                return false;
            }

            return OutputVirtualKeys.First() == key;
        }

        private bool  IsModKey(VirtualKeyCode key)
        {
            return key == VirtualKeyCode.SHIFT || key == VirtualKeyCode.CONTROL || key == VirtualKeyCode.RMENU ||
                   key == VirtualKeyCode.LMENU || key == VirtualKeyCode.RWIN || key == VirtualKeyCode.LWIN;
        }

        public bool Run()
        {
            IOutputAction action;
            try
            {
                IEnumerable<VirtualKeyCode> modKeys;
                IEnumerable<VirtualKeyCode> noModKeys;
                modKeys = default;
                noModKeys = default;
                switch (Type)
                {
                    case ActionType.Keyboard: 
                        action = new KeyboardAction(); 
                        modKeys = OutputVirtualKeys.Where(x => IsModKey(x));
                        noModKeys = OutputVirtualKeys.Where(x => !IsModKey(x)); 
                        action.SendKeyboardAction(new InputSimulator(), modKeys, noModKeys);
                        return action.OnlyModKeyAction;
                    case ActionType.Mouse:
                        action = new MouseAction();
                        modKeys = OutputVirtualKeys.Where(x => IsModKey(x));
                        noModKeys = OutputVirtualKeys.Where(x => !IsModKey(x));
                        action.SendMouseAction(new InputSimulator(), modKeys, noModKeys);
                        return action.OnlyModKeyAction;
                    default:
                        return false;
                }
            }
            catch (Exception e)
            {
                log.Error($"Unable to send output {e.StackTrace}");
                return false;
            }
        }

        public Task RunAsync()
        {
            throw new NotImplementedException();
        }

        public object Clone()
        {
            return new Action()
            {
                log = this.log,
                Type = this.Type, 
                OutputVirtualKeys = this.OutputVirtualKeys.ToList()
            };
        }
    }
}