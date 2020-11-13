///<summary>
/// Class provides an .net api functions used in this application 
/// </summary>

using System;
using System.Runtime.InteropServices;
using Personal_Keyboard_Mapper.Lib.Enums;
using Personal_Keyboard_Mapper.Lib.Hooks;
using Personal_Keyboard_Mapper.Lib.Structures;

namespace Personal_Keyboard_Mapper.Lib.PInvokeApFunctions
{
    public class ApiFunctions
    {
        /// <summary>
        /// Loads the library.
        /// </summary>
        /// <param name="lpFileName">Name of the library</param>
        /// <returns>A handle to the library</returns>
        [DllImport("kernel32.dll")]
 	    public	static extern IntPtr LoadLibrary(string lpFileName);

        //This is the Import for the SetWindowsHookEx function.
        //Use this function to install a thread-specific keyboard hook.
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(HookType hookType, KeyboardHook.KeyboardHookProc lpfn, IntPtr hMod, uint dwThreadId);

        //This is the Import for the SetWindowsHookEx function.
        //Use this function to install a thread-specific mouse hook.
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(HookType hookType, MouseHook.MouseHookProc lpfn, IntPtr hMod, uint dwThreadId);

        //This is the Import for the CallNextHookEx function for global keyboard hook.
        //Use this function to pass the keyboard hook information to the next hook procedure in chain.
        [DllImport("user32.dll")]
        public  static extern IntPtr CallNextHookEx(IntPtr hookHandler, int nCode, int wParam, ref KeyboardHookStructure.HookStruct lParam);

        //This is the Import for the CallNextHookEx function for global mouse hook.
        //Use this function to pass the mouse hook information to the next hook procedure in chain.
        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr hookHandler, int nCode, int wParam, ref MouseHookStructure.HookStruct lParam);

        //This is the Import for the UnhookWindowsHookEx function.
        //Call this function to uninstall the hook.
        [DllImport("user32.dll", CharSet = CharSet.Auto,
         CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(IntPtr idHook);

        //This is the Import for the VkKeyScan function.
        //Call this function to convert a char to virtual key code.
        [DllImport("user32.dll")]
        public static extern short VkKeyScan(char ch);

        //This is the Import for the VkKeyScanEx function.
        //Call this function to convert a char to virtual key code.
        //Keyboard layout must be provided.
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern short VkKeyScanEx(char ch, uint dwhkl);

        //This is the Import for the UnloadKeyboardLayout function.
        //Call this function to unload keyboard layout.
        [DllImport("user32.dll")]
        public static extern bool UnloadKeyboardLayout(IntPtr KbdLayoutPointer);

        //This is the Import for the LoadKeyboardLayout function.
        //Call this function to load keyboard layout.
        [DllImport("user32.dll")]
        public static extern uint LoadKeyboardLayout(string KbdLayoutId, uint Flags);

        [DllImport("user32.dll",
            CallingConvention = CallingConvention.StdCall,
            CharSet = CharSet.Unicode,
            EntryPoint = "GetKeyboardLayout",
            SetLastError = true,
            ThrowOnUnmappableChar = false)]
        public static extern uint GetKeyboardLayout(
            uint idThread);

        [DllImport("user32.dll",
            CallingConvention = CallingConvention.StdCall,
            CharSet = CharSet.Unicode,
            EntryPoint = "ActivateKeyboardLayout",
            SetLastError = true,
            ThrowOnUnmappableChar = false)]
        public static extern uint ActivateKeyboardLayout(
            uint hkl,
            uint Flags);
    }
}
