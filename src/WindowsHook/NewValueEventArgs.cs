using System;

namespace WindowsHook
{
    public class NewValueEventArgs<T> : EventArgs
    {
        public NewValueEventArgs(T newValue)
        {
            NewValue = newValue;
        }

        public T NewValue { get; set; }
    }
}
