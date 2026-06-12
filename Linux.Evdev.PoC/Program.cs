using System;
using System.Runtime.InteropServices;

// input_event layout on 64-bit Linux: timeval (16 bytes) + type + code + value = 24 bytes
[StructLayout(LayoutKind.Sequential)]
struct InputEvent
{
    public long tv_sec;
    public long tv_usec;
    public ushort type;
    public ushort code;
    public int value;
}

class Program
{
    const int O_RDONLY = 0;
    const ushort EV_KEY = 1;

    [DllImport("libc", SetLastError = true)]
    static extern int open([MarshalAs(UnmanagedType.LPStr)] string path, int flags);

    [DllImport("libc", SetLastError = true)]
    static extern int close(int fd);

    [DllImport("libc", SetLastError = true)]
    static extern int read(int fd, ref InputEvent ev, int count);

    static void Main(string[] args)
    {
        string device = args.Length > 0 ? args[0] : "/dev/input/event0";

        int fd = open(device, O_RDONLY);
        if (fd < 0)
        {
            Console.Error.WriteLine($"Cannot open {device}. Run as root or add user to 'input' group.");
            Environment.Exit(1);
        }

        Console.WriteLine($"Listening on {device} — press keys (Ctrl+C to exit)...");

        var ev = new InputEvent();
        int size = Marshal.SizeOf<InputEvent>();

        while (true)
        {
            int n = read(fd, ref ev, size);
            if (n < 0) break;

            if (ev.type == EV_KEY)
            {
                string action = ev.value switch { 0 => "UP", 1 => "DOWN", 2 => "REPEAT", _ => "?" };
                Console.WriteLine($"code={ev.code,-4} {action}");
            }
        }

        close(fd);
    }
}
