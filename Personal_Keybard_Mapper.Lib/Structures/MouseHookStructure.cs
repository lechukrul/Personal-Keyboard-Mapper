using System.Runtime.InteropServices;
using Personal_Keyboard_Mapper.Lib.Interfaces;
using PInvoke;

namespace Personal_Keyboard_Mapper.Lib.Structures
{
    /// <summary>
    /// This structure is used to manage a mouse hook.
    /// </summary> 
    public class MouseHookStructure
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct HookStruct
        {
            public POINT pt;
            public int mousedata;
            public int wHitTestCode;
            public int dwExtraInfo;
        }
    }
}
