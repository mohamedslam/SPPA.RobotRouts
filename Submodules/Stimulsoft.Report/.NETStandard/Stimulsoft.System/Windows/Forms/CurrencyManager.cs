using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Stimulsoft.System.Windows.Forms
{
    public class CurrencyManager : BindingManagerBase
    {
        public int Position { get; set; }
        public int Count { get; set; }
        public DataRowView Current { get; set; }
    }
}
