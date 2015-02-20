using System;

namespace WindowsHook
{
    public class WindowDeactivatingEventArgs : EventArgs
    {
        public WindowDeactivatingEventArgs(IntPtr willBeActivated)
        {
            WillBeActivated = willBeActivated;
        }

        public IntPtr WillBeActivated { get; set; }
    }
}
