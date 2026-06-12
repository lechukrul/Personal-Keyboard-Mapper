using System;
using System.Text;
using System.Threading;

namespace Personal_Keyboard_Mapper.Linux.Native
{
    /// <summary>
    /// A virtual keyboard+mouse device created through /dev/uinput.
    /// Used both to re-emit passed-through keys (when the real keyboard is grabbed)
    /// and to send synthetic input from mapped actions.
    /// </summary>
    public sealed class UinputDevice : IDisposable
    {
        private int fd = -1;
        private bool created;

        public UinputDevice(string deviceName = "personal-keyboard-mapper-virtual")
        {
            fd = Libc.open("/dev/uinput", Libc.O_WRONLY);
            if (fd < 0)
            {
                throw new InvalidOperationException(
                    "Cannot open /dev/uinput. Run as root or grant access (group 'input' + udev rule).");
            }

            Libc.ioctl(fd, Libc.UI_SET_EVBIT, Libc.EV_KEY);
            // enable every possible key code so any grabbed key can be passed through
            for (int code = 1; code < 0x2ff; code++)
            {
                Libc.ioctl(fd, Libc.UI_SET_KEYBIT, code);
            }
            Libc.ioctl(fd, Libc.UI_SET_EVBIT, Libc.EV_REL);
            Libc.ioctl(fd, Libc.UI_SET_RELBIT, Libc.REL_X);
            Libc.ioctl(fd, Libc.UI_SET_RELBIT, Libc.REL_Y);

            var setup = new UinputSetup
            {
                bustype = Libc.BUS_VIRTUAL,
                vendor = 0x1234,
                product = 0x5678,
                version = 1,
                name = new byte[80],
                ff_effects_max = 0
            };
            var nameBytes = Encoding.ASCII.GetBytes(deviceName);
            Array.Copy(nameBytes, setup.name, Math.Min(nameBytes.Length, 79));

            if (Libc.ioctl(fd, Libc.UI_DEV_SETUP, ref setup) < 0)
            {
                throw new InvalidOperationException("UI_DEV_SETUP failed.");
            }
            if (Libc.ioctl(fd, Libc.UI_DEV_CREATE, 0) < 0)
            {
                throw new InvalidOperationException("UI_DEV_CREATE failed.");
            }
            created = true;

            // give the system a moment to register the new device
            Thread.Sleep(200);
        }

        public void Emit(ushort type, ushort code, int value)
        {
            var ev = new InputEvent { type = type, code = code, value = value };
            Libc.write(fd, ref ev, 24);
        }

        public void Sync()
        {
            Emit(Libc.EV_SYN, Libc.SYN_REPORT, 0);
        }

        public void EmitKey(ushort evdevCode, int value)
        {
            Emit(Libc.EV_KEY, evdevCode, value);
            Sync();
        }

        public void KeyPress(ushort evdevCode)
        {
            EmitKey(evdevCode, Libc.KEY_PRESS);
            EmitKey(evdevCode, Libc.KEY_RELEASE);
        }

        public void Dispose()
        {
            if (fd >= 0)
            {
                if (created)
                {
                    Libc.ioctl(fd, Libc.UI_DEV_DESTROY, 0);
                }
                Libc.close(fd);
                fd = -1;
            }
        }
    }
}
