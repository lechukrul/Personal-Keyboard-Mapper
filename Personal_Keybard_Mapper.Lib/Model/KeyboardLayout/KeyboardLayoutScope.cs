using System;
using System.Globalization;

namespace Personal_Keyboard_Mapper.Lib.Model.KeyboardLayout
{
    public class KeyboardLayoutScope : IDisposable
    {
        public readonly KeyboardLayout currentLayout;

        public KeyboardLayoutScope(CultureInfo culture)
        {
            this.currentLayout = KeyboardLayout.GetCurrent();
            var layout = KeyboardLayout.Load(culture);
            //layout.Activate();
        }

        public void Dispose()
        { 
            //this.currentLayout.Activate();
        }
    }
}