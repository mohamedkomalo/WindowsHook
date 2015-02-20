using System;
using System.Drawing;

namespace WindowsHook
{
    public delegate void WindowsHookEventHandler<in TSender, in TEventArgs>(TSender windowHandle, TEventArgs args)
        where TEventArgs : EventArgs;

    public interface IWindowsHook
    {
        event WindowsHookEventHandler<IntPtr, EventArgs> WindowActivating;
        event WindowsHookEventHandler<IntPtr, WindowDeactivatingEventArgs> WindowDeactivating;
        event WindowsHookEventHandler<IntPtr, EventArgs> WindowClosed;
        event WindowsHookEventHandler<IntPtr, EventArgs> WindowSizeMoveBegins;
        event WindowsHookEventHandler<IntPtr, EventArgs> WindowSizeMoveEnds;
        event WindowsHookEventHandler<IntPtr, EventArgs> ManualUpdate;
        event WindowsHookEventHandler<IntPtr, NewValueEventArgs<Point>> WindowLocationChanging;
        event WindowsHookEventHandler<IntPtr, NewValueEventArgs<String>> WindowTitleChanging;
        event WindowsHookEventHandler<IntPtr, NewValueEventArgs<bool>> WindowVisibleChanging;
        event WindowsHookEventHandler<IntPtr, NewValueEventArgs<bool>> WindowEnableChanging;
        event WindowsHookEventHandler<IntPtr, NewValueEventArgs<Icon>> WindowIconChanging;
        event WindowsHookEventHandler<IntPtr, WindowSizeChangingEventArgs> WindowSizeChanging;
        event WindowsHookEventHandler<IntPtr, EventArgs> WindowMaximized;
    }
}