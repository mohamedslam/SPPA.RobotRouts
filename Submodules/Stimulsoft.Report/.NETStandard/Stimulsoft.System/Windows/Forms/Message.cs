using System;

namespace Stimulsoft.System.Windows.Forms
{
    public class Message
    {
        public int Msg { get; set; }

        public IntPtr WParam { get; set; }

        public IntPtr Result { get; set; }

        public IntPtr LParam { get; set; }

        public object GetLParam(Type cls)
        {
            throw new NotImplementedException();
        }

        public static Message Create(IntPtr handle, int v, IntPtr ptr, IntPtr zero)
        {
            throw new NotImplementedException();
        }
    }
}
