using System;
using System.Windows.Forms;

namespace WindowsHook.Impl
{
    internal class HookNativeWindow : NativeWindow, IDisposable
    {
        private readonly IntPtr HWND_MESSAGE = new IntPtr(-3);

        private bool disposedValue = false;

        EventRaiserProc EventRaiser;
        public HookNativeWindow(EventRaiserProc EventRaiser)
        {
            this.EventRaiser = EventRaiser;

            CreateParams CreateParams = new CreateParams();
            // Creates a message only window and is used to prevent the hook window from receving its own messages
            CreateParams.Parent = HWND_MESSAGE;
            base.CreateHandle(CreateParams);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue) {
                if (disposing) {
                    base.DestroyHandle();
                }
            }
            disposedValue = true;
        }

        protected override void WndProc(ref Message m)
        {
            if (!Enum.IsDefined(typeof(WindowsEvents), m.Msg)) {
                base.WndProc(ref m);
                //Debug.WriteLine(DirectCast(m.Msg, WindowMessages).ToString)
                return;
            } else {
                int Result = 0;
                IntPtr windowHandle = m.WParam;
                IntPtr additionalData = m.LParam;
                EventRaiser.Invoke((WindowsEvents)m.Msg, windowHandle, additionalData, ref Result);
                m.Result = new IntPtr(Result);
                return;
            }
        }

        #region " IDisposable Support "
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~HookNativeWindow()
        {
            Dispose(false);
        }
        #endregion
    }
}
