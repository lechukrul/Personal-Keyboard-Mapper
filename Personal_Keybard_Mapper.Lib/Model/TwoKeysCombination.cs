using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;
using log4net;
using Newtonsoft.Json;
using Personal_Keyboard_Mapper.Lib.Enums;
using Personal_Keyboard_Mapper.Lib.Extensions;
using Personal_Keyboard_Mapper.Lib.Interfaces;
using Personal_Keyboard_Mapper.Lib.PInvokeApFunctions;
using Personal_Keyboard_Mapper.Lib.Structures;

namespace Personal_Keyboard_Mapper.Lib.Model
{
    /// <summary>
    /// Represents a combination of two keyboard keys.
    /// </summary>
    /// <seealso cref="Personal_Keyboard_Mapper.Lib.Interfaces.IKeyCombination" />
    [JsonObject]
    public class TwoKeysCombination : IKeyCombination
    {
        public ILog logger { get; set; }

        private string _firstKeyVirtualCode;
        private string _secondKeyVirtualCode;
        public TwoKeysCombination()
        {
            Keys = new IKeyboardKey[2];
        }

        [JsonProperty("FirstKey")]
        public string FirstKeyVirtualCode
        {
            get => _firstKeyVirtualCode;
            set
            {
                VirtualKeyCode key;
                if (value.Length > 1)
                {
                    var tempKey = ((VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), value)).ConvertNumericKeyCodeToNumericKeypadKeyCode();
                    if (Globals.NumericKeypadVirtualKeyCodes.Contains(tempKey))
                    {
                        _firstKeyVirtualCode = value.Last().ToString();
                        key = (VirtualKeyCode)new VKeyScanHelper() { Value = ApiFunctions.VkKeyScan(_firstKeyVirtualCode[0]) }.LowByte;
                        Keys[0] = new KeyboardKey(key, KeyCombinationPosition.First);
                    }
                    else
                    {
                        throw new Exception("Single char is required");
                    }
                }
                else
                {
                    _firstKeyVirtualCode = value;
                    if (value.Any())
                    {
                        key = (VirtualKeyCode)new VKeyScanHelper() { Value = ApiFunctions.VkKeyScan(_firstKeyVirtualCode[0]) }.LowByte;
                        Keys[0] = new KeyboardKey(key, KeyCombinationPosition.First); 
                    }
                }
            } 
        }

        [JsonProperty("SecondKey")]
        public string SecondKeyVirtualCode
        {
            get => _secondKeyVirtualCode;
            set
            {
                VirtualKeyCode key;
                if (value.Length > 1)
                {
                    var tempKey = ((VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), value)).ConvertNumericKeyCodeToNumericKeypadKeyCode();
                    if (Globals.NumericKeypadVirtualKeyCodes.Contains(tempKey))
                    {
                        _secondKeyVirtualCode = value.Last().ToString();
                        key = (VirtualKeyCode)new VKeyScanHelper() { Value = ApiFunctions.VkKeyScan(_secondKeyVirtualCode[0]) }.LowByte;
                        Keys[1] = new KeyboardKey(key, KeyCombinationPosition.Second);
                    }
                    else
                    {
                        throw new Exception("Single char is required");
                    }
                }
                else
                {
                    _secondKeyVirtualCode = value;
                    if (value.Any())
                    {
                        key = (VirtualKeyCode)new VKeyScanHelper() { Value = ApiFunctions.VkKeyScan(_secondKeyVirtualCode[0]) }.LowByte;
                        Keys[1] = new KeyboardKey(key, KeyCombinationPosition.Second); 
                    }
                }
            }
        }

        [JsonIgnore]
        public IKeyboardKey[] Keys { get; set; }

        [JsonProperty]
        public Action Action { get; set; }

        public void SetKeyByPosition(KeyCombinationPosition position, VirtualKeyCode keyCode)
        {
            if (Keys == null)
            {
                Keys = new IKeyboardKey[2];
            }
            switch (position)
            {
                case KeyCombinationPosition.First:
                    FirstKeyVirtualCode = keyCode.ToString();
                    break;
                case KeyCombinationPosition.Second:
                    SecondKeyVirtualCode = keyCode.ToString();
                    break;
                case KeyCombinationPosition.None:
                     throw new Exception(nameof(SetKeyByPosition));
            }
        }

        public override bool Equals(object obj)
        {
            return (obj is TwoKeysCombination comb) && (comb.FirstKeyVirtualCode == FirstKeyVirtualCode)
                                                    && (comb.SecondKeyVirtualCode == SecondKeyVirtualCode);
        }
         

        public override int GetHashCode()
        {
            unchecked
            {
                var firstPrime = 17;
                var secondPrime = 23;
                var hash = firstPrime * secondPrime + FirstKeyVirtualCode.GetHashCode();
                hash = hash * secondPrime + SecondKeyVirtualCode.GetHashCode();
                return hash;
            }
        }

        public object Clone()
        {
            return new TwoKeysCombination()
            {
                logger = this.logger,
                FirstKeyVirtualCode = this.FirstKeyVirtualCode,
                SecondKeyVirtualCode = this.SecondKeyVirtualCode,
                Keys = this.Keys.Select(x => (IKeyboardKey)x.Clone()).ToArray(),
                Action = (Action)this.Action.Clone()
            };
        }

        public void Clear()
        {
            Keys = new IKeyboardKey[2];
            FirstKeyVirtualCode = "";
            SecondKeyVirtualCode = "";
            Action = null;
        }

        private void DebugKeysCollection()
        {
            logger.Debug($"START {this.GetType()}.{MethodBase.GetCurrentMethod().Name} DEBUG");
            logger.Debug($"{Keys[0]}, {Keys[1]}");
            logger.Debug($"END {this.GetType()}.{MethodBase.GetCurrentMethod().Name} DEBUG");
        }

        public void SetLogger(ILog log)
        {
            logger = log;
        }
    }
}