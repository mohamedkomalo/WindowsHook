using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using WindowsHook.Impl;

namespace WindowsHookRunner32bit
{
    class Program
    {
        private static Process _callingProcess;

        public static void Main(String[] args)
        {
            if (args.Length == 0) return;

            try
            {
                string processName = args[0];
                IntPtr recieverWindowHandle = new IntPtr(int.Parse(args[1]));

                Process[] foundProcesses = Process.GetProcessesByName(processName);

                _callingProcess = foundProcesses[0];
                _callingProcess.EnableRaisingEvents = true;
                _callingProcess.Exited += CallingProcessOnExited;

                if(!HookDllMethods.StartWindowsHook(recieverWindowHandle))
                    throw new Exception("Windows hook didn't not start");

                while (true)
                {
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText("Error.txt", ex.ToString());
            }
            finally
            {
                HookDllMethods.StopWindowsHook();
            }
        }

        private static void CallingProcessOnExited(object sender, EventArgs eventArgs)
        {
            HookDllMethods.StopWindowsHook();
            Environment.Exit(0);
        }
    }
}
