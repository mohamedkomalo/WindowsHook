using WindowsHook.Impl;

namespace WindowsHook
{
    public class WindowsHookControllerFactory
    {
        public static IWindowsHookController CreateGlobalWindowsHook()
        {
            return new GlobalWindowsHookController();
            ;
        }
    }
}
