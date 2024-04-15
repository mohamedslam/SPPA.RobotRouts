using System;
using System.Collections.Generic;
using System.Text;

namespace Stimulsoft.System.Windows.Forms
{
    public class Timer
    {
        public int Interval { get; set; }
        public Action<object, EventArgs> Tick { get; set; }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
