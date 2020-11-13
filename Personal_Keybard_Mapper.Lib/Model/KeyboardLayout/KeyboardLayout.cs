using System.Globalization;
using System.Text;
using System.Threading;
using Personal_Keyboard_Mapper.Lib.PInvokeApFunctions;

namespace Personal_Keyboard_Mapper.Lib.Model.KeyboardLayout
{
    /// <summary>
    /// Manipulate keyboard layouts.
    /// </summary>
    public class KeyboardLayout
    {
        private readonly uint hkl;

        private KeyboardLayout(CultureInfo cultureInfo)
        {
            string layoutName = cultureInfo.LCID.ToString("x8");

            var pwszKlid = new StringBuilder(layoutName);
            this.hkl = ApiFunctions.LoadKeyboardLayout(pwszKlid.ToString(), KeyboardLayoutFlags.KLF_ACTIVATE);
        }

        private KeyboardLayout(uint hkl)
        {
            this.hkl = hkl;
        }

        public uint Handle
        {
            get
            {
                return this.hkl;
            }
        }

        public static KeyboardLayout GetCurrent()
        {
            uint hkl = ApiFunctions.GetKeyboardLayout((uint)Thread.CurrentThread.ManagedThreadId);
            return new KeyboardLayout(hkl);
        }

        public static KeyboardLayout Load(CultureInfo culture)
        {
            return new KeyboardLayout(culture);
        }

        public void Activate()
        {
            ApiFunctions.ActivateKeyboardLayout(this.hkl, KeyboardLayoutFlags.KLF_SETFORPROCESS);
        }
    }
}