using System.Runtime.InteropServices;
using Personal_Keyboard_Mapper.Lib.PInvokeApFunctions;

namespace Personal_Keyboard_Mapper.Lib.Structures
{
    /// <summary>
    /// Helper structure for the <see cref="ApiFunctions.VkKeyScan"/> method
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct VKeyScanHelper
    {
        [FieldOffset(0)] public short Value;
        [FieldOffset(0)] public byte LowByte;
        [FieldOffset(1)] public byte HighByte;
    }
}