using System;
using System.Linq;
using WindowsInput.Native;
using log4net;
using Newtonsoft.Json;
using Personal_Keyboard_Mapper.Lib.Enums;
using Personal_Keyboard_Mapper.Lib.Interfaces;
using Personal_Keyboard_Mapper.Lib.PInvokeApFunctions;
using Personal_Keyboard_Mapper.Lib.Structures;

namespace Personal_Keyboard_Mapper.Lib.Model
{
    /// <summary>
    /// Represents a combination of three keyboard keys.
    /// </summary>
    /// <seealso cref="Personal_Keyboard_Mapper.Lib.Interfaces.IKeyCombination" />
    [JsonObject]
    public class ThreeKeysCombination : IKeyCombination
    {
        public ILog logger { get; set; }

        private string _firstKeyVirtualCode;
        private string _secondKeyVirtualCode;
        private string _thirdKeyVirtualCode;
        public ThreeKeysCombination()
        {
            Keys = new IKeyboardKey[3];
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
                    if (Globals.NumericKeypadVirtualKeyCodes.Contains((VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), value)))
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
                    if (Globals.NumericKeypadVirtualKeyCodes.Contains((VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), value)))
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

        [JsonProperty("ThirdKey")]
        public string ThirdKeyVirtualCode
        {
            get => _thirdKeyVirtualCode;
            set
            {
                VirtualKeyCode key;
                if (value.Length > 1)
                {
                    if (Globals.NumericKeypadVirtualKeyCodes.Contains((VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), value)))
                    {
                        _thirdKeyVirtualCode = value.Last().ToString();
                        key = (VirtualKeyCode)new VKeyScanHelper() { Value = ApiFunctions.VkKeyScan(_thirdKeyVirtualCode[0]) }.LowByte;
                        Keys[2] = new KeyboardKey(key, KeyCombinationPosition.Third);
                    }
                    else
                    {
                        throw new Exception("Single char is required");
                    }
                }
                else
                {
                    _thirdKeyVirtualCode = value;
                    if (value.Any())
                    {
                        key = (VirtualKeyCode)new VKeyScanHelper() { Value = ApiFunctions.VkKeyScan(_thirdKeyVirtualCode[0]) }.LowByte;
                        Keys[2] = new KeyboardKey(key, KeyCombinationPosition.Third); 
                    }
                }
            }
        }

        [JsonIgnore]
        /// <inheritdoc />
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
                case KeyCombinationPosition.Third:
                    ThirdKeyVirtualCode = keyCode.ToString();
                    break;
                case KeyCombinationPosition.None:
                    throw new Exception(nameof(SetKeyByPosition));
            }
        }

        public override bool Equals(object obj)
        {
            return (obj is ThreeKeysCombination comb) && (comb.FirstKeyVirtualCode == FirstKeyVirtualCode)
                                                      && (comb.SecondKeyVirtualCode == SecondKeyVirtualCode)
                                                      && (comb.ThirdKeyVirtualCode == ThirdKeyVirtualCode);
        }
        
        public override int GetHashCode()
        {
            unchecked
            {
                var firstPrime = 17;
                var secondPrime = 23;
                var hash = firstPrime * secondPrime + FirstKeyVirtualCode.GetHashCode();
                hash = hash * secondPrime + SecondKeyVirtualCode.GetHashCode();
                hash = hash * secondPrime + ThirdKeyVirtualCode.GetHashCode();
                return hash;
            }
        }

        public object Clone()
        {
            return new ThreeKeysCombination()
            {
                logger = this.logger,
                FirstKeyVirtualCode = this.FirstKeyVirtualCode,
                SecondKeyVirtualCode = this.SecondKeyVirtualCode,
                ThirdKeyVirtualCode = this.ThirdKeyVirtualCode,
                Keys = this.Keys.Select(x => (IKeyboardKey)x.Clone()).ToArray(),
                Action = (Action)this.Action.Clone()
            };
        }
        public void Clear()
        {
            Keys = new IKeyboardKey[2];
            FirstKeyVirtualCode = "";
            SecondKeyVirtualCode = "";
            ThirdKeyVirtualCode = "";
            Action = null;
        }
        public void SetLogger(ILog log)
        {
            logger = log;
        }
    }
}