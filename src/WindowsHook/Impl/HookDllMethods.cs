using System;
using System.Runtime.InteropServices;

namespace WindowsHook.Impl
{
    internal class HookDllMethods
    {
        private const string DLL_32 = "WindowsHookNative32Bit.dll";

        private const string DLL_64 = "WindowsHookNative64Bit.dll";

        [DllImport(DLL_64, CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true, EntryPoint = "StartWindowsHook")]
        private static extern bool StartWindowsHook64(IntPtr WindowHandle);

        [DllImport(DLL_64, CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true, EntryPoint = "StopWindowsHook")]
        private static extern bool StopWindowsHook64();

        [DllImport(DLL_32, CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true, EntryPoint = "StartWindowsHook")]
        private static extern bool StartWindowsHook32(IntPtr WindowHandle);

        [DllImport(DLL_32, CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true, EntryPoint = "StopWindowsHook")]
        private static extern bool StopWindowsHook32();

        internal static bool StartWindowsHook(IntPtr WindowHandle)
        {
            //
            return IsCurrentProcess32Bit ? StartWindowsHook32(WindowHandle) : StartWindowsHook64(WindowHandle);
        }

        internal static bool StopWindowsHook()
        {
            return IsCurrentProcess32Bit ? StopWindowsHook32() : StopWindowsHook64();
        }

        private static bool IsCurrentProcess32Bit
        {
            get { return IntPtr.Size == 4; }
        }
    }
}
