using System;
using System.Threading;
using Personal_Keyboard_Mapper.Core.Enums;
using Personal_Keyboard_Mapper.Core.Interfaces;
using Personal_Keyboard_Mapper.Core.Model;
using Personal_Keyboard_Mapper.Linux.Native;

namespace Personal_Keyboard_Mapper.Linux
{
    /// <summary>
    /// Global keyboard hook for Linux. Grabs the physical keyboard device
    /// (EVIOCGRAB), so no key reaches the system directly; keys the handler
    /// does not consume are re-emitted through the uinput virtual device.
    /// This mirrors the WH_KEYBOARD_LL "return 1 / CallNextHookEx" model.
    /// </summary>
    public sealed class EvdevKeyboardHook : IKeyboardHook
    {
        private readonly string devicePath;
        private readonly UinputDevice passthroughDevice;
        private int fd = -1;
        private Thread readThread;
        private volatile bool running;

        public Func<KeyEvent, bool> KeyHandler { get; set; }

        public EvdevKeyboardHook(string devicePath, UinputDevice passthrough)
        {
            this.devicePath = devicePath;
            passthroughDevice = passthrough;
        }

        public void StartHook()
        {
            fd = Libc.open(devicePath, Libc.O_RDONLY);
            if (fd < 0)
            {
                throw new InvalidOperationException(
                    $"Cannot open {devicePath}. Run as root or add user to 'input' group.");
            }
            if (Libc.ioctl(fd, Libc.EVIOCGRAB, 1) < 0)
            {
                Libc.close(fd);
                fd = -1;
                throw new InvalidOperationException($"EVIOCGRAB failed on {devicePath}.");
            }

            running = true;
            readThread = new Thread(ReadLoop) { IsBackground = true, Name = "evdev-hook" };
            readThread.Start();
        }

        public void StopHook()
        {
            running = false;
            if (fd >= 0)
            {
                Libc.ioctl(fd, Libc.EVIOCGRAB, 0);
                Libc.close(fd); // unblocks the pending read
                fd = -1;
            }
        }

        private void ReadLoop()
        {
            var ev = new InputEvent();
            while (running)
            {
                int n = Libc.read(fd, ref ev, 24);
                if (n <= 0)
                {
                    break;
                }
                if (ev.type != Libc.EV_KEY)
                {
                    continue; // EV_SYN/EV_MSC from the grabbed device are not re-emitted
                }

                bool suppress = false;
                if (KeyHandler != null && KeyCodeMap.TryGetVirtualKey(ev.code, out var vk))
                {
                    var state = ev.value switch
                    {
                        Libc.KEY_PRESS => KeyState.Down,
                        Libc.KEY_REPEAT => KeyState.Repeat,
                        _ => KeyState.Up
                    };
                    suppress = KeyHandler(new KeyEvent(vk, state));
                }

                if (!suppress)
                {
                    passthroughDevice.EmitKey(ev.code, ev.value);
                }
            }
        }

        public void Dispose()
        {
            StopHook();
        }
    }
}
