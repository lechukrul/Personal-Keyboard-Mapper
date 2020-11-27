using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using WindowsInput.Native;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Personal_Keyboard_Mapper.Lib.Enums;
using Personal_Keyboard_Mapper.Lib.Model;
using Personal_Keyboard_Mapper.Lib.Model.KeyboardLayout;
using Personal_Keyboard_Mapper.Lib.PInvokeApFunctions;
using Personal_Keyboard_Mapper.Lib.Structures;

namespace Personal_Keyboard_Mapper.Lib.Converters
{
    public class OutputKeysConverter : JsonConverter<IEnumerable<VirtualKeyCode>>
    {
        private Dictionary<string, VirtualKeyCode> keyboardCommandsDict;
        private Dictionary<string, (MouseButtonAction mouseButtonAction, IEnumerable<VirtualKeyCode> mouseVirtualKeyCodes)> mouseCommandsDict;
        private List<(string sign, VirtualKeyCode[] keys)> bracesKeyCodes;
        private IEnumerable<string> leftArrowRequireStrings;
        public OutputKeysConverter()
        {
            SetKeyboardCommandsDict();
            SetMouseCommandsDict();
            SetBracesList();
            SetLeftArrowRequireStringList();
        }

        public override void WriteJson(JsonWriter writer, IEnumerable<VirtualKeyCode> value, JsonSerializer serializer)
        {
            foreach (var key in value.Select(x => x.ToString()))
            {
                if (key.StartsWith("VK_") || key.StartsWith("NUMPAD"))
                {
                    var character = key.ToLower().Last();
                    writer.WriteValue(character);
                }
                else
                {
                    writer.WriteValue(key.ToLower());
                }
            }
        }

        public override IEnumerable<VirtualKeyCode> ReadJson(JsonReader reader, Type objectType, 
            IEnumerable<VirtualKeyCode> existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            List<VirtualKeyCode> result = new List<VirtualKeyCode>(); 
            ApiFunctions.LoadLibrary("User32");
            var keys = JArray.Load(reader);
            foreach (var key in keys)
            {
                var stringKey = key.ToObject<string>()?.Replace("/'", "\"");
                var culture = new CultureInfo("pl-PL");
                if (stringKey.Length == 1)
                {
                    var charKey = stringKey[0];
                    var bracket = bracesKeyCodes.FirstOrDefault(x => x.sign == charKey.ToString());
                    if (!bracket.Equals(default))
                    {
                        result.AddRange(bracket.keys);
                        break;
                    } 
                    result.AddRange(CharToVirtualCode(charKey, culture));
                }
                else
                {
                    if (keyboardCommandsDict.ContainsKey(stringKey))
                    {
                        result.Add(keyboardCommandsDict[stringKey]);
                    }
                    else if (mouseCommandsDict.ContainsKey(stringKey))
                    {
                        result.AddRange(mouseCommandsDict[stringKey].mouseVirtualKeyCodes);
                    }
                    else
                    {
                        foreach (var charKey in stringKey)
                        {
                            result.AddRange(CharToVirtualCode(charKey, culture));
                        }
                        AddLeftArrowToAction(stringKey, result);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Converts character to virtual code.
        /// </summary>
        /// <param name="charKey">The character key.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>List of <see cref="VirtualKeyCode"/>, which represents a converted character</returns>
        private static IEnumerable<VirtualKeyCode> CharToVirtualCode(char charKey, CultureInfo culture)
        {
            List<VirtualKeyCode> result = new List<VirtualKeyCode>();
            using (var keyboardLayout = new KeyboardLayoutScope(culture))
            {
                var virtKeyHelper = new VKeyScanHelper() { Value = ApiFunctions.VkKeyScanEx(charKey, keyboardLayout.currentLayout.Handle) };
                switch ((int)virtKeyHelper.HighByte)
                {
                    case 0:
                        result.Add((VirtualKeyCode)virtKeyHelper.LowByte);
                        break;
                    case 1:
                        result.Add(VirtualKeyCode.SHIFT);
                        result.Add((VirtualKeyCode)virtKeyHelper.LowByte);
                        break;
                    case 2:
                        result.Add(VirtualKeyCode.CONTROL);
                        result.Add((VirtualKeyCode)virtKeyHelper.LowByte);
                        break;
                    case 6:
                        result.Add(VirtualKeyCode.RMENU);
                        result.Add((VirtualKeyCode)virtKeyHelper.LowByte);
                        break;
                    case 7:
                        result.Add(VirtualKeyCode.SHIFT);
                        result.Add(VirtualKeyCode.RMENU);
                        result.Add((VirtualKeyCode)virtKeyHelper.LowByte);
                        break;
                }
            }

            return result;
        }

        private void SetKeyboardCommandsDict()
        {
            keyboardCommandsDict = new Dictionary<string, VirtualKeyCode>()
            {
                [ConfigurationManager.AppSettings["enterAlias"]] = VirtualKeyCode.RETURN,
                [ConfigurationManager.AppSettings["shiftAlias"]] = VirtualKeyCode.SHIFT,
                [ConfigurationManager.AppSettings["crtlAlias"]] = VirtualKeyCode.CONTROL,
                [ConfigurationManager.AppSettings["altAlias"]] = VirtualKeyCode.RMENU,
                [ConfigurationManager.AppSettings["winAlias"]] = VirtualKeyCode.LWIN,
                [ConfigurationManager.AppSettings["escAlias"]] = VirtualKeyCode.ESCAPE,
                [ConfigurationManager.AppSettings["lArrowAlias"]] = VirtualKeyCode.LEFT,
                [ConfigurationManager.AppSettings["rArrowAlias"]] = VirtualKeyCode.RIGHT,
                [ConfigurationManager.AppSettings["dArrowAlias"]] = VirtualKeyCode.DOWN,
                [ConfigurationManager.AppSettings["uArrowAlias"]] = VirtualKeyCode.UP,
                [ConfigurationManager.AppSettings["tabAlias"]] = VirtualKeyCode.TAB,
                [ConfigurationManager.AppSettings["delAlias"]] = VirtualKeyCode.DELETE,
                [ConfigurationManager.AppSettings["insAlias"]] = VirtualKeyCode.INSERT,
                [ConfigurationManager.AppSettings["backslashAlias"]] = VirtualKeyCode.OEM_102,
                [ConfigurationManager.AppSettings["homeAlias"]] = VirtualKeyCode.HOME,
                [ConfigurationManager.AppSettings["endAlias"]] = VirtualKeyCode.END
            };
            for (int i = (int)VirtualKeyCode.F1; i <= (int)VirtualKeyCode.F12; i++)
            {
                keyboardCommandsDict.Add(((VirtualKeyCode)i).ToString(), (VirtualKeyCode)i);
            }
        }

        private void SetMouseCommandsDict()
        {
            mouseCommandsDict = new Dictionary<string, (MouseButtonAction mouseButtonAction, IEnumerable<VirtualKeyCode> mouseKeyCodes)>()
            {
                [ConfigurationManager.AppSettings["leftMouseClickAlias"]] = (MouseButtonAction.L, new List<VirtualKeyCode>()
                    {
                        VirtualKeyCode.LBUTTON
                    }),
                [ConfigurationManager.AppSettings["rightMouseClickAlias"]] = (MouseButtonAction.R, new List<VirtualKeyCode>()
                {
                    VirtualKeyCode.RBUTTON 
                }),
                [ConfigurationManager.AppSettings["middleMouseClickAlias"]] = (MouseButtonAction.M, new List<VirtualKeyCode>()
                {
                    VirtualKeyCode.MBUTTON
                }),
                [ConfigurationManager.AppSettings["leftDoubleMouseClickAlias"]] = (MouseButtonAction.LD, new List<VirtualKeyCode>()
                {
                    VirtualKeyCode.LBUTTON,
                    VirtualKeyCode.LBUTTON
                }),
                [ConfigurationManager.AppSettings["rightDoubleMouseClickAlias"]] = (MouseButtonAction.RD, new List<VirtualKeyCode>()
                {
                    VirtualKeyCode.RBUTTON,
                    VirtualKeyCode.RBUTTON
                }),
                [ConfigurationManager.AppSettings["middleDoubleMouseClickAlias"]] = (MouseButtonAction.MD, new List<VirtualKeyCode>()
                {
                    VirtualKeyCode.MBUTTON,
                    VirtualKeyCode.MBUTTON
                }),
                [ConfigurationManager.AppSettings["leftHoldMouseClickAlias"]] = (MouseButtonAction.LH, new List<VirtualKeyCode>()
                {
                    VirtualKeyCode.LBUTTON,
                    VirtualKeyCode.LBUTTON,
                    VirtualKeyCode.LBUTTON
                }),
                [ConfigurationManager.AppSettings["rightHoldMouseClickAlias"]] = (MouseButtonAction.RH, new List<VirtualKeyCode>()
                {
                    VirtualKeyCode.RBUTTON,
                    VirtualKeyCode.RBUTTON,
                    VirtualKeyCode.RBUTTON
                }),
                [ConfigurationManager.AppSettings["middleHoldMouseClickAlias"]] = (MouseButtonAction.MH, new List<VirtualKeyCode>()
                {
                    VirtualKeyCode.MBUTTON,
                    VirtualKeyCode.MBUTTON,
                    VirtualKeyCode.MBUTTON
                })
            }; 
        }

        private void SetBracesList()
        {
            bracesKeyCodes = new List<(string sign, VirtualKeyCode[] keys)>()
            {
                ("(", new[] {VirtualKeyCode.SHIFT, VirtualKeyCode.VK_9}),
                (")", new[] {VirtualKeyCode.SHIFT, VirtualKeyCode.VK_0}),
                ("[", new[] {VirtualKeyCode.OEM_4}),
                ("]", new[] {VirtualKeyCode.OEM_6}),
                ("{", new[] {VirtualKeyCode.SHIFT, VirtualKeyCode.OEM_4}),
                ("}", new[] {VirtualKeyCode.SHIFT, VirtualKeyCode.OEM_6}),
                ("<", new[] {VirtualKeyCode.SHIFT, VirtualKeyCode.DECIMAL}),
                (">", new[] {VirtualKeyCode.SHIFT, VirtualKeyCode.OEM_COMMA}),
            };
        }

        private void SetLeftArrowRequireStringList()
        {
            leftArrowRequireStrings = new List<string>()
            {
                "()",
                "{}",
                "[]",
                "<>",
                @"''",
                "\"\""
            };
        }

        private void AddLeftArrowToAction(string stringKey, List<VirtualKeyCode> actionKeys)
        {
            if (leftArrowRequireStrings.Contains(stringKey))
            {
                actionKeys.Add(VirtualKeyCode.LEFT);
            }
        }

    }
}