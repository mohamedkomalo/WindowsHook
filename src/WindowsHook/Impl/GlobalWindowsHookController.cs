using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace WindowsHook.Impl
{
    internal delegate void EventRaiserProc(WindowsEvents windowEvent, IntPtr window, IntPtr extendedData, ref int result);

    internal class GlobalWindowsHookController : IWindowsHookController
    {
        private readonly HookNativeWindow _hookWindow;

        private readonly HookEventsRaiser _defaultHookEventsRaiser = new HookEventsRaiser();
        private readonly Dictionary<IntPtr, HookEventsRaiser> _hookedWindows = new Dictionary<IntPtr, HookEventsRaiser>();

        private bool _started;

        public GlobalWindowsHookController()
        {
            _hookWindow = new HookNativeWindow(EventRaiser);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing){
                Stop();
                _hookWindow.Dispose();
            }
        }

        public void Start()
        {
            if (HookDllMethods.StartWindowsHook(_hookWindow.Handle))
            {
                if (IntPtr.Size != 4)
                {
                    Process.Start("WindowsHookRunner32bit.exe",
                        "\"" + Assembly.GetEntryAssembly().GetName().Name + "\" " + 
                        _hookWindow.Handle);
                }
                _started = true;
            }
        }

        public void Stop()
        {
            HookDllMethods.StopWindowsHook();
            _started = false;
        }

        public bool Started
        {
            get { return _started; }
        }

        public IWindowsHook HookAllWindows()
        {
            return _defaultHookEventsRaiser;
        }

        public IWindowsHook HookWindowByHandle(IntPtr windowHandle)
        {
            HookEventsRaiser hookEventsRaiser;
            _hookedWindows.TryGetValue(windowHandle, out hookEventsRaiser);

            if (hookEventsRaiser == null)
            {
                hookEventsRaiser = new HookEventsRaiser();
                _hookedWindows.Add(windowHandle, hookEventsRaiser);
            }

            return hookEventsRaiser;
        }

        void EventRaiser(WindowsEvents windowEvent, IntPtr window, IntPtr extendedData,
            ref int result)
        {
            try
            {
                _defaultHookEventsRaiser.EventRaiser(windowEvent, window, extendedData, ref result);

                
                HookEventsRaiser hookEventsRaiser;

                if (!_hookedWindows.TryGetValue(window, out hookEventsRaiser))
                {
                    return;
                }

                hookEventsRaiser.EventRaiser(windowEvent, window, extendedData, ref result);

                if (windowEvent == WindowsEvents.Closed)
                {
                    _hookedWindows.Remove(window);
                }
            }
            catch
            {
                //ErrorManager.ProccessError(ex);
            }
        }

    }
}