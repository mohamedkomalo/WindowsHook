using System;

namespace WindowsHook
{
    public interface IWindowsHookController : IDisposable
    {
        bool Started { get; }
        void Start();
        void Stop();

        IWindowsHook HookAllWindows();
        IWindowsHook HookWindowByHandle(IntPtr windowHandle);
    }
}
