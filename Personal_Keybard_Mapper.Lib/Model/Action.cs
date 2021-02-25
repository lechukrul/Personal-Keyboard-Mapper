﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
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

        [JsonProperty("OutputVirtualKeys")]
        public IEnumerable<string> Keys
        {
            get { return ActionStringKeys; }
            set
            {
                ActionStringKeys ??= new List<string>();
                VirtualKeys ??= new List<VirtualKeyCode>();
                ActionStringKeys.AddRange(value ?? new List<string>());
                ReplaceStringAliases();
                var desValue = JToken.FromObject(value);
                var vkArray = JsonConvert.DeserializeObject<IEnumerable<VirtualKeyCode>>(
                    desValue.ToString(),
                    new OutputKeysConverter()
                    );
                VirtualKeys.AddRange(vkArray ?? Array.Empty<VirtualKeyCode>());
            }
        }

        /// <summary>
        /// Replaces the string aliases.
        /// In some cases an action string in configuration file has to be specified by an alias.
        /// </summary>
        private void ReplaceStringAliases()
        {
            ActionStringKeys = ActionStringKeys.Select(x => x.Replace("backslash", "\\")).ToList();
            ActionStringKeys = ActionStringKeys.Select(x => x.Replace(@"/'/'", @"""""")).ToList();
        }

        public List<string> ActionStringKeys { get; set; }

        //[JsonConverter(typeof(OutputKeysConverter))]
        public List<VirtualKeyCode> VirtualKeys { get; set; }

        /// <summary>
        /// Determines whether an action is to send CRTL key.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if an action is to send CRTL key; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCrtlAction()
        {
            if (Type == ActionType.Keyboard && VirtualKeys.Count() == 1)
            {
                try
                {
                    var virtKey = VirtualKeys.First();
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
            if (Type == ActionType.Keyboard && VirtualKeys.Count() == 1)
            {
                try
                {
                    var virtKey = VirtualKeys.First();
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
            if (Type == ActionType.Keyboard && VirtualKeys.Count() == 1)
            {
                try
                {
                    var virtKey = VirtualKeys.First();
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
            if (Type == ActionType.Keyboard && VirtualKeys.Count() == 1)
            {
                try
                {
                    var virtKey = VirtualKeys.First();
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
            if (Type == ActionType.Keyboard && VirtualKeys.Count() == 1)
            {
                try
                {
                    var virtKey = VirtualKeys.First();
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
            if (Type == ActionType.Keyboard && VirtualKeys.Count() == 1)
            {
                try
                {
                    var virtKey = VirtualKeys.First();
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
            if (VirtualKeys.Count() != 1)
            {
                return false;
            }

            return VirtualKeys.First() == key;
        }

        private bool  IsModKey(VirtualKeyCode key)
        {
            return key == VirtualKeyCode.SHIFT || key == VirtualKeyCode.CONTROL || key == VirtualKeyCode.RMENU ||
                   key == VirtualKeyCode.LMENU || key == VirtualKeyCode.RWIN || key == VirtualKeyCode.LWIN;
        }

        /// <summary>
        /// Gets the action modification keys.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<VirtualKeyCode> GetActionModKeys()
        {
            return VirtualKeys.Where(x => IsModKey(x));
        }

        /// <summary>
        /// Gets the action no modification keys.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<VirtualKeyCode> GetActionNoModKeys()
        {
            return VirtualKeys.Where(x => !IsModKey(x));
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
                        modKeys = GetActionModKeys();
                        noModKeys = GetActionNoModKeys(); 
                        action.SendKeyboardAction(new InputSimulator(), modKeys, noModKeys);
                        return action.OnlyModKeyAction;
                    case ActionType.Mouse:
                        action = new MouseAction();
                        modKeys = GetActionModKeys();
                        noModKeys = GetActionNoModKeys();
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
                VirtualKeys = this.VirtualKeys.ToList()
            };
        }
    }
}