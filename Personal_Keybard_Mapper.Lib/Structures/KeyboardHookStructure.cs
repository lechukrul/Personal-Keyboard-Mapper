using System;
using System.Runtime.InteropServices;
using Personal_Keyboard_Mapper.Lib.Interfaces;

namespace Personal_Keyboard_Mapper.Lib.Structures
{
    /// <summary>
    /// This structure is used to manage a keyboard hook.
    /// </summary> 
    public class KeyboardHookStructure
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct HookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        } 
    }
}
