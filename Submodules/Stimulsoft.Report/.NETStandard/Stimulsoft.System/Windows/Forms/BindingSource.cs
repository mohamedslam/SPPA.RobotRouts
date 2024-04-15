using System;
using System.Collections.Generic;
using System.Text;

namespace Stimulsoft.System.Windows.Forms
{
    public class BindingSource
    {
        public object DataSource { get; set; }
        public IList<object> List { get; set; }
    }
}
