using System;
using System.Runtime.InteropServices;

namespace Personal_Keyboard_Mapper.Linux.Native
{
    /// <summary>
    /// input_event layout on 64-bit Linux: timeval (16 bytes) + type + code + value = 24 bytes.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct InputEvent
    {
        public long tv_sec;
        public long tv_usec;
        public ushort type;
        public ushort code;
        public int value;
    }

    /// <summary>
    /// struct uinput_setup: input_id (8 bytes) + name[80] + ff_effects_max = 92 bytes.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct UinputSetup
    {
        public ushort bustype;
        public ushort vendor;
        public ushort product;
        public ushort version;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
        public byte[] name;
        public uint ff_effects_max;
    }

    public static class Libc
    {
        public const int O_RDONLY = 0;
        public const int O_WRONLY = 1;
        public const int O_RDWR = 2;

        // event types
        public const ushort EV_SYN = 0;
        public const ushort EV_KEY = 1;
        public const ushort EV_REL = 2;
        public const ushort SYN_REPORT = 0;
        public const ushort REL_X = 0;
        public const ushort REL_Y = 1;

        // key event values
        public const int KEY_RELEASE = 0;
        public const int KEY_PRESS = 1;
        public const int KEY_REPEAT = 2;

        // mouse buttons (evdev codes)
        public const ushort BTN_LEFT = 0x110;
        public const ushort BTN_RIGHT = 0x111;

        // ioctl request codes
        public const uint EVIOCGRAB = 0x40044590;      // _IOW('E', 0x90, int)
        public const uint UI_SET_EVBIT = 0x40045564;   // _IOW('U', 100, int)
        public const uint UI_SET_KEYBIT = 0x40045565;  // _IOW('U', 101, int)
        public const uint UI_SET_RELBIT = 0x40045566;  // _IOW('U', 102, int)
        public const uint UI_DEV_SETUP = 0x405C5503;   // _IOW('U', 3, struct uinput_setup)
        public const uint UI_DEV_CREATE = 0x5501;      // _IO('U', 1)
        public const uint UI_DEV_DESTROY = 0x5502;     // _IO('U', 2)

        public const ushort BUS_VIRTUAL = 0x06;

        [DllImport("libc", SetLastError = true)]
        public static extern int open([MarshalAs(UnmanagedType.LPStr)] string path, int flags);

        [DllImport("libc", SetLastError = true)]
        public static extern int close(int fd);

        [DllImport("libc", SetLastError = true)]
        public static extern int read(int fd, ref InputEvent ev, int count);

        [DllImport("libc", SetLastError = true)]
        public static extern int write(int fd, ref InputEvent ev, int count);

        [DllImport("libc", SetLastError = true)]
        public static extern int ioctl(int fd, uint request, int arg);

        [DllImport("libc", SetLastError = true)]
        public static extern int ioctl(int fd, uint request, ref UinputSetup setup);
    }
}
