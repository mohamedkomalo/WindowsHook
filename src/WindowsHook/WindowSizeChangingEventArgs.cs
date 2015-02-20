using System;
using System.Drawing;

namespace WindowsHook
{
    public class WindowSizeChangingEventArgs : EventArgs
    {
        public Size NewValue { get; set; }

        public int Result { get; set; }
    }
}
