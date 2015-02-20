using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace WindowsHook.Impl
{
    internal class HookEventsRaiser : IWindowsHook
    {
        public event WindowsHookEventHandler<IntPtr, EventArgs> WindowActivating;

        public event WindowsHookEventHandler<IntPtr, WindowDeactivatingEventArgs> WindowDeactivating;

        public event WindowsHookEventHandler<IntPtr, EventArgs> WindowClosed;

        public event WindowsHookEventHandler<IntPtr, EventArgs> WindowSizeMoveBegins;

        public event WindowsHookEventHandler<IntPtr, EventArgs> WindowSizeMoveEnds;

        public event WindowsHookEventHandler<IntPtr, EventArgs> ManualUpdate;

        public event WindowsHookEventHandler<IntPtr, NewValueEventArgs<Point>> WindowLocationChanging;

        public event WindowsHookEventHandler<IntPtr, NewValueEventArgs<string>> WindowTitleChanging;

        public event WindowsHookEventHandler<IntPtr, NewValueEventArgs<bool>> WindowVisibleChanging;

        public event WindowsHookEventHandler<IntPtr, NewValueEventArgs<bool>> WindowEnableChanging;

        public event WindowsHookEventHandler<IntPtr, NewValueEventArgs<Icon>> WindowIconChanging;

        public event WindowsHookEventHandler<IntPtr, WindowSizeChangingEventArgs> WindowSizeChanging;

        public event WindowsHookEventHandler<IntPtr, EventArgs> WindowMaximized;

        internal void EventRaiser(WindowsEvents windowEvent, IntPtr window, IntPtr extendedData,
           ref int result)
        {
            try
            {
                switch (windowEvent)
                {
                    case WindowsEvents.Activating:
                        RaiseEvent(WindowActivating, window, EventArgs.Empty);

                        break;
                    case WindowsEvents.DeActivating:
                        var willBeActivated = extendedData;
                        RaiseEvent(WindowDeactivating, window, new WindowDeactivatingEventArgs(willBeActivated));

                        break;
                    case WindowsEvents.Closed:
                        RaiseEvent(WindowClosed, window, EventArgs.Empty);

                        break;
                    case WindowsEvents.SizeMoveBegins:
                        RaiseEvent(WindowSizeMoveBegins, window, EventArgs.Empty);

                        break;
                    case WindowsEvents.SizeMoveEnds:
                        RaiseEvent(WindowSizeMoveEnds, window, EventArgs.Empty);

                        break;
                    case WindowsEvents.ManualUpdate:
                        RaiseEvent(ManualUpdate, window, EventArgs.Empty);
                        break;

                    case WindowsEvents.LocationChanging:
                        short newX = NativeMethods.LoWord(extendedData);
                        short newY = NativeMethods.HiWord(extendedData);

                        RaiseEvent(WindowLocationChanging, window,
                            new NewValueEventArgs<Point>(new Point(newX, newY)));

                        break;
                    case WindowsEvents.SizeChanging:
                        short newWidth = NativeMethods.LoWord(extendedData);
                        short newHeight = NativeMethods.HiWord(extendedData);

                        var args = new WindowSizeChangingEventArgs
                        {
                            NewValue = new Size(newWidth, newHeight)
                        };

                        RaiseEvent(WindowSizeChanging, window, args);

                        result = args.Result;

                        break;

                    case WindowsEvents.Maximized:
                        RaiseEvent(WindowMaximized, window, EventArgs.Empty);

                        break;
                    case WindowsEvents.IconChanging:
                        Icon newIcon = extendedData != IntPtr.Zero ? Icon.FromHandle(extendedData) : null;
                        RaiseEvent(WindowIconChanging, window, new NewValueEventArgs<Icon>(newIcon));

                        break;
                    case WindowsEvents.TitleChanging:
                        string newTitle = Marshal.PtrToStringAuto(extendedData);
                        RaiseEvent(WindowTitleChanging, window, new NewValueEventArgs<String>(newTitle));

                        break;
                    case WindowsEvents.VisibleChanging:
                        RaiseEvent(WindowVisibleChanging, window,
                            new NewValueEventArgs<bool>(extendedData.ToInt32() != 0));

                        break;
                    case WindowsEvents.EnabledChanging:
                        RaiseEvent(WindowEnableChanging, window,
                            new NewValueEventArgs<bool>(extendedData.ToInt32() != 0));
                        break;
                }
            }
            catch (Exception ex)
            {
                //ErrorManager.ProccessError(ex);
            }
        }

        private void RaiseEvent<TEventArgs>(WindowsHookEventHandler<IntPtr, TEventArgs> eventHandler,
            IntPtr windowHandle, TEventArgs args) where TEventArgs : EventArgs
        {
            if (eventHandler != null)
            {
                eventHandler(windowHandle, args);
            }
        }


        private static class NativeMethods
        {
            public static Int16 HiWord(IntPtr Number)
            {
                return BitConverter.ToInt16(BitConverter.GetBytes(Number.ToInt32()), 2);
            }

            public static Int16 LoWord(IntPtr Number)
            {
                return BitConverter.ToInt16(BitConverter.GetBytes(Number.ToInt32()), 0);
            }
        }
    }
}
