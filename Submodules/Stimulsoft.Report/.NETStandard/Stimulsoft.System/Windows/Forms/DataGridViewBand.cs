using System;
using System.Collections.Generic;
using System.Text;

namespace Stimulsoft.System.Windows.Forms
{
    public class DataGridViewBand : DataGridViewElement, ICloneable, IDisposable
    {
        public virtual DataGridViewCellStyle DefaultCellStyle { get; set; }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
